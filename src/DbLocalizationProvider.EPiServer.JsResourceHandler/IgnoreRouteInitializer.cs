using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    [InitializableModule]
    public class IgnoreRouteInitializer : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.IgnoreRoute(Constants.IgnoreRoute);
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}
