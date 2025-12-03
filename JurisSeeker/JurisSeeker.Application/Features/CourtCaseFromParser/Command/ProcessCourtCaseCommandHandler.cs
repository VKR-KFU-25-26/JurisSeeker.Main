using JurisSeeker.Application.Dto.Messages;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.Entities;
using JurisSeeker.Domain.ElasticEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.CourtCaseFromParser.Command;

public class ProcessCourtCaseCommandHandler(
    IDbContext db,
    IElasticsearchContext elastic,
    ILogger<ProcessCourtCaseCommandHandler> logger)
    : IRequestHandler<ProcessCourtCaseCommand, Guid>
{
    public async Task<Guid> Handle(ProcessCourtCaseCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var id = await UpsertPostgresAsync(message, cancellationToken);

        if (id == Guid.Empty)
            return Guid.Empty;

        await UpsertElasticAsync(id, message, cancellationToken);

        return id;
    }

    private async Task<Guid> UpsertPostgresAsync(CourtCaseMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var exist = await db.CourtCases
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CaseNumber == message.CaseNumber, cancellationToken);

            Guid caseId;

            if (exist is null)
            {
                var newCase = new CourtCase
                {
                    Id = Guid.NewGuid(),
                    CaseNumber = message.CaseNumber,
                    Title = message.Title,
                    Link = message.Link,
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
                    Representatives = message.Representatives,
                    CaseResult = message.CaseResult,
                    ReceivedDate = message.ReceivedDate,
                    CaseCategory = message.CaseCategory,
                    CaseSubcategory = message.CaseSubcategory,
                    DecisionContent = message.DecisionContent,
                    StartDate = message.StartDate,
                    JudgeName = message.JudgeName
                };

                caseId = newCase.Id;

                await db.CourtCases.AddAsync(newCase, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Создано новое дело {CaseNumber}", message.CaseNumber);
            }
            else
            {
                caseId = exist.Id;

                await db.CourtCases
                    .Where(x => x.Id == caseId)
                    .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.Title, message.Title)
                            .SetProperty(x => x.Link, message.Link)
                            .SetProperty(x => x.CourtType, message.CourtType)
                            .SetProperty(x => x.Description, message.Description)
                            .SetProperty(x => x.OriginalCaseLink, message.OriginalCaseLink)
                            .SetProperty(x => x.Subject, message.Subject)
                            .SetProperty(x => x.HasDecision, message.HasDecision)
                            .SetProperty(x => x.DecisionLink, message.DecisionLink)
                            .SetProperty(x => x.DecisionDate, message.DecisionDate)
                            .SetProperty(x => x.DecisionType, message.DecisionType)
                            .SetProperty(x => x.FederalDistrict, message.FederalDistrict)
                            .SetProperty(x => x.Region, message.Region)
                            .SetProperty(x => x.Plaintiff, message.Plaintiff)
                            .SetProperty(x => x.Defendant, message.Defendant)
                            .SetProperty(x => x.ThirdParties, message.ThirdParties)
                            .SetProperty(x => x.Representatives, message.Representatives)
                            .SetProperty(x => x.CaseResult, message.CaseResult)
                            .SetProperty(x => x.ReceivedDate, message.ReceivedDate)
                            .SetProperty(x => x.CaseCategory, message.CaseCategory)
                            .SetProperty(x => x.CaseSubcategory, message.CaseSubcategory)
                            .SetProperty(x => x.DecisionContent, message.DecisionContent)
                            .SetProperty(x => x.StartDate, message.StartDate)
                            .SetProperty(x => x.JudgeName, message.JudgeName),
                        cancellationToken
                    );

                logger.LogInformation("Дело {CaseNumber} обновлено", message.CaseNumber);
            }

            // === Movements ===
            await db.CourtCaseMovements
                .Where(m => m.CourtCaseId == caseId)
                .ExecuteDeleteAsync(cancellationToken);

            if (message.CaseMovements.Count > 0)
            {
                var movements = message.CaseMovements.Select(m => new CourtCaseMovement
                {
                    Id = Guid.NewGuid(),
                    CourtCaseId = caseId,
                    EventName = m.EventName,
                    EventResult = m.EventResult,
                    Basis = m.Basis,
                    EventDate = m.EventDate
                }).ToList();

                await db.CourtCaseMovements.AddRangeAsync(movements, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            return caseId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в UpsertPostgresAsync для дела {CaseNumber}", message.CaseNumber);
            return Guid.Empty;
        }
    }
    
    private async Task UpsertElasticAsync(Guid id, CourtCaseMessage message, CancellationToken token)
    {
        try
        {
            var doc = new CourtDecisionDocument
            {
                CourtCaseId = id,
                CaseNumber = message.CaseNumber,
                Region = message.Region,
                DecisionDate = message.DecisionDate,
                DecisionType = message.DecisionType,
                DecisionContent = message.DecisionContent ?? string.Empty,
                Link = message.Link,
                CourtType = message.CourtType,
                Description = message.Description,
                OriginalCaseLink = message.OriginalCaseLink,
                Subject = message.Subject,
                HasDecision = message.HasDecision,
                DecisionLink = message.DecisionLink,
                FederalDistrict = message.FederalDistrict,
                Plaintiff = message.Plaintiff,
                Defendant = message.Defendant,
                ThirdParties = message.ThirdParties,
                Representatives = message.Representatives,
                JudgeName = message.JudgeName,
                ReceivedDate = message.ReceivedDate,
                StartDate = message.StartDate,
                CaseCategory = message.CaseCategory,
                CaseSubcategory = message.CaseSubcategory,
                Outcome = CaseOutcome.Unknown,
                CaseResult = message.CaseResult,
                Articles = [],
                Movements = message.CaseMovements.Select(x => new ElasticCourtCaseMovement
                {
                    EventName = x.EventName,
                    EventResult = x.EventResult,
                    Basis = x.Basis,
                    EventDate = x.EventDate
                }).ToList(),
                Timestamp = message.Timestamp
            };

            await elastic.IndexCourtDecisionDocumentAsync(doc, token);

            logger.LogInformation(
                "Дело {CaseNumber} успешно проиндексировано в Elasticsearch",
                message.CaseNumber
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка индексации в Elasticsearch для {CaseNumber}", message.CaseNumber);
        }
    }
}
