using Microsoft.AspNetCore.Mvc;
using JapanApp.Services;
using System.Text.Json;

namespace JapanApp.Controllers
{
    public class FestivalController : Controller
    {
        private readonly FestivalService _service;

        public FestivalController(FestivalService service)
        {
            _service = service;
        }

        // 📌 LIST + SEARCH + FILTER
        public IActionResult Index(string keyword, int? seasonId, int? regionId)
        {
            var data = _service.Search(keyword, seasonId, regionId);

            ViewBag.Keyword = keyword;
            ViewBag.SeasonId = seasonId;
            ViewBag.RegionId = regionId;
            ViewBag.FilterTitle = GetFilterTitle(seasonId, regionId);

            return View(data);
        }

        // 📌 TIÊU ĐỀ THEO BỘ LỌC
        private string GetFilterTitle(int? seasonId, int? regionId)
        {
            if (seasonId.HasValue)
            {
                return seasonId.Value switch
                {
                    1 => "🌸 Lễ hội mùa xuân",
                    2 => "🎆 Lễ hội mùa hè",
                    3 => "🍁 Lễ hội mùa thu",
                    4 => "❄️ Lễ hội mùa đông",
                    _ => "🎌 Explore Japan Festivals"
                };
            }

            if (regionId.HasValue)
            {
                return "📍 Lễ hội theo địa điểm";
            }

            return "🎌 Explore Japan Festivals";
        }

        // 📌 DETAILS
        public IActionResult Details(int id)
        {
            var festival = _service.GetById(id);

            if (festival == null)
            {
                return NotFound();
            }

            return View(festival);
        }

        // ❤️ FAVORITE - Không cần đăng nhập vẫn theo dõi được
        public IActionResult AddFavorite(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");

            // Nếu đã đăng nhập thì lưu vào database
            if (userIdStr != null)
            {
                int userId = int.Parse(userIdStr);
                _service.AddFavorite(userId, id);

                return RedirectToAction("Index", "Favorite");
            }

            // Nếu chưa đăng nhập thì lưu tạm bằng Session
            var guestFavoriteJson = HttpContext.Session.GetString("GuestFavorites");

            List<int> guestFavorites;

            if (string.IsNullOrEmpty(guestFavoriteJson))
            {
                guestFavorites = new List<int>();
            }
            else
            {
                guestFavorites = JsonSerializer.Deserialize<List<int>>(guestFavoriteJson) ?? new List<int>();
            }

            if (!guestFavorites.Contains(id))
            {
                guestFavorites.Add(id);
            }

            HttpContext.Session.SetString("GuestFavorites", JsonSerializer.Serialize(guestFavorites));

            return RedirectToAction("Index", "Favorite");
        }

        // ⭐ REVIEW
        [HttpPost]
        public IActionResult AddReview(int festivalId, int rating, string comment)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");

            if (userIdStr == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);

            _service.AddReview(userId, festivalId, rating, comment);

            return RedirectToAction("Details", new { id = festivalId });
        }

        // 🔥 ADMIN: CREATE
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(JapanApp.Models.Festival model)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.CreateFestival(model);
            return RedirectToAction("Index");
        }

        // 🔥 ADMIN: EDIT
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            var festival = _service.GetById(id);

            if (festival == null)
            {
                return NotFound();
            }

            return View(festival);
        }

        [HttpPost]
        public IActionResult Edit(JapanApp.Models.Festival model)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.UpdateFestival(model);
            return RedirectToAction("Index");
        }

        // 🔥 ADMIN: DELETE
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.DeleteFestival(id);
            return RedirectToAction("Index");
        }
    }
}