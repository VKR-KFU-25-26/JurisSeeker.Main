using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JurisSeeker.Persistence.EfContext;

public class ApplicationDbContext : DbContext, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<CourtCase> CourtCases { get; set; }
    public DbSet<CaseArticle> CaseArticles { get; set; }
    public DbSet<CourtCaseMovement> CourtCaseMovements { get; set; }
    public DbSet<User> Users { get; set; }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) 
        => await Database.BeginTransactionAsync(cancellationToken);

    public IDbContextTransaction? CurrentTransaction => Database.CurrentTransaction;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Entry).Assembly);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Unspecified ? 
                                DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                }
            }
        }
    }
}