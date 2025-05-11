using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

[Route("api/[controller]")]
[ApiController]
public class MatchController : ControllerBase
{
    private readonly DataContext _context;

    public MatchController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetMatches()
    {
        // Veritabanından tüm maçları al
        var matches = _context.Matches.ToList();

        // Tüm takımları alıp ID'lerine göre sözlük haline getir
        var teams = _context.Teams
            .ToDictionary(t => t.Id, t => t.Name);

        // Maçlara göre takımları eşleştir
        var result = matches.Select(m => new
        {
            m.Id,
            Team1Name = teams.ContainsKey(m.Team1Id) ? teams[m.Team1Id] : "Bilinmiyor",
            Team2Name = teams.ContainsKey(m.Team2Id) ? teams[m.Team2Id] : "Bilinmiyor",
            m.Score1,
            m.Score2
        });

        return Ok(result);
    }
}