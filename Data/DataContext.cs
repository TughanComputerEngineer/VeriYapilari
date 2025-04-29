using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;


namespace WebApplication1.Data
{
    public class DataContext : DbContext
    {
        // Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSet - Team modelini bağlayacak
        public DbSet<Team> Teams { get; set; }
    }
}

