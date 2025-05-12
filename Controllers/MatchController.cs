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
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            HomeScore = m.HomeScore,
            AwayScore = m.AwayScore,
            m.MatchDate,
            m.Location,
            StatusText = GetStatusText(m.Status)
        });

        return Ok(result);
    }

    // Maç durumunu metin olarak döndürür
    private string GetStatusText(int status)
    {
        return status switch
        {
            0 => "Planlandı",
            1 => "Devam Ediyor",
            2 => "Tamamlandı",
            3 => "İptal Edildi",
            _ => "Bilinmiyor"
        };
    }
}