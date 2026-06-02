// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.AspNetCore.Translation;
using DbLocalizationProvider.AdminUI.Models;
using DbLocalizationProvider.AspNetCore.Import;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Routing;

internal static class AdminUIEndpoints
{
    private static readonly JsonSerializerSettings resolver = new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    private static readonly JsonSerializerSettings settings =
        new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

    public static IResult Get(
        string? keyword,
        IQueryExecutor queries,
        IOptions<UiConfigurationContext> uiConfig,
        HttpContext httpContext)
    {
        return CamelCaseJson(BuildListModel(keyword?.Trim(), queries, uiConfig.Value, httpContext));
    }

    public static IResult GetTree(
        IQueryExecutor queries,
        IOptions<UiConfigurationContext> uiConfig,
        IOptions<ConfigurationContext> configurationContext,
        HttpContext httpContext)
    {
        var resources = GetResources(queries);
        var languages = GetAvailableLanguages(queries);
        var visibleLanguages = GetVisibleLanguages(httpContext, languages);

        var model = new LocalizationResourceApiTreeModel(
            resources,
            languages,
            visibleLanguages ?? languages,
            uiConfig.Value.MaxResourceKeyPopupTitleLength,
            uiConfig.Value.MaxResourceKeyDisplayLength,
            BuildOptions(uiConfig.Value),
            configurationContext.Value.EnableLegacyMode());

        return CamelCaseJson(model);
    }

