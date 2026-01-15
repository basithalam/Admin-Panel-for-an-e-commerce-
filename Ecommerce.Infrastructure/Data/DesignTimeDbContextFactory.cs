using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Ecommerce.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read connection string from appsettings.json
            var basePath = Directory.GetCurrentDirectory();

            // Try to locate appsettings.json in Admin project, then in root
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile(Path.Combine(basePath, "Ecommerce.Admin", "appsettings.json"), optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            // Read connection string named DefaultConnection
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? configuration["ConnectionStrings:DefaultConnection"]
                                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=EcommerceDb;Trusted_Connection=True;MultipleActiveResultSets=True";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
