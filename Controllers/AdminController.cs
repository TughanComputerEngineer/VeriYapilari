using Microsoft.AspNetCore.Mvc;

namespace webapplication.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

