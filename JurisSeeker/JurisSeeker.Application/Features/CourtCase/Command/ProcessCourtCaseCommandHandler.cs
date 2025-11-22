using JurisSeeker.Application.Dto.Messages;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.ElasticEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.CourtCase.Command;

public class ProcessCourtCaseCommandHandler : IRequestHandler<ProcessCourtCaseCommand, Guid>
{
    private readonly IDbContext _db;
    private readonly IElasticsearchContext _elastic;
    private readonly ILogger<ProcessCourtCaseCommandHandler> _logger;

    public ProcessCourtCaseCommandHandler(IDbContext db, IElasticsearchContext elastic, ILogger<ProcessCourtCaseCommandHandler> logger)
    {
        _db = db;
        _elastic = elastic;
        _logger = logger;
    }
    
    public async Task<Guid> Handle(ProcessCourtCaseCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;
   
        var id = await SaveCaseToDatabaseAsync(message, cancellationToken);
        
        if (id == Guid.Empty) 
            return Guid.Empty;
                    
        await IndexToElasticAsync(id, message, cancellationToken);
        return id;
    }
    
    private async Task<Guid> SaveCaseToDatabaseAsync(CourtCaseMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _db.CourtCases.AnyAsync(x => x.CaseNumber == message.CaseNumber, cancellationToken);
        
            if (exists)
            {
                _logger.LogWarning("Дело с номером {CaseNumber} уже существует", message.CaseNumber);
                return Guid.Empty;
            }
        
            var courtCase = new Domain.Entities.CourtCase
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
                Representatives = message.Representatives,
                CaseResult = message.CaseResult,
                ReceivedDate = message.ReceivedDate,
                CaseCategory = message.CaseCategory,
                CaseSubcategory = message.CaseSubcategory,
                Outcome = CaseOutcome.Unknown,
                Articles = [],
                DecisionContent = null,
                StartDate = message.StartDate,
                JudgeName = message.JudgeName,
                Movements = message.CaseMovements
            };
        
            await _db.CourtCases.AddAsync(courtCase, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Дело {CaseNumber} успешно сохранено в БД", message.CaseNumber);
            return courtCase.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении дела {CaseNumber} в БД", message.CaseNumber);
            return Guid.Empty;
        }
    }
    
    private async Task IndexToElasticAsync(
        Guid id,
        CourtCaseMessage message,
        CancellationToken token)
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
            Subject = message.Subject, HasDecision = message.HasDecision,
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
        
        await _elastic.IndexCourtDecisionDocumentAsync(doc, token);

        _logger.LogInformation("Документ дела {CaseNumber} отправлен в Elasticsearch", doc.CaseNumber);
    }
}