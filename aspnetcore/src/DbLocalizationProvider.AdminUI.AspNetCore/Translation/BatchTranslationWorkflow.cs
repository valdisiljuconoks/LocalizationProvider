// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.AdminUI.Models;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Translation;

/// <summary>
/// Orchestrates batch machine-translation: a preview pass that translates without persisting,
/// and an apply pass that commits the confirmed translations.
/// </summary>
internal sealed class BatchTranslationWorkflow(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
{
    /// <summary>
    /// Translates the requested resources but does NOT persist anything - the result is a preview only.
    /// </summary>
    public async Task<IReadOnlyList<BatchTranslateItem>> PreviewAsync(
        ITranslatorService translator,
        IReadOnlyList<string> keys,
        CultureInfo sourceLanguage,
        CultureInfo targetLanguage,
        bool onlyEmpty)
    {
        if (keys.Count == 0 || sourceLanguage.Equals(targetLanguage))
        {
            return [];
        }

        var all = queryExecutor.Execute(new GetAllResources.Query(true))!;
        var candidates = new List<BatchTranslateItem>();

        foreach (var key in keys)
        {
            if (!all.TryGetValue(key, out var resource) || resource == null)
            {
                continue;
            }

            var sourceText = resource.Translations.FindByLanguage(sourceLanguage)?.Value;
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                continue;
            }

            if (onlyEmpty && !string.IsNullOrWhiteSpace(resource.Translations.FindByLanguage(targetLanguage)?.Value))
            {
                continue;
            }

            candidates.Add(new BatchTranslateItem { Key = key, SourceText = sourceText });
        }

        if (candidates.Count == 0)
        {
            return [];
        }

        var translations = await translator
            .TranslateAsync(candidates.Select(c => c.SourceText).ToList(), targetLanguage, sourceLanguage)
            .ConfigureAwait(false);

        for (var i = 0; i < candidates.Count; i++)
        {
            var result = translations[i];
            candidates[i].Success = result.IsSuccessful;
            candidates[i].Translation = result.Result;
            candidates[i].Error = result.Error?.Message;
        }

        return candidates;
    }

    /// <summary>
    /// Persists the supplied translations for the target language.
    /// </summary>
    public void Apply(CultureInfo targetLanguage, IEnumerable<BatchTranslateApplyItem> items)
    {
        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Key) || item.Translation == null)
            {
                continue;
            }

            commandExecutor.Execute(new CreateOrUpdateTranslation.Command(item.Key, targetLanguage, item.Translation));
        }
    }
}
