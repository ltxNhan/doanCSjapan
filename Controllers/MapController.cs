using JapanApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapanApp.Controllers
{
    public class MapController : Controller
    {
        private readonly AppDbContext _context;

        public MapController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var festivals = _context.Festivals
                .Include(f => f.Region)
                .Include(f => f.Season)
                .Where(f => f.Latitude != 0 && f.Longitude != 0)
                .ToList();

            return View(festivals);
        }
    }
}