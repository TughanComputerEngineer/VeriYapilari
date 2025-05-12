using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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

        public IActionResult Index()
        {
            // Takımları al ve puana göre sırala
            var teams = _context.Teams.OrderByDescending(t => t.CurrentScore).ToList();
            
            // Kullanıcının favori takımını bul
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                // Favori takımları ViewBag'e at
                ViewBag.FavoriteTeams = user?.Favourites ?? new List<int>();
            }
            else
            {
                ViewBag.FavoriteTeams = new List<int>();
            }

            return View(teams);
        }

        public IActionResult Fixtures()
        {
            // Tüm maçları tarih sırasına göre getir
            var matches = _context.Matches
                .OrderBy(m => m.MatchDate)
                .ToList();

            // Sonraki planlanan maçları (status = 0) öncelikli olarak göster
            var upcomingMatches = matches
                .Where(m => m.Status == 0) // Planlanmış maçlar
                .OrderBy(m => m.MatchDate)
                .ToList();

            // Tamamlanmış maçları (status = 2) sonra göster
            var completedMatches = matches
                .Where(m => m.Status == 2) // Tamamlanmış maçlar
                .OrderByDescending(m => m.MatchDate)
                .ToList();

            // Devam eden maçları (status = 1) göster
            var liveMatches = matches
                .Where(m => m.Status == 1) // Devam eden maçlar
                .OrderBy(m => m.MatchDate)
                .ToList();

            ViewBag.UpcomingMatches = upcomingMatches;
            ViewBag.CompletedMatches = completedMatches;
            ViewBag.LiveMatches = liveMatches;

            return View();
        }
    }
}
