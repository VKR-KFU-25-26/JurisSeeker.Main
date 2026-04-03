using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Entities;
using JurisSeeker.Domain.Entities.NotMapped;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JurisSeeker.Persistence.EfContext;

public class ApplicationDbContext : DbContext, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<CourtCase> CourtCases { get; set; } = null!;
    public DbSet<CaseArticle> CaseArticles { get; set; } = null!;
    public DbSet<CourtCaseMovement> CourtCaseMovements { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    
    // view
    public DbSet<TotalCasesStats> TotalCasesStats { get; set; } = null!;
    public DbSet<OutcomeStats> OutcomeStats { get; set; } = null!;
    public DbSet<DecisionStats> DecisionStats { get; set; } = null!;
    public DbSet<CasesByRegionStats> CasesByRegionStats { get; set; } = null!;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) 
        => await Database.BeginTransactionAsync(cancellationToken);

    public IDbContextTransaction? CurrentTransaction => Database.CurrentTransaction;
    
    public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default)
        => await Database.ExecuteSqlRawAsync(sql, cancellationToken);
    
    public async Task<int> ExecuteSqlRawAsync(string sql, object[] parameters, CancellationToken cancellationToken = default)
        => await Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    
    public IQueryable<T> SqlQueryRaw<T>(string sql, params object[] parameters) where T : class
        => Database.SqlQueryRaw<T>(sql, parameters);
    
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
        
        
        modelBuilder.Entity<TotalCasesStats>(entity =>
        {
            entity.ToView("TotalCasesStats");
            entity.HasNoKey(); // У view нет первичного ключа
            entity.Property(e => e.TotalCases).HasColumnName("TotalCases");
            entity.Property(e => e.TotalCasesWithDecision).HasColumnName("TotalCasesWithDecision");
            entity.Property(e => e.DecisionPercentage).HasColumnName("DecisionPercentage");
            entity.Property(e => e.CalculatedAt).HasColumnName("CalculatedAt");
        });
    
        // CasesByRegionStats
        modelBuilder.Entity<CasesByRegionStats>(entity =>
        {
            entity.ToView("CasesByRegionStats");
            entity.HasNoKey();
            entity.Property(e => e.Region).HasColumnName("Region");
            entity.Property(e => e.FederalDistrict).HasColumnName("FederalDistrict");
            entity.Property(e => e.CasesInRegion).HasColumnName("CasesInRegion");
            entity.Property(e => e.PercentageOfTotal).HasColumnName("PercentageOfTotal");
            entity.Property(e => e.CalculatedAt).HasColumnName("CalculatedAt");
        });
    
        // DecisionStats
        modelBuilder.Entity<DecisionStats>(entity =>
        {
            entity.ToView("DecisionStats");
            entity.HasNoKey();
            entity.Property(e => e.FederalDistrict).HasColumnName("FederalDistrict");
            entity.Property(e => e.Region).HasColumnName("Region");
            entity.Property(e => e.TotalCases).HasColumnName("TotalCases");
            entity.Property(e => e.CasesWithDecision).HasColumnName("CasesWithDecision");
            entity.Property(e => e.DecisionRate).HasColumnName("DecisionRate");
            entity.Property(e => e.CalculatedAt).HasColumnName("CalculatedAt");
        });
    
        // OutcomeStats
        modelBuilder.Entity<OutcomeStats>(entity =>
        {
            entity.ToView("OutcomeStats");
            entity.HasNoKey();
            entity.Property(e => e.FederalDistrict).HasColumnName("FederalDistrict");
            entity.Property(e => e.Region).HasColumnName("Region");
            entity.Property(e => e.CasesWithKnownOutcome).HasColumnName("CasesWithKnownOutcome");
            entity.Property(e => e.PlaintiffWins).HasColumnName("PlaintiffWins");
            entity.Property(e => e.PlaintiffWinPercentage).HasColumnName("PlaintiffWinPercentage");
            entity.Property(e => e.DefendantWins).HasColumnName("DefendantWins");
            entity.Property(e => e.DefendantWinPercentage).HasColumnName("DefendantWinPercentage");
            entity.Property(e => e.Settlements).HasColumnName("Settlements");
            entity.Property(e => e.SettlementPercentage).HasColumnName("SettlementPercentage");
            entity.Property(e => e.Dismissed).HasColumnName("Dismissed");
            entity.Property(e => e.DismissedPercentage).HasColumnName("DismissedPercentage");
            entity.Property(e => e.CalculatedAt).HasColumnName("CalculatedAt");
        });
    }
}