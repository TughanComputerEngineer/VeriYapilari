using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

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
}
