using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JurisSeeker.Persistence.Configurations;

public class CaseArticleConfiguration : IEntityTypeConfiguration<CaseArticle>
{
    public void Configure(EntityTypeBuilder<CaseArticle> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ArticleNumber).IsRequired();

        builder.HasIndex(a => a.ArticleNumber);
        builder.HasIndex(a => a.Outcome);
        builder.HasIndex(a => new { a.ArticleNumber, a.Outcome });
        builder.HasIndex(a => a.CourtCaseId);
    }
}