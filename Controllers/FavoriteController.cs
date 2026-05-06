using JapanApp.Data;
using JapanApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JapanApp.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserID");

            List<Festival> festivals;

            // Nếu đã đăng nhập: lấy từ database Favorites
            if (userIdStr != null)
            {
                int userId = int.Parse(userIdStr);

                festivals = _context.Favorites
                    .Include(f => f.Festival)
                    .Where(f => f.UserID == userId)
                    .Select(f => f.Festival)
                    .ToList();
            }
            else
            {
                // Nếu chưa đăng nhập: lấy từ Session
                var guestFavoriteJson = HttpContext.Session.GetString("GuestFavorites");

                var favoriteIds = string.IsNullOrEmpty(guestFavoriteJson)
                    ? new List<int>()
                    : JsonSerializer.Deserialize<List<int>>(guestFavoriteJson) ?? new List<int>();

                festivals = _context.Festivals
                    .Where(f => favoriteIds.Contains(f.FestivalID))
                    .ToList();
            }

            return View(festivals);
        }

        public IActionResult Remove(int festivalId)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");

            // Nếu đã đăng nhập: xóa khỏi database
            if (userIdStr != null)
            {
                int userId = int.Parse(userIdStr);

                var favorite = _context.Favorites
                    .FirstOrDefault(f => f.UserID == userId && f.FestivalID == festivalId);

                if (favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Nếu chưa đăng nhập: xóa khỏi Session
            var guestFavoriteJson = HttpContext.Session.GetString("GuestFavorites");

            var favoriteIds = string.IsNullOrEmpty(guestFavoriteJson)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(guestFavoriteJson) ?? new List<int>();

            if (favoriteIds.Contains(festivalId))
            {
                favoriteIds.Remove(festivalId);
            }

            HttpContext.Session.SetString("GuestFavorites", JsonSerializer.Serialize(favoriteIds));

            return RedirectToAction("Index");
        }
    }
}