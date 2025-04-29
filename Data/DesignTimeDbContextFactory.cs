using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using WebApplication1.Data;
using System.IO;

namespace WebApplication1.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // appsettings.json dosyasındaki bağlantı dizesini alıyoruz
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

            return new DataContext(optionsBuilder.Options);
        }
    }
}
