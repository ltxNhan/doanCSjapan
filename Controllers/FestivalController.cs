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

        // 📌 DANH SÁCH
        public IActionResult Index(string keyword, int? seasonId, int? regionId)
        {
            var data = _service.Search(keyword, seasonId, regionId);

            return View(data);
        }

        // 📌 CHI TIẾT
        public IActionResult Details(int id)
        {
            // 🔒 CHƯA LOGIN -> KHÔNG CHO XEM
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập";

                return RedirectToAction("Login", "Account");
            }

            var festival = _service.GetById(id);

            if (festival == null)
            {
                return NotFound();
            }

            return View(festival);
        }

        // ❤️ FAVORITE
        public IActionResult AddFavorite(int id)
        {
            // 🔒 CHƯA LOGIN
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập";

                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserID"));

            _service.AddFavorite(userId, id);

            return RedirectToAction("Index", "Favorite");
        }

        // ⭐ REVIEW
        [HttpPost]
        public IActionResult AddReview(int festivalId, int rating, string comment)
        {
            // 🔒 CHƯA LOGIN
            if (HttpContext.Session.GetString("Username") == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập";

                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(HttpContext.Session.GetString("UserID"));

            _service.AddReview(userId, festivalId, rating, comment);

            return RedirectToAction("Details", new { id = festivalId });
        }

        // ================= ADMIN =================

        // CREATE
        public IActionResult Create()
        {
            // 🔒 CHỈ ADMIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(JapanApp.Models.Festival model)
        {
            // 🔒 CHỈ ADMIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.CreateFestival(model);

            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            // 🔒 CHỈ ADMIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            var festival = _service.GetById(id);

            return View(festival);
        }

        [HttpPost]
        public IActionResult Edit(JapanApp.Models.Festival model)
        {
            // 🔒 CHỈ ADMIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.UpdateFestival(model);

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            // 🔒 CHỈ ADMIN
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Index");
            }

            _service.DeleteFestival(id);

            return RedirectToAction("Index");
        }
    }
}