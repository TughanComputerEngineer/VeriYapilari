using System.Security.AccessControl;
using webapplication.Models;

namespace webapplication.Models
{

    public enum UserRole
    {
        Guest,  // sistemde yok - kayıtlı değil
        User,     // normal kullanıcı
        Admin     // admin yetkisi olan kullanıcı
    }

    public class User
    {
        public int Id {get; set;}
        public required string Username {get; set;}
        public required string Password {get; set;}

        // favori takımların id değerlerini tutuyoruz
        public List<int> Favourites {get; set;}

        public UserRole Role { get; set; } = UserRole.Guest; // default olarak User

        public User()
        {
            Favourites = new List<int>();
        }
    }
}
