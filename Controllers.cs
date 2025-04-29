using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamController(DataContext context)
        {
            _context = context;
        }

        // POST api/team
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam([FromBody] Team team)
        {
            // Veritabanına ekleme
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // GET api/team/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }
    }
}
