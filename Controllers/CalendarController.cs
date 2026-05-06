using JapanApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapanApp.Controllers
{
    public class CalendarController : Controller
    {
        private readonly AppDbContext _context;

        public CalendarController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var festivals = _context.Festivals
                .Include(f => f.Region)
                .Include(f => f.Season)
                .ToList();

            return View(festivals);
        }
    }
}