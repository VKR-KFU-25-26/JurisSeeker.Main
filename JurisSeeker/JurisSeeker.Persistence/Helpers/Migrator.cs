using JurisSeeker.Infrastructure.Services.Elastic;
using JurisSeeker.Persistence.EfContext;
using Microsoft.EntityFrameworkCore;

namespace JurisSeeker.Persistence.Helpers;

/// <summary>
/// Мигратор для наката миграций и сидов 
/// </summary>
public class Migrator(
    ApplicationDbContext context
   )
{
    /// <summary>
    /// Мигратор
    /// </summary>
    public async Task MigrateAsync()
    {
        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine($"migrations apply failed {e.Message}");
            throw;
        }
    }
}