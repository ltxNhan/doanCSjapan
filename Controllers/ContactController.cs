using Microsoft.AspNetCore.Mvc;

namespace JapanApp.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(
            string fullName,
            string email,
            string departure,
            string destination,
            string departureDate,
            string returnDate,
            string festivalName,
            string message)
        {
            TempData["Success"] = "Cảm ơn bạn đã gửi yêu cầu tư vấn đặt vé. Chúng tôi sẽ liên hệ lại sớm nhất có thể.";
            return RedirectToAction("Index");
        }
    }
}