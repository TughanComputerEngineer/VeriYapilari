using Microsoft.EntityFrameworkCore;
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

            // Servisleri ekliyoruz
            builder.Services.AddControllersWithViews(); // Controller ve View'leri ekliyoruz
            builder.Services.AddRazorPages();


            builder.Services.AddSession();

            var app = builder.Build();

            // Veritabanı işlemlerini uygulama başlatılmadan önce yapıyoruz
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                db.Database.Migrate();  // migrations'ı otomatik olarak çalıştırıyoruz

                // Eğer kullanıcı yoksa örnek kullanıcı ekleniyor
                if (false)
                {
                    var user = new User
                    {
                        Username = "denemekullanici2",
                        Password = "sifre123",  // Gerçek uygulamada hash kullan
                        Favourites = new List<int> { 1, 2, 3, 10 } // Örnek takım ID'leri
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                // Veritabanına veri ekliyoruz
                if (!db.Teams.Any())
                {
                    var team = new Team()
                    {
                        Name = "denemeadınayaplmışuzunisimliherhangibirtakımdeğeri",
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

            // Otomatik giriş yapma işlemi (denemekullanici ile)
            //var session = app.Services.GetRequiredService<ISession>();
            //session.SetString("username", "denemekullanici");

            // HTTP isteği işleme
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();
            app.MapRazorPages(); // Razor Pages yönlendirmesi

            app.MapGet("/", () => Results.Redirect("/home"));
            app.MapControllerRoute( // Controller routing
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); 
            

            // Uygulamayı başlatıyoruz
            app.Run();
        }
    }
}

/*
var heapTree = new HeapTree();
foreach (var team in db.Teams)
{
    heapTree.Insert(team);
}
var max = heapTree.GetMax();
Console.WriteLine($"Max: {max}");
*/