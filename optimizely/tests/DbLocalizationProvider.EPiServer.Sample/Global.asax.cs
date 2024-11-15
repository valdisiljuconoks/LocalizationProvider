using System.Web.Mvc;
using EPiServer;
using EPiServer.Logging;
using EPiServer.Logging.Log4Net;
using log4net.Config;

[assembly: LoggerFactory(typeof(Log4NetLoggerFactory))]

namespace DbLocalizationProvider.EPiServer.Sample
{
    public class EPiServerApplication : Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            XmlConfigurator.Configure();
            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }
    }
}
