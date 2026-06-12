using HtmlAgilityPack;
using JurisSeeker.Application.Dto.Messages;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.CourtCaseFromParser.Command;

/// <summary>
/// Команда для сохранения дел в базу
/// </summary>
/// <param name="db"></param>
/// <param name="logger"></param>
public class ProcessCourtCaseCommandHandler(
    IDbContext db,
    ILogger<ProcessCourtCaseCommandHandler> logger)
    : IRequestHandler<ProcessCourtCaseCommand, Guid>
{
    public async Task<Guid> Handle(ProcessCourtCaseCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var id = await UpsertPostgresAsync(message, cancellationToken);

        if (id == Guid.Empty)
            return Guid.Empty;
        
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
                    OriginalCaseLink = message.OriginalCaseLink,
                    HasDecision = message.HasDecision,
                    DecisionLink = message.DecisionLink,
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
                    DecisionContentHtml = message.DecisionContent,
                    DecisionContent = message.DecisionContent != null ? CleanHtml(message.DecisionContent) : string.Empty,
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
                            .SetProperty(x => x.OriginalCaseLink, message.OriginalCaseLink)
                            .SetProperty(x => x.HasDecision, message.HasDecision)
                            .SetProperty(x => x.DecisionLink, message.DecisionLink)
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
                            .SetProperty(x => x.DecisionContentHtml, message.DecisionContent)
                            .SetProperty(x => x.DecisionContent, CleanHtml(message.DecisionContent!))
                            .SetProperty(x => x.StartDate, message.StartDate)
                            .SetProperty(x => x.JudgeName, message.JudgeName),
                        cancellationToken
                    );

                logger.LogInformation("Дело {CaseNumber} обновлено", message.CaseNumber);
            }

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
    
    private string CleanHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.InnerText.Trim(); 
    }
}
