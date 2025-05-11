using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace webapplication.Models
{
    public class Team
    {
        public int Id { get; set;}
        public required string Name { get; set;}

        // current season
        public int CurrentScore { get; set;}
        public List<string> Last5Match { get; set;}

        public string? LogoPath {get; set;} = "/images/team_logos/placeholder.png";

        public Team()
        {
            Last5Match = new List<string>();
        }
    }
}