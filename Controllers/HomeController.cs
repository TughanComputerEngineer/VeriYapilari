using Microsoft.AspNetCore.Mvc;
using webapplication.Models; // <-- DataContext burada tanımlı olmalı

namespace webapplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var teams = _context.Teams.ToList(); // SQL'den takımlar çekiliyor
            return View(teams); // View'a gönderiliyor
        }
    }
}
