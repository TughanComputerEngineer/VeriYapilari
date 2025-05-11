using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

namespace webapplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TakimlarController : ControllerBase
    {
        private readonly DataContext _context;

        public TakimlarController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var siraliTakimlar = _context.Teams
                .OrderByDescending(t => t.CurrentScore)
                .ToList();

            var jsonList = siraliTakimlar.Select((team, index) => new
            {
                id = team.Id,
                isim = team.Name,
                puan = team.CurrentScore,
                sira = index + 1,
                son5mac = team.Last5Match,
                logo = $"images/{team.Name.ToLower().Replace(" ", "")}.png"
            });

            return Ok(jsonList);
        }
    }
}
