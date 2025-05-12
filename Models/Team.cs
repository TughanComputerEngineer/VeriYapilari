using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace webapplication.Models
{
    public class PreviousSeasonsClass
    {
        public int Id { get; set; }

        public required int Score { get; set; }
        public required int Year { get; set; }
        public required int Ranking { get; set; }

        // team information
        public required int TeamId { get; set; }
        public required Team Team { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Takým logosu için resim yolu
        public string? ImagePath { get; set; }

        // current season
        public required string CurrentLeague { get; set; }
        public required int CurrentScore { get; set; }
        public required List<string> Last5Match { get; set; }

        // previous seasons
        public List<PreviousSeasonsClass> previousSeasons { get; set; }

        public Team()
        {
            Last5Match = new List<string>();
            previousSeasons = new List<PreviousSeasonsClass>();
        }
    }
}
