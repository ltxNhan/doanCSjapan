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

        [HttpPost]
        public IActionResult Submit(List<int> answerIds)
        {
            int seasonId = _service.GetSuggestedSeason(answerIds);
            var festivals = _service.GetBySeason(seasonId);

            return View("Result", festivals);
        }
    }
}