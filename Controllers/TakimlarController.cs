using Microsoft.AspNetCore.Mvc;
using webapplication.Models;

namespace webapplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TakimlarController : ControllerBase
    {
        private readonly DataContext _context;

        public TakimlarController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
           
            var heap = new HeapTree();

           
            var takımlar = _context.Teams.ToList();
            foreach (var team in takımlar)
            {
                heap.Insert(team);
            }

            
            var siraliTakimlar = heap.ToSortedList(); 

            var sonuc = siraliTakimlar.Select((t, index) => new
            {
                id = t.Id,
                isim = t.Name,
                puan = t.CurrentScore,
                sira = index + 1,
                son5mac = t.Last5Match,
                logo = $"images/{t.Name.ToLower().Replace(" ", "")}.png"
            }).ToList();

            return Ok(sonuc);
        }
    }
}
