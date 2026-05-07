using Microsoft.AspNetCore.Mvc;
using JapanApp.Services;

namespace JapanApp.Controllers
{
    public class QuizController : Controller
    {
        private readonly FestivalService _service;

        public QuizController(FestivalService service)
        {
            _service = service;
        }

        // 🔒 CHƯA LOGIN -> BẮT LOGIN
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập";

                return RedirectToAction("Login", "Account");
            }

            var questions = _service.GetQuizQuestions();

            return View(questions);
        }

        [HttpPost]
        public IActionResult Submit(List<int> answerIds)
        {
            // 🔒 CHECK LOGIN
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập";

                return RedirectToAction("Login", "Account");
            }

            int seasonId = _service.GetSuggestedSeason(answerIds);

            var festivals = _service.GetBySeason(seasonId);

            return View("Result", festivals);
        }
    }
}