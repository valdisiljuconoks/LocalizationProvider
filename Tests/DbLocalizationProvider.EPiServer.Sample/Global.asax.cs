using System.Web.Mvc;
using EPiServer;

namespace DbLocalizationProvider.EPiServer.Sample
{
    public class EPiServerApplication : Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }
    }
}
