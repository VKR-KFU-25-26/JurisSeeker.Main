using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JurisSeeker.Persistence.Configurations;

public class CourtCaseConfigurations : IEntityTypeConfiguration<CourtCase>
{
    public void Configure(EntityTypeBuilder<CourtCase> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Link).IsRequired();
        
        builder.Property(x => x.CaseNumber).IsRequired();
        
        builder.HasMany(x => x.Articles)
            .WithOne(x => x.CourtCase)
            .HasForeignKey(x => x.CourtCaseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.CourtCase)
            .HasForeignKey(x => x.CourtCaseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => c.CaseNumber);
        builder.HasIndex(c => c.Outcome);
        builder.HasIndex(c => c.Region);
    }
}