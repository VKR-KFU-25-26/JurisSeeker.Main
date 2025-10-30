using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JurisSeeker.Persistence.EfContext;

public class EfFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\JurisSeeker.Web\"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        var connectionString = configuration.GetConnectionString("Postgres");
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}