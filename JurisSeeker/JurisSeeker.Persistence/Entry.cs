using JurisSeeker.Core.Interfaces;
using JurisSeeker.Persistence.EfContext;
using JurisSeeker.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JurisSeeker.Persistence;

public static class Entry
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString,
            builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IDbContext, ApplicationDbContext>();
        services.AddTransient<Migrator>();
    }
}