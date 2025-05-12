using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using webapplication.Models;

namespace webapplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Web uygulaması başlatma
            var builder = WebApplication.CreateBuilder(args);

            // Veritabanı yapılandırmasını ekliyoruz
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlite("Data Source=database.db"));

            // Kimlik doğrulama servislerini ekliyoruz
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.Cookie.Name = "WebAppAuth";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                });

            // Servisleri ekliyoruz
            builder.Services.AddControllersWithViews(); // Controller ve View'leri ekliyoruz
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Veritabanı işlemlerini uygulama başlatılmadan önce yapıyoruz
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Veritabanını oluştur (yoksa)
                db.Database.EnsureCreated();

                // Admin hesabını kontrol et ve yoksa oluştur
                if (!db.Users.Any(u => u.Username == "admin"))
                {
                    var adminUser = new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Favourites = new List<int>(),
                        IsAdmin = true // Admin olarak işaretliyoruz
                    };
                    db.Users.Add(adminUser);
                    db.SaveChanges();
                    Console.WriteLine("Admin hesabı oluşturuldu.");
                }

                // Normal kullanıcı hesabını kontrol et ve yoksa oluştur
                if (!db.Users.Any(u => u.Username == "user"))
                {
                    var normalUser = new User
                    {
                        Username = "user",
                        Password = "user123",
                        Favourites = new List<int>(),
                        IsAdmin = false // Normal kullanıcı
                    };
                    db.Users.Add(normalUser);
                    db.SaveChanges();
                    Console.WriteLine("Normal kullanıcı hesabı oluşturuldu.");
                }

                // Veritabanına veri ekliyoruz
                if (!db.Teams.Any())
                {
                    var team = new Team()
                    {
                        Name = "Deneme",
                        CurrentLeague = "league1",
                        CurrentScore = 20,
                        Last5Match = new List<string> { "W", "", "L", "D", "W" }
                    };
                    // Yeni bir takım oluşturma
                    db.Teams.Add(team);
                    db.SaveChanges();  // Önce takım kaydedilir

                    // geçmiş sezonlar
                    var previousSeasons = new List<PreviousSeasonsClass>()
                    {
                        new PreviousSeasonsClass
                        {
                            Score = 30,
                            Year = 2022,
                            Ranking = 1,
                            TeamId = team.Id,  // Takım ID'sini ilişkilendirilir
                            Team = team
                        },
                        new PreviousSeasonsClass
                        {
                            Score = 25,
                            Year = 2021,
                            Ranking = 2,
                            TeamId = team.Id,  // Takım ID'sini ilişkilendirilir
                            Team = team
                        }
                    };
                    db.PreviousSeasons.AddRange(previousSeasons);
                    db.SaveChanges();  // Geçmiş sezonlar kaydedildi
                }
            }

            // HTTP isteği işleme
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Kimlik doğrulama ve yetkilendirme middleware'lerini ekliyoruz
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapGet("/", () => Results.Redirect("/home"));
            app.MapControllerRoute( // Controller routing
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Uygulamayı başlatıyoruz
            app.Run();
        }
    }
}
