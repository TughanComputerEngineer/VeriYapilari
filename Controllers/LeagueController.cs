using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
            // Veritaban�ndan tak�mlar� �ek
            var teams = _context.Teams.ToList();

            // Null kontrol� yap
            if (teams == null)
            {
                teams = new List<Team>();
            }

            // Modeli view'a g�nder
            return View(teams);
        }

        // Tak�m verilerini JSON olarak d�ner
        [HttpGet]
        [Route("api/tournament/teams")]
        public IActionResult GetTeams()
        {
            var teams = _context.Teams
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.CurrentScore,
                    t.Last5Match,
                    LogoPath = $"/images/{t.Name.Replace(" ", "_").ToLower()}.png"
                })
                .ToList();

            return Json(teams);
        }
    }
}
