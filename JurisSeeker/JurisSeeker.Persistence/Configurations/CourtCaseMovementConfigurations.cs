using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JurisSeeker.Persistence.Configurations;

public class CourtCaseMovementConfigurations : IEntityTypeConfiguration<CourtCaseMovement>
{
    public void Configure(EntityTypeBuilder<CourtCaseMovement> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.EventName).IsRequired();
        builder.Property(x => x.EventResult).IsRequired();
        builder.Property(x => x.Basis).IsRequired();
        builder.Property(x => x.EventDate);
    }
}