using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using webapplication.Models;

namespace webapplication.Controllers
{
    public class LeagueController : Controller
    {
        private readonly DataContext _context;

        public LeagueController(DataContext context)
        {
            _context = context;
        }

        // Ana sayfa
        public IActionResult Index()
        {
            // Takımları ve puanlarını al
            var teams = _context.Teams.OrderByDescending(t => t.CurrentScore).ToList();

            // En çok puana sahip takımı bul
            var topTeam = teams.OrderByDescending(t => t.CurrentScore).FirstOrDefault();

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

        // Takım verilerini JSON olarak döner
        [HttpGet]
        [Route("api/tournament/teams")]
        public IActionResult GetTeams()
        {
            var teams = _context.Teams
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.CurrentLeague,
                    t.CurrentScore,
                    t.Last5Match,
                    LogoPath = $"/images/{t.Name.Replace(" ", "_").ToLower()}.png"
                })
                .ToList();

            return Json(teams);
        }

        // Favorilere ekle/çkar
        [HttpPost]
        public IActionResult ToggleFavorite(int teamId)
        {
            // Kullanıcı giriş yapmış mı kontrol et
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcı ID'sini al
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }
            
            int userId = int.Parse(userIdClaim);

            // Kullanıcıyı veritabanından bul
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Takım ID'si geçerli mi kontrol et
            var team = _context.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            // Favorilere ekle veya çkar
            if (user.Favourites.Contains(teamId))
            {
                user.Favourites.Remove(teamId);
            }
            else
            {
                user.Favourites.Add(teamId);
            }

            // Değişiklikleri kaydet
            _context.SaveChanges();

            // Referrer URL'e geri dön veya ana sayfaya yönlendir
            string referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }

        // Kullanıcının favori takımlarını al
        private List<int> GetUserFavorites()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return new List<int>();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return new List<int>();
            }
            
            int userId = int.Parse(userIdClaim);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || user.Favourites == null)
            {
                return new List<int>();
            }

            return user.Favourites;
        }
    }
}
