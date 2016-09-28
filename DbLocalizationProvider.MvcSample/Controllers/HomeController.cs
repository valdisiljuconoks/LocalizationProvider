using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using DbLocalizationProvider.MvcSample.Models;

namespace DbLocalizationProvider.MvcSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string l)
        {
            if (!string.IsNullOrEmpty(l))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(l);
            }

            return View(new HomeViewModel());
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            return View(new HomeViewModel());
        }
    }
}
