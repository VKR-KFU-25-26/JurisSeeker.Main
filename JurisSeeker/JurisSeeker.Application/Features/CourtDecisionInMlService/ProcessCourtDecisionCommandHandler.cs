using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.CourtDecisionInMlService;

/// <summary>
/// Команда для отправки дела в ml сервис
/// </summary>
public class CourtDecisionCommandHandler : IRequestHandler<ProcessCourtDecisionCommand, bool>
{
    private readonly ILogger<CourtDecisionCommandHandler> _logger;
    private readonly IKafkaCourtMlProducer _kafkaProducer;
    private readonly IDbContext _context;

    public CourtDecisionCommandHandler(ILogger<CourtDecisionCommandHandler> logger, IKafkaCourtMlProducer kafkaProducer, IDbContext context)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _context = context;
    }
    public async Task<bool> Handle(ProcessCourtDecisionCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;
        try
        {
            var courtCase = await _context.CourtCases.FirstOrDefaultAsync(x => x.Id == message.Id, cancellationToken)
                ?? throw new NullReferenceException();

            var result = false;
            
            if (courtCase.HasDecision && courtCase.Outcome == CaseOutcome.Unknown && !string.IsNullOrWhiteSpace(courtCase.DecisionContent))
            {
                _logger.LogInformation("Отправка дела в ML сервис: {CaseId}", message.Id);
            
                result = await _kafkaProducer.SendCourtCaseToMlServiceAsync(message, cancellationToken);
            
                if (result)
                {
                    _logger.LogInformation("Дело успешно отправлено в ML сервис: {CaseId}", message.Id);
                }
                else
                {
                    _logger.LogWarning("Не удалось отправить дело в ML сервис: {CaseId}", message.Id);
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке команды отправки в ML сервис для дела: {CaseId}", 
                message.Id);
            return false;
        }
    }
}