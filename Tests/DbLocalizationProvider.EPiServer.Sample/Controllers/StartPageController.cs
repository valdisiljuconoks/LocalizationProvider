using System.Web.Mvc;
using DbLocalizationProvider.EPiServer.Sample.Models.Pages;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.Web.Mvc;

namespace DbLocalizationProvider.EPiServer.Sample.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        public ActionResult Index(StartPage currentPage)
        {
            LogManager.GetLogger(typeof(StartPageController)).Log(Level.Information, "Test log message");

            LocalizationService.Current.GetString("/asdfasdf/asdfasdf");

            return View(currentPage);
        }
    }
}
