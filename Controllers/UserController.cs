using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Favourites()
        {
            // Giriş yapan kullanıcıyı Session'dan alıyoruz (girişte session'a yazdığını varsayıyoruz)
            var username = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login"); // Kullanıcı giriş yapmamışsa login'e yönlendir

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login");

            // Favori takım ID'lerini al ve o takımları sorgula
            var favouriteTeams = _context.Teams
                .Where(t => user.Favourites.Contains(t.Id))
                .ToList();

            return View(favouriteTeams); // View'a gönder
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Bu kullanıcı adı zaten mevcut.");
            }

            user.Role = UserRole.User;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Session'dan tüm verileri temizle
            return Redirect("/home");  // Ana sayfaya yönlendir
        }

        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.Select(u => u.Username).ToList();
            return Ok(users);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            

            if (user == null || user.Password != model.Password)
            {
                return BadRequest("Kullanıcı adı veya şifre hatalı.");
            }

            // Başarılı giriş sonrası, kullanıcının oturum bilgisini session'a kaydet
            HttpContext.Session.SetString("username", model.Username);
            HttpContext.Session.SetString("role", model.Role.ToString());

            return Ok(user.Role.ToString());

        }
    }
}

