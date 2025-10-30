using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JurisSeeker.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Password).IsRequired();
    }
}