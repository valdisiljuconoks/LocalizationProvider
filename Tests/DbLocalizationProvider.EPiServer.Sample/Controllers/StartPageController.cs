using System.Web.Mvc;
using DbLocalizationProvider.EPiServer.Sample.Models.Pages;
using EPiServer.Web.Mvc;

namespace DbLocalizationProvider.EPiServer.Sample.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        public ActionResult Index(StartPage currentPage)
        {
            return View(currentPage);
        }
    }
}
