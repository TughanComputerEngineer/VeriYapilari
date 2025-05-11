using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context; // DataContext'i constructor üzerinden inject ettik
        }

        public IActionResult Index()
        {
            return View(); // Admin sayfası için view'ı döndürüyoruz
        }
        [HttpPost("delete-team")]
        public IActionResult DeleteTeam([FromBody] TeamDeleteRequest request)
        {
            // ID'yi alıyoruz
            var teamToDelete = _context.Teams.FirstOrDefault(t => t.Id == request.Id);
            
            if (teamToDelete == null)
            {
                return NotFound(new { message = "Takım bulunamadı." });
            }
            
            // Takımı silme işlemi
            _context.Teams.Remove(teamToDelete);
            _context.SaveChanges(); // Değişiklikleri kaydet
            
            return Ok(new { success = true, message = "Takım başarıyla silindi." });
        }         
        [HttpPost("add-team")]
        public IActionResult AddTeam([FromBody] TeamAddRequest request)
        {
            // Gelen takım adını kontrol et
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { message = "Takım adı zorunludur!" });
            }
            // Aynı adı taşıyan bir takımın var olup olmadığını kontrol et
            var existingTeam = _context.Teams.FirstOrDefault(t => t.Name.ToLower() == request.Name.ToLower());
            if (existingTeam != null)
            {
                return BadRequest(new { message = "Bu takım zaten mevcut!" });
            }

             // Takım adı dosya adı olarak kullanılacağından güvenli hale getiriyoruz
            var sanitizedTeamName = request.Name.Replace(" ", "_").ToLowerInvariant();
            string logoFileName;

            // Eğer logo gelmediyse ya da placeholder geldi, o zaman özel logo kaydediyoruz
            if (string.IsNullOrWhiteSpace(request.Logo) || request.Logo.EndsWith("placeholder.png"))
            {
                var wwwRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                // Takım adıyla dosya ismi oluşturulacak
                var destPath = Path.Combine(wwwRoot, "images", "team_logos", $"{sanitizedTeamName}.png");

                try
                {
                    // Eğer placeholder'ı kullanmıyorsak, burada direkt takıma ait logo dosyasını kaydediyoruz
                    // Örneğin, burada "takım_adı.png" olarak kaydedeceğiz
                    // Boş dosya kaydedilebilir, sadece logonun resmini sonradan yükleyebilirsiniz
                    System.IO.File.WriteAllBytes(destPath, new byte[] { }); // Boş bir dosya kaydediyoruz

                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Logo dosyası kaydedilirken hata oluştu.", error = ex.Message });
                }

                logoFileName = $"/images/team_logos/{sanitizedTeamName}.png";  // Frontend'deki yol
            }
            else
            {
                // Kullanıcı özel logo gönderdi → olduğu gibi kullan
                logoFileName = request.Logo;
            }

            // Yeni takımı oluşturuyoruz
            var newTeam = new Team
            {
                Name = request.Name,
                CurrentScore = 0, // Varsayılan puan değeri
                Last5Match = new List<string>(), // Başlangıçta boş bir liste
                LogoPath = logoFileName
            };

            // Takımı veritabanına ekliyoruz
            _context.Teams.Add(newTeam);
            _context.SaveChanges(); // Veritabanına kaydediyoruz

            return Ok(new { success = true });
        }

        [HttpPost("add-match")]
        public IActionResult AddMatch([FromBody] MatchAddRequest request)
        {
            if (request.Team1Id == request.Team2Id)
            {
                return BadRequest(new { message = "İki takım farklı olmalıdır!" });
            }

            var team1 = _context.Teams.Find(request.Team1Id);
            var team2 = _context.Teams.Find(request.Team2Id);

            if (team1 == null || team2 == null)
            {
                return NotFound(new { message = "Takımlardan biri bulunamadı." });
            }

            var match = new Match
            {
                Team1Id = request.Team1Id,
                Team2Id = request.Team2Id,
                Score1 = request.Score1,
                Score2 = request.Score2
            };

            _context.Matches.Add(match);
            _context.SaveChanges();

            return Ok(new { success = true });
        }
        [HttpPost("delete-match")]
        public IActionResult DeleteMatch([FromBody] DeleteMatchRequest request)
        {
            var match = _context.Matches.FirstOrDefault(m => m.Id == request.Id);
            if (match == null)
            {
                return NotFound(new { message = "Maç bulunamadı." });
            }

            _context.Matches.Remove(match);
            _context.SaveChanges();

            return Ok(new { success = true });
        }
   
        public class TeamDeleteRequest
        {
            public int Id { get; set; }
        }
        public class TeamAddRequest
        {
            public required string Name {get; set;}
            public string? Logo {get; set;}
        }

        public class MatchAddRequest
        {
            public int Team1Id { get; set; }
            public int Team2Id { get; set; }
            public int Score1 { get; set; }
            public int Score2 { get; set; }
        }
        public class DeleteMatchRequest
        {
            public int Id { get; set; }
        }
    }
}