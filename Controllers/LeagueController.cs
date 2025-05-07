using Microsoft.AspNetCore.Mvc;

namespace webapplication.Controllers
{
    public class LeagueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

