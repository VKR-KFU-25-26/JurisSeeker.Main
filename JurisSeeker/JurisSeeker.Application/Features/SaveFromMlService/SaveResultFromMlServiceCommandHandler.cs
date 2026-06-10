using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.SaveFromMlService;

public class SaveFromMlServiceCommandHandler : IRequestHandler<SaveResultFromMlServiceCommand>
{
    private readonly IDbContext _context;
    private readonly ILogger<SaveFromMlServiceCommandHandler> _logger;

    public SaveFromMlServiceCommandHandler(IDbContext context, ILogger<SaveFromMlServiceCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task Handle(SaveResultFromMlServiceCommand request, CancellationToken cancellationToken)
    {
        var message = request.MlProcessingResult;
        
        try
        {
            if (!Guid.TryParse(message.Id, out var caseId))
                throw new ArgumentNullException();
    
            // Обновляем Outcome
            await _context.CourtCases
                .Where(x => x.Id == caseId)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Outcome, ToDisplayEnum(message.Winner)),
                    cancellationToken);
            
            _logger.LogInformation($"Успешно приняли данные с Ml сервиса и обновили данные по делу c Id: {message.Id}");
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при получении данных из Ml сервиса");
            _logger.LogError(e.Message, e.StackTrace);
        }
    }
    
    private static CaseOutcome ToDisplayEnum(string outcome)
    {
        return outcome.ToLower() switch
        {
            "истец" => CaseOutcome.PlaintiffWon,
            "ответчик" => CaseOutcome.DefendantWon,
            _ => CaseOutcome.Unknown
        };
    }
}