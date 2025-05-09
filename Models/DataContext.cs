using Microsoft.EntityFrameworkCore;

namespace webapplication.Models
{
    public class DataContext:DbContext
    {
        public DbSet<Team> Teams { get; set;}

        public DbSet<User> Users { get; set; }
        
        public DbSet<PreviousSeasonsClass> PreviousSeasons { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Team ile PreviousSeasons arasındaki ilişkiyi tanımlıyoruz
            modelBuilder.Entity<PreviousSeasonsClass>()
                .HasOne(p => p.Team)  // PreviousSeasons bir Team'e ait olacak
                .WithMany(t => t.previousSeasons)  // Team birden fazla PreviousSeasons'a sahip olacak
                .HasForeignKey(p => p.TeamId);  // Foreign key olarak TeamId kullanılacak
        }
    }
}