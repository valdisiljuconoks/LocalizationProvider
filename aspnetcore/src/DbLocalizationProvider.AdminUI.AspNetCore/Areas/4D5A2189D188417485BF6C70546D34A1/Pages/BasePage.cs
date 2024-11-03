// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using System.Text;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Areas._4D5A2189D188417485BF6C70546D34A1.Pages;

public class BasePage : PageModel
{
    private const string _lastViewCookieName = "LocalizationProvider_LastView";
    private readonly ICommandExecutor _commandExecutor;
    private readonly ConfigurationContext _configurationContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly UiConfigurationContext _uiConfigurationContext;

    public BasePage(
        IOptions<ConfigurationContext> configurationContext,
        IOptions<UiConfigurationContext> uiConfigurationContext,
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor)
    {
        _configurationContext = configurationContext?.Value ?? throw new ArgumentNullException(nameof(configurationContext));
        _uiConfigurationContext = uiConfigurationContext?.Value ?? throw new ArgumentNullException(nameof(uiConfigurationContext));
        _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    public HtmlString IncludeStyle(string cssFileName)
    {
        return new HtmlString(
            $"<link rel=\"stylesheet\" href=\"/_content/LocalizationProvider.AdminUI.AspNetCore/css/{cssFileName}\">");
    }

    public HtmlString IncludeScript(string scriptFileName)
    {
        return new HtmlString(
            $"<script src=\"/_content/LocalizationProvider.AdminUI.AspNetCore/js/{scriptFileName}\"></script>");
    }

    public IActionResult OnGet()
    {
        // this is need in order for the Vue and other "sub-components" to load correctly.
        // otherwise - resources will be mapped to one level above and will fail to be fetched
        var url = Request.GetEncodedUrl();
        if (!url.EndsWith('/'))
        {
            return Redirect(url + "/");
        }

        var lastView = Request.Cookies[_lastViewCookieName];
        if (!string.IsNullOrEmpty(lastView))
        {
            return Page();
        }

        var defaultView = _uiConfigurationContext.DefaultView;
        var isTreeView = url.Contains("tree");

        // set view from config
        Response.Cookies.Append(_lastViewCookieName, defaultView.ToString(), new CookieOptions { HttpOnly = true });
        if (!isTreeView && defaultView == ResourceListView.Tree)
        {
            Response.Redirect(url + "tree/");
        }

        return Page();
    }

    public FileResult OnGetExport(string format = "json")
    {
        var exporter = _configurationContext.Export.Providers.FindById(format);
        var resourcesQuery = new GetAllResources.Query(true);
        var resources = _queryExecutor.Execute(resourcesQuery);

        var result = exporter.Export(resources.ToList(), Request.Query?.ToDictionary(x => x.Key, x => x.Value.ToArray()));

        return new FileContentResult(Encoding.UTF8.GetBytes(result.SerializedData), result.FileMimeType)
        {
            FileDownloadName = result.FileName
        };
    }

    public IActionResult OnGetCleanCache()
    {
        _commandExecutor.Execute(new ClearCache.Command());

        return Page();
    }
}
