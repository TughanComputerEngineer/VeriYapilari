using System.Runtime.CompilerServices;

namespace webapplication.Models
{
    public class Team
    {
        public int Id { get; set;}
        public required string Name { get; set;}

        // current season
        public required string CurrentLeague { get; set;}
        public required int CurrentScore { get; set;}
        public required List<string> Last5Match { get; set;}

        public Team()
        {
            Last5Match = new List<string>();
        }

    }
}