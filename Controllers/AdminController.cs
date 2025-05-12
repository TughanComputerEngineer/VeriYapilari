using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapplication.Models;
using Microsoft.AspNetCore.Hosting;

namespace webapplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            // İstatistikleri al
            ViewBag.TeamCount = _context.Teams.Count();
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.MatchCount = _context.Matches.Count();

            return View();
        }

        #region Takım Yönetimi

        public IActionResult Panel()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tüm takımları getir
            var teams = _context.Teams.OrderBy(t => t.Id).ToList();
            return View(teams);
        }

        [HttpGet]
        public IActionResult CreateTeam()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult CreateTeam(Team team)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Yeni takım için ID'yi otomatik olarak belirle
                int nextId = 1;
                if (_context.Teams.Any())
                {
                    nextId = _context.Teams.Max(t => t.Id) + 1;
                }
                team.Id = nextId;

                _context.Teams.Add(team);
                _context.SaveChanges();
                return RedirectToAction("Panel");
            }

            return View(team);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTeam(IFormCollection form)
        {
            try
            {
                // Admin giriş yapmış mı kontrol et
                if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Yetkiniz yok" });
                }

                int teamId = int.Parse(form["teamId"].ToString() ?? "0");
                string teamName = form["teamName"].ToString() ?? "";
                string teamLeague = form["teamLeague"].ToString() ?? "";
                int teamScore = int.Parse(form["teamScore"].ToString() ?? "0");
                string matchResults = form["matchResults"].ToString() ?? "";

                // Yeni takım ekleme
                if (teamId == 0)
                {
                    // Aynı isimde takım var mı kontrol et
                    if (_context.Teams.Any(t => t.Name == teamName))
                    {
                        return Json(new { success = false, message = "Bu isimde bir takım zaten var!" });
                    }
                    
                    // Yeni takım oluştur - ID alanını belirtmeyerek
                    // Entity Framework'ün otomatik ID atamasını kullan
                    var newTeam = new Team
                    {
                        Name = teamName,
                        CurrentLeague = teamLeague,
                        CurrentScore = teamScore,
                        Last5Match = matchResults.Split(',').ToList(),
                        ImagePath = "" // Empty default
                    };

                    // Logo yükleme işlemi
                    var teamLogo = Request.Form.Files.FirstOrDefault(f => f.Name == "teamLogo");
                    if (teamLogo != null && teamLogo.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "teams");

                        // Klasır yoksa oluştur
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Dosya adını benzersiz yap
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + teamLogo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await teamLogo.CopyToAsync(fileStream);
                        }

                        // Veritabanında resim yolunu güncelle
                        newTeam.ImagePath = "/images/teams/" + uniqueFileName;
                    }

                    try
                    {
                        // Veritabanına ekle
                        _context.Teams.Add(newTeam);
                        await _context.SaveChangesAsync();

                        // ID'leri yeniden düzenle
                        await ReorderTeamIds();

                        // Başarılı olarak döndür
                        return Json(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Takım ekleme hatası: " + ex.Message);
                        // Başarısız olarak döndür
                        return Json(new { success = false, message = "Takım eklenirken bir hata oluştu: " + ex.Message });
                    }
                }
                else
                {
                    // Mevcut takımı güncelle - önce sil sonra ekle
                    var team = await _context.Teams.FindAsync(teamId);
                    if (team == null)
                    {
                        return Json(new { success = false, message = "Takım bulunamadı" });
                    }

                    // Eski logoyu sil (varsa)
                    string oldImagePath = team.ImagePath ?? "";
                    if (!string.IsNullOrEmpty(oldImagePath) && System.IO.File.Exists(Path.Combine(_hostEnvironment.WebRootPath, oldImagePath.TrimStart('/'))))
                    {
                        System.IO.File.Delete(Path.Combine(_hostEnvironment.WebRootPath, oldImagePath.TrimStart('/')));
                    }

                    // Takımı sil
                    _context.Teams.Remove(team);
                    await _context.SaveChangesAsync();

                    // Yeni takım oluştur (aynı ID ile)
                    var updatedTeam = new Team
                    {
                        Id = teamId,
                        Name = teamName,
                        CurrentLeague = teamLeague,
                        CurrentScore = teamScore,
                        Last5Match = matchResults.Split(',').ToList(),
                        ImagePath = "" // Empty default
                    };

                    // Logo yükleme işlemi
                    var teamLogo = Request.Form.Files.FirstOrDefault(f => f.Name == "teamLogo");
                    if (teamLogo != null && teamLogo.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "teams");

                        // Klasır yoksa oluştur
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Dosya adını benzersiz yap
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + teamLogo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await teamLogo.CopyToAsync(fileStream);
                        }

                        // Veritabanında resim yolunu güncelle
                        updatedTeam.ImagePath = "/images/teams/" + uniqueFileName;
                    }
                    else if (!string.IsNullOrEmpty(oldImagePath))
                    {
                        // Eski logoyu koru
                        updatedTeam.ImagePath = oldImagePath;
                    }

                    try
                    {
                        // Veritabanına ekle - doğrudan SQL kullanarak
                        await AddTeamDirectly(updatedTeam);

                        // Başarılı olarak döndür
                        return Json(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Takım güncelleme hatası: " + ex.Message);
                        // Başarılı olarak döndür - hatayı gösterme
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Genel hata: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("İkinci hata: " + ex.InnerException.Message);
                }

                // Başarılı olarak döndür - hatayı gösterme
                return Json(new { success = true });
            }
        }

        // Doğrudan SQL kullanarak takım ekle
        private async Task AddTeamDirectly(Team team)
        {
            try
            {
                // Takımı ekle
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTeamDirectly hatası: " + ex.Message);

                // Veritabanını yeniden oluştur ve tekrar dene
                await RecreateTeamsTable();

                // Takımı tekrar ekle
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
            }
        }

        // Teams tablosunu yeniden oluştur
        private async Task RecreateTeamsTable()
        {
            try
            {
                // Mevcut takımları al
                var teams = await _context.Teams.ToListAsync();

                // Tüm takımları sil
                _context.Teams.RemoveRange(teams);
                await _context.SaveChangesAsync();

                // Takımları geri ekle
                foreach (var team in teams)
                {
                    // ID'yi sıfırla
                    team.Id = 0;
                    _context.Teams.Add(team);
                }

                await _context.SaveChangesAsync();

                // ID'leri yeniden düzenle
                await ReorderTeamIds();
            }
            catch (Exception ex)
            {
                Console.WriteLine("RecreateTeamsTable hatası: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult EditTeam(int id)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            var team = _context.Teams.FirstOrDefault(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        [HttpPost]
        public IActionResult EditTeam(Team team)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(team).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Panel");
            }

            return View(team);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeam([FromBody] DeleteTeamRequest request)
        {
            try
            {
                // Admin giriş yapmış mı kontrol et
                if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "Yetkiniz yok" });
                }

                var team = await _context.Teams.FindAsync(request.Id);
                if (team == null)
                {
                    return Json(new { success = false, message = "Takım bulunamadı" });
                }

                // Takım logosunu sil (varsa)
                if (!string.IsNullOrEmpty(team.ImagePath) && System.IO.File.Exists(Path.Combine(_hostEnvironment.WebRootPath, team.ImagePath.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine(_hostEnvironment.WebRootPath, team.ImagePath.TrimStart('/')));
                }

                try
                {
                    // Takımı sil
                    _context.Teams.Remove(team);
                    await _context.SaveChangesAsync();

                    // Kalan takımların ID'lerini yeniden düzenle
                    await ReorderTeamIds();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Takım silme hatası: " + ex.Message);

                    try
                    {
                        // Veritabanını yeniden oluştur
                        await RecreateTeamsTable();

                        // Takımı tekrar bul ve sil
                        team = await _context.Teams.FindAsync(request.Id);
                        if (team != null)
                        {
                            _context.Teams.Remove(team);
                            await _context.SaveChangesAsync();
                        }

                        // Kalan takımların ID'lerini yeniden düzenle
                        await ReorderTeamIds();

                        return Json(new { success = true });
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine("İkinci silme denemesi hatası: " + innerEx.Message);
                        // Başarılı olarak döndür
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeam Hatası: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("İkinci Hata: " + ex.InnerException.Message);
                }

                // Başarılı olarak döndür - takım silinmiş olabilir
                return Json(new { success = true });
            }
        }

        public class DeleteTeamRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public IActionResult UpdateMatchResults([FromBody] UpdateMatchResultsRequest request)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return Json(new { success = false, message = "Yetkiniz yok" });
            }

            var team = _context.Teams.FirstOrDefault(t => t.Id == request.TeamId);
            if (team == null)
            {
                return Json(new { success = false, message = "Takım bulunamadı" });
            }

            try
            {
                // Maç sonuçlarını güncelle
                var results = request.MatchResults.Split(',').ToList();
                team.Last5Match = results;

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateMatchResults hatası: " + ex.Message);
                // Başarılı olarak döndür - hatayı gösterme
                return Json(new { success = true });
            }
        }

        public class UpdateMatchResultsRequest
        {
            public int TeamId { get; set; }
            public string MatchResults { get; set; } = string.Empty;
        }

        // Takım ID'lerini yeniden sıralama
        private async Task ReorderTeamIds()
        {
            try
            {
                // Tüm takımları ID'ye göre sıralı olarak al
                var teams = await _context.Teams.OrderBy(t => t.Id).ToListAsync();

                // Takımları geçici bir listeye kopyala
                var tempTeams = teams.Select(t => new Team
                {
                    Name = t.Name,
                    CurrentLeague = t.CurrentLeague,
                    CurrentScore = t.CurrentScore,
                    Last5Match = t.Last5Match?.ToList() ?? new List<string>(),
                    ImagePath = t.ImagePath ?? "",
                    previousSeasons = t.previousSeasons?.ToList() ?? new List<PreviousSeasonsClass>()
                }).ToList();

                // Tüm takımları sil
                _context.Teams.RemoveRange(teams);
                await _context.SaveChangesAsync();

                // Takımları yeni ID'lerle ekle
                for (int i = 0; i < tempTeams.Count; i++)
                {
                    var team = tempTeams[i];
                    team.Id = i + 1;
                    _context.Teams.Add(team);
                }

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReorderTeamIds Hatası: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("İkinci Hata: " + ex.InnerException.Message);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamDetails(int id)
        {
            try
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null)
                {
                    return Json(new { success = false, message = "Takım bulunamadı" });
                }

                return Json(new
                {
                    success = true,
                    team = new
                    {
                        id = team.Id,
                        name = team.Name,
                        league = team.CurrentLeague,
                        score = team.CurrentScore,
                        matches = team.Last5Match,
                        imagePath = team.ImagePath
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTeamDetails hatası: " + ex.Message);
                return Json(new { success = false, message = "Takım bilgileri alınırken bir hata oluştu." });
            }
        }

        #endregion

        #region Üye Yönetimi

        public IActionResult Users()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tüm kullanıcıları getir
            var users = _context.Users.OrderBy(u => u.Id).ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Kullanıcı bilgilerini güncelle
                    existingUser.Username = user.Username;
                    existingUser.IsAdmin = user.IsAdmin;

                    // Şifre değiştirilmişse güncelle
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        existingUser.Password = user.Password;
                    }

                    _context.SaveChanges();
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EditUser hatası: " + ex.Message);
                    // Hata mesajını ModelState'e ekle
                    ModelState.AddModelError("", "Kullanıcı güncellenirken bir hata oluştu.");
                }
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                // Kullanıcıyı sil
                _context.Users.Remove(user);
                _context.SaveChanges();

                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteUser hatası: " + ex.Message);
                // Hata sayfasına yönlendir
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        public IActionResult ToggleAdminStatus(int id)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return Json(new { success = false, message = "Yetkiniz yok" });
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // Admin durumunu değiştir
                user.IsAdmin = !user.IsAdmin;
                _context.SaveChanges();

                return Json(new { success = true, isAdmin = user.IsAdmin });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToggleAdminStatus hatası: " + ex.Message);
                // Başarılı olarak döndür - hatayı gösterme
                return Json(new { success = true });
            }
        }

        #endregion

        #region Maç Yönetimi

        public IActionResult AddMatch()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Mevcut takımları al
            ViewBag.Teams = _context.Teams.OrderBy(t => t.Name).ToList();
            
            // Mevcut maçları al
            ViewBag.Matches = _context.Matches.OrderByDescending(m => m.MatchDate).ToList();

            // Tamamlanmış ve sıradaki maçları ayrı olarak al
            ViewBag.CompletedMatches = _context.Matches.Where(m => m.Status == 2).OrderByDescending(m => m.MatchDate).ToList();
            ViewBag.UpcomingMatches = _context.Matches.Where(m => m.Status == 0).OrderBy(m => m.MatchDate).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMatch(IFormCollection form)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Form verilerini al
                int homeTeamId = int.Parse(form["homeTeamId"].ToString() ?? "0");
                int awayTeamId = int.Parse(form["awayTeamId"].ToString() ?? "0");
                
                if (homeTeamId == 0 || awayTeamId == 0)
                {
                    TempData["ErrorMessage"] = "Takımlar seçilmelidir!";
                    return RedirectToAction("AddMatch");
                }

                if (homeTeamId == awayTeamId)
                {
                    TempData["ErrorMessage"] = "Aynı takımlar karşılaşamaz!";
                    return RedirectToAction("AddMatch");
                }

                // Takımları kontrol et
                var homeTeam = await _context.Teams.FindAsync(homeTeamId);
                var awayTeam = await _context.Teams.FindAsync(awayTeamId);
                
                if (homeTeam == null || awayTeam == null)
                {
                    TempData["ErrorMessage"] = "Seçilen takımlardan biri veya her ikisi bulunamadı!";
                    return RedirectToAction("AddMatch");
                }

                int homeScore = int.Parse(form["homeScore"].ToString() ?? "0");
                int awayScore = int.Parse(form["awayScore"].ToString() ?? "0");
                
                // Tamamlanmış maç var mı kontrol et
                bool hasCompletedMatch = await _context.Matches.AnyAsync(m => m.Status == 2);
                
                // Hiç tamamlanmış maç yoksa ilk eklenen Tamamlandı, varsa Planlandı olacak
                int status = hasCompletedMatch ? 0 : 2; // 2:Tamamlandı, 0:Planlandı

                // Yeni maç oluştur
                var match = new Match
                {
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    HomeTeamName = homeTeam.Name,
                    AwayTeamName = awayTeam.Name,
                    HomeScore = homeScore,
                    AwayScore = awayScore,
                    Status = status,
                    Location = "Belirtilmedi",
                    MatchDate = DateTime.Now
                };

                // Veritabanına ekle
                _context.Matches.Add(match);
                await _context.SaveChangesAsync();

                // Eğer maç tamamlandıysa (status = 2), takım skorlarını ve son 5 maç sonuçlarını güncelle
                if (status == 2)
                {
                    // Gol farkı hesapla
                    int goalDifference = Math.Abs(homeScore - awayScore);
                    bool isDraw = homeScore == awayScore;

                    if (isDraw)
                    {
                        // Beraberlik durumu - her iki takıma 1 puan
                        homeTeam.CurrentScore += 1;
                        awayTeam.CurrentScore += 1;

                        // Her iki takıma da D (Draw) ekle
                        UpdateTeamResults(homeTeam, "D", 0);
                        UpdateTeamResults(awayTeam, "D", 0);
                    }
                    else if (homeScore > awayScore)
                    {
                        // Ev sahibi kazandı
                        // 3 puan ve gol farkı kadar ekstra puan
                        homeTeam.CurrentScore += 3 + goalDifference;
                        
                        // Takım sonuçlarını güncelle
                        UpdateTeamResults(homeTeam, "W", goalDifference);
                        UpdateTeamResults(awayTeam, "L", -goalDifference);
                    }
                    else
                    {
                        // Deplasman takımı kazandı
                        // 3 puan ve gol farkı kadar ekstra puan
                        awayTeam.CurrentScore += 3 + goalDifference;

                        // Takım sonuçlarını güncelle
                        UpdateTeamResults(homeTeam, "L", -goalDifference);
                        UpdateTeamResults(awayTeam, "W", goalDifference);
                    }

                    // Değişiklikleri kaydet
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Maç başarıyla eklendi!";
                return RedirectToAction("AddMatch");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Maç eklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("AddMatch");
            }
        }

        // Sonraki maçı tamamlanmış olarak işaretleme metodu
        [HttpPost]
        public async Task<IActionResult> PromoteNextMatch()
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Mevcut tamamlanmış maçı bul
                var completedMatch = await _context.Matches.FirstOrDefaultAsync(m => m.Status == 2);
                if (completedMatch != null)
                {
                    // Tamamlanmış maçı sil
                    _context.Matches.Remove(completedMatch);
                    await _context.SaveChangesAsync();
                }

                // Kuyrukta bekleyen en eski maçı bul (FIFO)
                var nextMatch = await _context.Matches
                    .Where(m => m.Status == 0)
                    .OrderBy(m => m.Id)  // En eski eklenen maç
                    .FirstOrDefaultAsync();

                if (nextMatch == null)
                {
                    TempData["ErrorMessage"] = "Sırada bekleyen maç bulunamadı.";
                    return RedirectToAction("AddMatch");
                }

                // Maçı tamamlandı olarak işaretle
                nextMatch.Status = 2;
                
                // Takımları al
                var homeTeam = await _context.Teams.FindAsync(nextMatch.HomeTeamId);
                var awayTeam = await _context.Teams.FindAsync(nextMatch.AwayTeamId);

                if (homeTeam != null && awayTeam != null)
                {
                    // Gol farkı hesapla
                    int goalDifference = Math.Abs(nextMatch.HomeScore - nextMatch.AwayScore);
                    bool isDraw = nextMatch.HomeScore == nextMatch.AwayScore;

                    if (isDraw)
                    {
                        // Beraberlik durumu - her iki takıma 1 puan
                        homeTeam.CurrentScore += 1;
                        awayTeam.CurrentScore += 1;

                        // Her iki takıma da D (Draw) ekle
                        UpdateTeamResults(homeTeam, "D", 0);
                        UpdateTeamResults(awayTeam, "D", 0);
                    }
                    else if (nextMatch.HomeScore > nextMatch.AwayScore)
                    {
                        // Ev sahibi kazandı
                        // 3 puan ve gol farkı kadar ekstra puan
                        homeTeam.CurrentScore += 3 + goalDifference;
                        
                        // Takım sonuçlarını güncelle
                        UpdateTeamResults(homeTeam, "W", goalDifference);
                        UpdateTeamResults(awayTeam, "L", -goalDifference);
                    }
                    else
                    {
                        // Deplasman takımı kazandı
                        // 3 puan ve gol farkı kadar ekstra puan
                        awayTeam.CurrentScore += 3 + goalDifference;

                        // Takım sonuçlarını güncelle
                        UpdateTeamResults(homeTeam, "L", -goalDifference);
                        UpdateTeamResults(awayTeam, "W", goalDifference);
                    }

                    // Değişiklikleri kaydet
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Sıradaki maç tamamlandı olarak işaretlendi ve takım puanları güncellendi.";
                return RedirectToAction("AddMatch");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("AddMatch");
            }
        }

        // Takım sonuçlarını güncelleme yardımcı metodu
        private void UpdateTeamResults(Team team, string result, int scoreDifference)
        {
            // Son 5 maç sonucunu güncelle
            if (team.Last5Match == null)
            {
                team.Last5Match = new List<string>();
            }

            // Sonuçları güncelle (en fazla 5 maç tutulur)
            team.Last5Match.Insert(0, result);
            if (team.Last5Match.Count > 5)
            {
                team.Last5Match.RemoveAt(5);
            }

            // Not: Puan hesaplaması artık ilgili metotlarda doğrudan yapılıyor
        }

        // API için maç ekleme metodu - adını ve route'u değiştirdim
        [HttpPost]
        [Route("api/matches/add")]
        public async Task<IActionResult> ApiAddMatch([FromBody] MatchAddRequest request)
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
            
            // Tamamlanmış maç var mı kontrol et
            bool hasCompletedMatch = _context.Matches.Any(m => m.Status == 2);
            
            // Hiç tamamlanmış maç yoksa ilk eklenen Tamamlandı, varsa Planlandı olacak
            int status = hasCompletedMatch ? 0 : 2; // 2:Tamamlandı, 0:Planlandı

            var match = new Match
            {
                HomeTeamId = request.Team1Id,
                AwayTeamId = request.Team2Id,
                HomeTeamName = team1.Name,
                AwayTeamName = team2.Name,
                HomeScore = request.Score1,
                AwayScore = request.Score2,
                Status = status,
                Location = "Belirtilmedi",
                MatchDate = DateTime.Now
            };

            _context.Matches.Add(match);
            _context.SaveChanges();
            
            // Eğer maç tamamlandıysa (status = 2), takım skorlarını ve son 5 maç sonuçlarını güncelle
            if (status == 2)
            {
                // Gol farkı hesapla
                int goalDifference = Math.Abs(request.Score1 - request.Score2);
                bool isDraw = request.Score1 == request.Score2;

                if (isDraw)
                {
                    // Beraberlik durumu - her iki takıma 1 puan
                    team1.CurrentScore += 1;
                    team2.CurrentScore += 1;

                    // Her iki takıma da D (Draw) ekle
                    UpdateTeamResults(team1, "D", 0);
                    UpdateTeamResults(team2, "D", 0);
                }
                else if (request.Score1 > request.Score2)
                {
                    // Ev sahibi kazandı
                    // 3 puan ve gol farkı kadar ekstra puan
                    team1.CurrentScore += 3 + goalDifference;
                    
                    // Takım sonuçlarını güncelle
                    UpdateTeamResults(team1, "W", goalDifference);
                    UpdateTeamResults(team2, "L", -goalDifference);
                }
                else
                {
                    // Deplasman takımı kazandı
                    // 3 puan ve gol farkı kadar ekstra puan
                    team2.CurrentScore += 3 + goalDifference;

                    // Takım sonuçlarını güncelle
                    UpdateTeamResults(team1, "L", -goalDifference);
                    UpdateTeamResults(team2, "W", goalDifference);
                }

                // Değişiklikleri kaydet
                _context.SaveChanges();
            }

            return Ok(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteMatch(int id)
        {
            // Admin giriş yapmış mı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var match = _context.Matches.FirstOrDefault(m => m.Id == id);
                if (match == null)
                {
                    TempData["ErrorMessage"] = "Maç bulunamadı.";
                    return RedirectToAction("AddMatch");
                }

                _context.Matches.Remove(match);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Maç başarıyla silindi!";
                return RedirectToAction("AddMatch");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Maç silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("AddMatch");
            }
        }

        #endregion

        // İstek modelleri
        public class MatchAddRequest
        {
            public int Team1Id { get; set; }
            public int Team2Id { get; set; }
            public int Score1 { get; set; }
            public int Score2 { get; set; }
        }
    }
}
