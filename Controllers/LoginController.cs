using Microsoft.AspNetCore.Mvc;

namespace webapplication.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Views/Login/Index.cshtml'i döndürür
        }
    }
}