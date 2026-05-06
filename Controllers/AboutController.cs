using Microsoft.AspNetCore.Mvc;

namespace JapanApp.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}