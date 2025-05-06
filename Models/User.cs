using System.Security.AccessControl;
using webapplication.Models;

namespace webapplication.Models
{
    public class User
    {
        public required int Id {get; set;}
        public required string Username {get; set;}
        public required string Password {get; set;}

        // favori takımların id değerlerini tutuyoruz
        public List<int> Favourites {get; set;}

        public User()
        {
            Favourites = new List<int>();
        }
    }
}
