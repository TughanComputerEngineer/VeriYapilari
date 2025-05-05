using Microsoft.EntityFrameworkCore;

namespace webapplication.Models
{
    public class DataContext:DbContext
    {
        public DbSet<Team> Teams { get; set;}

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlite("Data Source=database.db");
        }
    }
}