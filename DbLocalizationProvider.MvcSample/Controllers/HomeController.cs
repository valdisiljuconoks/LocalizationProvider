using System.Web.Mvc;
using DbLocalizationProvider.MvcSample.Models;

namespace DbLocalizationProvider.MvcSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
