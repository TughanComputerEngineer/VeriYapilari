using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

namespace webapplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavorilerController : ControllerBase
    {
        private readonly DataContext _context;

        private static Dictionary<string, CustomLinkedList> FavoriListeleri = new();

        public FavorilerController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("listele")]
        public IActionResult Listele()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.Favourites == null)
                return NotFound();

            
            var list = new CustomLinkedList();
            foreach (var id in user.Favourites)
            {
                var t = _context.Teams.FirstOrDefault(t => t.Id == id);
                if (t != null)
                    list.Add(t);
            }
            FavoriListeleri[username] = list;

            
            var heap = new HeapTree();
            var tumTakimlar = _context.Teams.ToList();
            foreach (var t in tumTakimlar)
                heap.Insert(t);
            var sirali = heap.ToSortedList();

            
            var sonuc = new List<object>();
            var node = FavoriListeleri[username].root;

            while (node != null)
            {
                var t = node.team;
                sonuc.Add(new
                {
                    id = t.Id,
                    isim = t.Name,
                    puan = t.CurrentScore,
                    sira = sirali.FindIndex(x => x.Id == t.Id) + 1
                });

                node = node.next;
            }

            return Ok(sonuc);
        }
    }
}
