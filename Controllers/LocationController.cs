using JapanApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JapanApp.Controllers
{
    public class LocationController : Controller
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var regions = _context.Regions
                .Include(r => r.Festivals)
                .ToList();

            return View(regions);
        }
    }
}