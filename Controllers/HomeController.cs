using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Session için gerekli
using webapplication.Models;
using Microsoft.AspNetCore.Mvc;

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
            HttpContext.Session.SetString("username", "denemekullanici2");

            var teams = _context.Teams.ToList(); // Tüm takımlar

            // Giriş yapmış kullanıcı varsa onu bul
            var username = HttpContext.Session.GetString("username");

            List<Team> favoriteTeams = new();

            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null && user.Favourites != null)
                {
                    favoriteTeams = _context.Teams
                        .Where(t => user.Favourites.Contains(t.Id))
                        .ToList();
                }
            }

            ViewBag.Favorites = favoriteTeams;
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(username);

            

            return View(teams);
        }

        // Favori takımlarını eklemek/çıkarmak için endpoint
        [HttpPost]
        public IActionResult ToggleFavorite(int teamId)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(); // Eğer giriş yapılmamışsa
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(); // Kullanıcı bulunamadıysa
            }

            if (user.Favourites == null)
            {
                user.Favourites = new List<int>();
            }

            if (user.Favourites.Contains(teamId))
            {
                user.Favourites.Remove(teamId); // Eğer favorideyse çıkar
            }
            else
            {
                user.Favourites.Add(teamId); // Eğer favoride değilse ekle
            }

            _context.SaveChanges(); // Değişiklikleri kaydet
            return Ok();
        }
    }
}
