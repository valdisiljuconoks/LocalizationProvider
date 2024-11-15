using Microsoft.AspNetCore.Mvc;

namespace AlloySampleSite.Controllers
{
    public class TestController : Controller
    {
        public IActionResult TestView()
        {
            return View();
        }
    }
}
