using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JurisSeeker.Application.Features.SaveFromMlService;

public class SaveFromMlServiceCommandHandler : IRequestHandler<SaveResultFromMlServiceCommand>
{
    private readonly IDbContext _context;

    public SaveFromMlServiceCommandHandler(IDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(SaveResultFromMlServiceCommand request, CancellationToken cancellationToken)
    {
        var message = request.MlProcessingResult;

        if (!Guid.TryParse(message.CaseId, out var caseId) || message.Articles == null! || message.Winner == null!)
            throw new ArgumentNullException();
    
        // 1. Обновляем Outcome
        await _context.CourtCases
            .Where(x => x.Id == caseId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Outcome, ToDisplayEnum(message.Winner)), 
                cancellationToken);
    
        // 2. Удаляем старые статьи
        await _context.CaseArticles
            .Where(x => x.CourtCaseId == caseId)
            .ExecuteDeleteAsync(cancellationToken);
    
        // 3. Добавляем новые статьи
        var newArticles = message.Articles
            .Where(article => !string.IsNullOrWhiteSpace(article))
            .Select(article => new CaseArticle
            {
                Id = Guid.NewGuid(),
                ArticleNumber = article,
                CourtCaseId = caseId
            })
            .ToList();
    
        if (newArticles.Count != 0)
        {
            await _context.CaseArticles.AddRangeAsync(newArticles, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    
    private static CaseOutcome ToDisplayEnum(string outcome)
    {
        return outcome switch
        {
            "Неизвестно" => CaseOutcome.Unknown,
            "Истец выиграл" => CaseOutcome.PlaintiffWon,
            "Ответчик выиграл" => CaseOutcome.DefendantWon,
            "Мировое соглашение" => CaseOutcome.Settlement,
            "Дело прекращено" => CaseOutcome.CaseDismissed,
            "Частично удовлетворено" => CaseOutcome.PartiallyGranted,
            _ => CaseOutcome.Unknown
        };
    }
}