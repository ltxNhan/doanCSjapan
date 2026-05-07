using JapanApp.Data;
using JapanApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace JapanApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Username == username &&
                    u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserID", user.UserID);

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Festival");
            }

            return RedirectToAction("Index", "Home");
        }

        // ================= REGISTER =================

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {
            var checkUser = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (checkUser != null)
            {
                ViewBag.Error = "Tài khoản đã tồn tại";
                return View();
            }

            model.Role = "User";

            _context.Users.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}