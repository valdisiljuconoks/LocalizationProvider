// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonConverter = DbLocalizationProvider.Json.JsonConverter;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider;

/// <summary>
/// Handler to respond with client-side JSON document.
/// </summary>
public class RequestHandler
{
    /// <summary>
    /// Must have, otherwise exception at runtime.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline.</param>
    public RequestHandler(RequestDelegate next) { }

    /// <summary>
    /// Main execution method.
    /// </summary>
    /// <param name="context">Http context. Will be used to write response to.</param>
    /// <returns>Task (for async).</returns>
    public async Task Invoke(HttpContext context)
    {
        var response = GenerateResponse(context);
        await context.Response.WriteAsync(response);
    }

    private string GenerateResponse(HttpContext context)
    {
        context.Response.ContentType = "application/javascript";

        var queryExecutor = context.RequestServices.GetRequiredService<IQueryExecutor>();

        var languageName = !context.Request.Query.ContainsKey("lang")
            ? queryExecutor.Execute(new GetCurrentUICulture.Query()).Name
            : context.Request.Query["lang"].ToString();

        var filename = ExtractFileName(context);

        if (filename == ClientsideConfigurationContext.DeepMergeScriptName)
        {
            return
                "/* https://github.com/KyleAMathews/deepmerge */ var jsResourceHandler=function(){function r(r){return!(c=r,!c||'object'!=typeof c||(t=r,n=Object.prototype.toString.call(t),'[object RegExp]'===n||'[object Date]'===n||(o=t,o.$$typeof===e)));var t,n,o,c}var e='function'==typeof Symbol&&Symbol.for?Symbol.for('react.element'):60103;function t(e,t){var n;return(!t||!1!==t.clone)&&r(e)?o((n=e,Array.isArray(n)?[]:{}),e,t):e}function n(r,e,n){return r.concat(e).map(function(r){return t(r,n)})}function o(e,c,a){var u,f,y,i,b=Array.isArray(c);return b===Array.isArray(e)?b?((a||{arrayMerge:n}).arrayMerge||n)(e,c,a):(f=c,y=a,i={},r(u=e)&&Object.keys(u).forEach(function(r){i[r]=t(u[r],y)}),Object.keys(f).forEach(function(e){r(f[e])&&u[e]?i[e]=o(u[e],f[e],y):i[e]=t(f[e],y)}),i):t(c,a)}return{deepmerge:function(r,e,t){return o(r,e,t)}}}();";
        }

        var debugMode = context.Request.Query.ContainsKey("debug");
        var camelCase = context.Request.Query.ContainsKey("camel");
        var alias = !context.Request.Query.ContainsKey("alias")
            ? ClientsideConfigurationContext.DefaultAlias
            : context.Request.Query["alias"].ToString();
        var cacheKey = CacheHelper.GenerateKey(filename, languageName, debugMode, camelCase);
        var cache = context.RequestServices.GetService<ICacheManager>();

        // trying to detect response format
        var windowAlias = true;
        if (context.Request.Query.ContainsKey("json"))
        {
            windowAlias = false;
        }
        else if (context.Request.Headers.ContainsKey("X-Requested-With")
                 && context.Request.Headers["X-Requested-With"].Contains("XMLHttpRequest"))
        {
            windowAlias = false;
        }
        else if (context.Request.GetTypedHeaders().Accept.Contains(new MediaTypeHeaderValue("application/json")))
        {
            windowAlias = false;
        }

        if (!(cache.Get(cacheKey) is string responseObject))
        {
            responseObject = GetJson(filename,
                                     languageName,
                                     debugMode,
                                     camelCase,
                                     context.RequestServices.GetRequiredService<IQueryExecutor>(),
                                     context.RequestServices.GetRequiredService<IOptions<ConfigurationContext>>(),
                                     context.RequestServices.GetRequiredService<ScanState>());

            cache.Insert(cacheKey, responseObject, false);
        }

        if (windowAlias)
        {
            responseObject = $"window.{alias} = jsResourceHandler.deepmerge(window.{alias} || {{}}, {responseObject})";
        }
        else
        {
            context.Response.ContentType = "application/json";
        }

        return responseObject;
    }

    private string GetJson(
        string filename,
        string languageName,
        bool debugMode,
        bool camelCase,
        IQueryExecutor queryExecutor,
        IOptions<ConfigurationContext> configurationContext,
        ScanState scanState)
    {
        var settings = new JsonSerializerSettings();
        var converter = new JsonConverter(queryExecutor, scanState);

        if (camelCase)
        {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        if (debugMode)
        {
            settings.Formatting = Formatting.Indented;
        }

        return JsonConvert.SerializeObject(
            converter.GetJson(filename, languageName, configurationContext.Value._fallbackCollection, camelCase),
            settings);
    }

    private static string ExtractFileName(HttpContext context)
    {
        var result = context.Request.Path.ToString().Replace(ClientsideConfigurationContext.RootPath, string.Empty);
        result = result.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? result.TrimStart('/') : result;
        result = result.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? result.TrimEnd('/') : result;

        return result.Replace("---", "+");
    }
}