    public static IResult Save(CreateOrUpdateTranslationRequestModel model, ICommandExecutor commands)
    {
        commands.Execute(new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation));
        return OkStatus();
    }

    public static IResult SaveNotes(UpdateResourceNotesRequestModel model, ICommandExecutor commands)
    {
        commands.Execute(new UpdateResourceNotes.Command(model.Key, model.Notes));
        return OkStatus();
    }

    public static IResult Add(AddResourceAndTranslationRequestModel model, ICommandExecutor commands)
    {
        var resource = new LocalizationResource(model.Key, true) { IsHidden = false, IsModified = true };
        resource.Translations.Add(new LocalizationResourceTranslation { Value = model.Translation, Language = model.Language });

        commands.Execute(new CreateNewResource.Command(resource));
        return OkStatus();
    }

    public static async Task<IResult> ValidateFile(
        IFormFile? importFile,
        IOptions<ConfigurationContext> configurationContext,
        ICommandExecutor commands,
        IQueryExecutor queries)
    {
        if (importFile == null)
        {
            return CamelCaseJson(null);
        }

        using var reader = new StreamReader(importFile.OpenReadStream());
        var content = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(content))
        {
            return CamelCaseJson(null);
        }

        var parser = configurationContext.Value.Import.Providers.FindByExtension(new FileInfo(importFile.FileName).Extension);
        var parsed = parser!.Parse(content);
        var workflow = new ResourceImportWorkflow(commands, queries);
        var detectedChanges = workflow.DetectChanges(parsed.Resources, GetResources(queries));

        return CamelCaseJson(detectedChanges);
    }

    public static async Task<IResult> ImportFile(HttpRequest request, ICommandExecutor commands, IQueryExecutor queries)
    {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();
        var changes = JsonConvert.DeserializeObject<List<DetectedImportChange>>(body, settings) ?? [];

        var selected = changes.Where(c => c.Selected).ToList();
        var workflow = new ResourceImportWorkflow(commands, queries);
        var messages = workflow.ImportChanges(selected).ToList();

        return Results.Json(messages);
    }

    public static IResult Remove(RemoveTranslationRequestModel model, ICommandExecutor commands)
    {
        commands.Execute(new RemoveTranslation.Command(model.Key, new CultureInfo(model.Language)));
        return OkStatus();
    }

    public static IResult Delete(
        DeleteResourceRequestModel model,
        ICommandExecutor commands,
        IOptions<UiConfigurationContext> uiConfig)
    {
        if (!uiConfig.Value.HideDeleteButton)
        {
            commands.Execute(new DeleteResource.Command(model.Key));
        }

        return OkStatus();
    }

    public static IResult BulkDelete(
        BulkDeleteResourcesRequestModel model,
        ICommandExecutor commands,
        IOptions<UiConfigurationContext> uiConfig)
    {
        if (!uiConfig.Value.HideDeleteButton && model.Keys is { Length: > 0 })
        {
            commands.Execute(new BulkDeleteResources.Command(model.Keys));
        }

        return OkStatus();
    }

    public static async Task<IResult> AutoTranslate(string inputText, string targetLanguage, HttpContext httpContext)
    {
        var translator = httpContext.RequestServices.GetService<ITranslatorService>();
        if (translator == null)
        {
            return Results.BadRequest("Translator service is not configured.");
        }

        var result = await translator.TranslateAsync(
            inputText,
            CultureInfo.GetCultureInfo(targetLanguage),
            CultureInfo.InvariantCulture);

        return Results.Ok(result);
    }

    public static async Task<IResult> BatchTranslatePreview(
        BatchTranslatePreviewRequestModel model,
        IQueryExecutor queries,
        ICommandExecutor commands,
        HttpContext httpContext)
    {
        var translator = httpContext.RequestServices.GetService<ITranslatorService>();
        if (translator == null)
        {
            return Results.BadRequest("Translator service is not configured.");
        }

        if (model.Keys is not { Length: > 0 } || string.IsNullOrEmpty(model.TargetLanguage))
        {
            return CamelCaseJson(new BatchTranslatePreviewModel { Language = model.TargetLanguage ?? string.Empty });
        }

        var source = ParseSourceLanguage(model.SourceLanguage);
        var target = new CultureInfo(model.TargetLanguage);

        var workflow = new BatchTranslationWorkflow(queries, commands);
        var results = await workflow.PreviewAsync(translator, model.Keys, source, target, model.OnlyEmpty);

        return CamelCaseJson(new BatchTranslatePreviewModel { Language = target.Name, Results = results.ToArray() });
    }

    public static IResult BatchTranslateApply(
        BatchTranslateApplyRequestModel model,
        IQueryExecutor queries,
        ICommandExecutor commands)
    {
        if (string.IsNullOrEmpty(model.TargetLanguage) || model.Items is not { Length: > 0 })
        {
            return OkStatus();
        }

        var workflow = new BatchTranslationWorkflow(queries, commands);
        workflow.Apply(new CultureInfo(model.TargetLanguage), model.Items);

        return OkStatus();
    }

    private static CultureInfo ParseSourceLanguage(string? language) =>
        string.IsNullOrEmpty(language) || language.Equals("invariant", StringComparison.OrdinalIgnoreCase)
            ? CultureInfo.InvariantCulture
            : new CultureInfo(language);

    private static LocalizationResourceApiModel BuildListModel(
        string? keyword,
        IQueryExecutor queries,
        UiConfigurationContext uiConfig,
        HttpContext httpContext)
    {
        var languages = GetAvailableLanguages(queries);
        List<LocalizationResource> resources;

        if (uiConfig.EnableDbSearch)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                resources = [];
            }
            else if (keyword.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                resources = GetResources(queries);
            }
            else
            {
                resources = GetResources(queries)
                    .Where(r => r.ResourceKey.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    .Take(uiConfig.PageSize)
                    .ToList();
            }
        }
        else
        {
            resources = GetResources(queries);
        }

        var visibleLanguages = GetVisibleLanguages(httpContext, languages);

        return new LocalizationResourceApiModel(
            resources,
            languages,
            visibleLanguages ?? languages,
            uiConfig.MaxResourceKeyPopupTitleLength,
            uiConfig.MaxResourceKeyDisplayLength,
            BuildOptions(uiConfig));
    }

    private static IEnumerable<AvailableLanguage>? GetVisibleLanguages(
        HttpContext httpContext,
        IEnumerable<AvailableLanguage> availableLanguages)
    {
        var cookie = httpContext.Request.Cookies["LocalizationProvider_VisibleLanguages"];
        if (string.IsNullOrEmpty(cookie))
        {
            return null;
        }

        var languagesInCookie = cookie
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.Equals("invariant", StringComparison.InvariantCultureIgnoreCase))
            .Select(l => new CultureInfo(l))
            .ToList();

        return availableLanguages.Where(a => languagesInCookie.Contains(a.CultureInfo));
    }

    private static List<LocalizationResource> GetResources(IQueryExecutor queries) =>
        queries.Execute(new GetAllResources.Query(true))!
            .Select(kv => kv.Value)
            .OrderBy(r => r.ResourceKey)
            .ToList();

    private static ICollection<AvailableLanguage> GetAvailableLanguages(IQueryExecutor queries) =>
        queries.Execute(new AvailableLanguages.Query { IncludeInvariant = false })!.ToList();

    private static UiOptions BuildOptions(UiConfigurationContext uiConfig) =>
        new()
        {
            AdminMode = true,
            ShowInvariantCulture = uiConfig.ShowInvariantCulture,
            ShowHiddenResources = uiConfig.ShowHiddenResources,
            ShowDeleteButton = !uiConfig.HideDeleteButton,
            EnableDbSearch = uiConfig.EnableDbSearch
        };

    private static IResult OkStatus() => Results.Json(new { status = "Ok" });

    private static IResult CamelCaseJson(object? value) => new CamelCaseJsonResult(value);

    private sealed class CamelCaseJsonResult(object? value) : IResult
    {
        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(value, resolver));
        }
    }
}
