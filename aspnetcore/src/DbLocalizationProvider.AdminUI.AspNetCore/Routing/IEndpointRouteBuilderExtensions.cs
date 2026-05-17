// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Routing;

/// <summary>
/// Endpoint routing extensions for AdminUI.
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps AdminUI service endpoints as minimal-API routes under <see cref="UiConfigurationContext.RootUrl"/>/api/service.
    /// Call this from <c>UseEndpoints</c> (or top-level routing). It does not require <c>MapControllers()</c>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The same builder to allow chaining.</returns>
    public static IEndpointRouteBuilder MapDbLocalizationAdminUI(this IEndpointRouteBuilder endpoints)
    {
        var rootUrl = endpoints.ServiceProvider
            .GetRequiredService<IOptions<UiConfigurationContext>>()
            .Value.RootUrl;

        var group = endpoints
            .MapGroup(rootUrl + "/api/service")
            .RequireAuthorization(AccessPolicy.Name);

        group.MapGet("/get", AdminUIEndpoints.Get);
        group.MapGet("/gettree", AdminUIEndpoints.GetTree);
        group.MapPost("/save", AdminUIEndpoints.Save);
        group.MapPost("/add", AdminUIEndpoints.Add);
        group.MapPost("/validate", AdminUIEndpoints.ValidateFile).DisableAntiforgery();
        group.MapPost("/import", AdminUIEndpoints.ImportFile);
        group.MapPost("/remove", AdminUIEndpoints.Remove);
        group.MapPost("/delete", AdminUIEndpoints.Delete);
        group.MapPost("/bulk-delete", AdminUIEndpoints.BulkDelete);
        group.MapGet("/auto-translate", AdminUIEndpoints.AutoTranslate);

        return endpoints;
    }
}
