using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace DbLocalizationProvider.AdminUI
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseDbLocalizationProviderAdminUI(this IAppBuilder builder, Action<UiConfigurationContext> setup = null)
        {
            setup?.Invoke(UiConfigurationContext.Current);

            builder.UseFileServer(new FileServerOptions
                                  {
                                      EnableDefaultFiles = true,
                                      DefaultFilesOptions =
                                      {
                                          DefaultFileNames = new[] { "index.html" }
                                      },
                                      FileSystem = new EmbeddedResourceFileSystem(typeof(AppBuilderExtensions).Assembly, "DbLocalizationProvider.AdminUI")
                                  });

            builder.UseFileServer(new FileServerOptions
                                  {
                                      RequestPath = new PathString("/res"),
                                      FileSystem = new EmbeddedResourceFileSystem(typeof(AppBuilderExtensions).Assembly, "DbLocalizationProvider.AdminUI.ClientResources")
                                  });

            builder.UseStageMarker(PipelineStage.MapHandler);
            var config = new HttpConfiguration();

            // explicitly registering required routes in order to avoid double callss for register attribute based routes
            config.Routes.MapHttpRoute("resources-get", "api/get", defaults: new { controller = "ResourcesApi", action = "Get" });
            config.Routes.MapHttpRoute("resources-update", "api/update", defaults: new { controller = "ResourcesApi", action = "Update" });

            builder.UseWebApi(config);

            return builder;
        }
    }
}
