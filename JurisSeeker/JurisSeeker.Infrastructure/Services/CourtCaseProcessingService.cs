using JurisSeeker.Core.Dto.Messages;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Infrastructure.Services;

public class CourtCaseProcessingService(IDbContext context, ILogger<CourtCaseProcessingService> logger)
    : ICourtCaseProcessingService
{
    public async Task<bool> ProcessCourtCaseAsync(CourtCaseMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await context.CourtCases.AnyAsync(x => x.CaseNumber == message.CaseNumber, cancellationToken);
        
            if (exists)
            {
                logger.LogWarning("Дело с номером {CaseNumber} уже существует", message.CaseNumber);
                return false;
            }
        
            var courtCase = new CourtCase
            {
                Title = message.Title,
                Link = message.Link,
                CaseNumber = message.CaseNumber,
                CourtType = message.CourtType,
                Description = message.Description,
                OriginalCaseLink = message.OriginalCaseLink,
                Subject = message.Subject,
                HasDecision = message.HasDecision,
                DecisionLink = message.DecisionLink,
                DecisionDate = message.DecisionDate,
                DecisionType = message.DecisionType,
                FederalDistrict = message.FederalDistrict,
                Region = message.Region,
                Plaintiff = message.Plaintiff,
                Defendant = message.Defendant,
                ThirdParties = message.ThirdParties,
                ReceivedDate = message.ReceivedDate,
                CaseCategory = message.CaseCategory,
                CaseSubcategory = message.CaseSubcategory,
                DecisionContent = message.DecisionContent,
                Outcome = CaseOutcome.Unknown,
                Articles = []
            };
        
            await context.CourtCases.AddAsync(courtCase, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation("Дело {CaseNumber} успешно сохранено в БД", message.CaseNumber);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при сохранении дела {CaseNumber} в БД", message.CaseNumber);
            return false;
        }
    }
}