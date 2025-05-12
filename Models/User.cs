using System.Security.AccessControl;
using webapplication.Models;

namespace webapplication.Models
{
    public class User
    {
        public int Id { get; set; } // required kaldırıldı
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool IsAdmin { get; set; } = false; // Admin kontrolü için yeni özellik

        // favori takımların id değerlerini tutuyoruz
        public List<int> Favourites { get; set; }

        public User()
        {
            Favourites = new List<int>();
        }
    }
}
