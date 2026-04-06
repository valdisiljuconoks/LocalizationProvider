using DbLocalizationProvider.Optimizely.Sandbox.Business;
using DbLocalizationProvider.Optimizely.Sandbox.Business.Channels;
using DbLocalizationProvider.Optimizely.Sandbox.Business.Rendering;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DbLocalizationProvider.Optimizely.Sandbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAlloy(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new SiteViewEngineLocationExpander()));

        services.Configure<DisplayOptions>(displayOption =>
        {
            displayOption.Add("full", "/displayoptions/full", Globals.ContentAreaTags.FullWidth, string.Empty, "epi-icon__layout--full");
            displayOption.Add("wide", "/displayoptions/wide", Globals.ContentAreaTags.WideWidth, string.Empty, "epi-icon__layout--wide");
            displayOption.Add("half", "/displayoptions/half", Globals.ContentAreaTags.HalfWidth, string.Empty, "epi-icon__layout--half");
            displayOption.Add("narrow", "/displayoptions/narrow", Globals.ContentAreaTags.NarrowWidth, string.Empty, "epi-icon__layout--narrow");
        });

        services.Configure<MvcOptions>(options => options.Filters.Add<PageContextActionFilter>());

        services.AddDisplayResolutions();
        services.AddDetection();

        return services;
    }

    private static void AddDisplayResolutions(this IServiceCollection services)
    {
        services.AddSingleton<StandardResolution>();
        services.AddSingleton<IpadHorizontalResolution>();
        services.AddSingleton<IphoneVerticalResolution>();
        services.AddSingleton<AndroidVerticalResolution>();
    }
}
