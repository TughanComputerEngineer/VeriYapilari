using System.ComponentModel.DataAnnotations;

namespace webapplication.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public int Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; } = DateTime.Now;
    }
}