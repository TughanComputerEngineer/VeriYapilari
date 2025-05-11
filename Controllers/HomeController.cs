using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

namespace webapplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchQuery)
        {
            var teams = _context.Teams.ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                teams = teams.Where(t => t.Name.ToLower().Contains(searchQuery.ToLower())).ToList();
            }

            return View(teams);
        }
    }
}
