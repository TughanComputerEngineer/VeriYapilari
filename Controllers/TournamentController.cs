using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly DataContext _context;

        public TournamentController(DataContext context)
        {
            _context = context;
        }

        // Get: api/tournament/teams
        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _context.Teams.ToListAsync();
            return Ok(teams);
        }
    }
}