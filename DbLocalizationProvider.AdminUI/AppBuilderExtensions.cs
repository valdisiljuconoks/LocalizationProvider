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
        public static void UseDbLocalizationProviderAdminUI(this IAppBuilder builder, Action<UiConfigurationContext> setup = null)
        {
            // set default implementations

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
            config.Routes.MapHttpRoute("testroute", "api", defaults: new { controller = "ResourcesApi", action = "Get" });

            builder.UseWebApi(config);
        }
    }
}
