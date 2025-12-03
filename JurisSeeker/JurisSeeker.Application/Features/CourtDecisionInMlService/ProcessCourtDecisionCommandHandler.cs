using JurisSeeker.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Application.Features.CourtDecisionInMlService;

public class CourtDecisionCommandHandler : IRequestHandler<ProcessCourtDecisionCommand, bool>
{
    private readonly ILogger<CourtDecisionCommandHandler> _logger;
    private readonly IKafkaCourtMlProducer _kafkaProducer;

    public CourtDecisionCommandHandler(ILogger<CourtDecisionCommandHandler> logger, IKafkaCourtMlProducer kafkaProducer)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }
    public async Task<bool> Handle(ProcessCourtDecisionCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;
        try
        {
            _logger.LogInformation("Отправка дела в ML сервис: {CaseId}", message.Id);
            
            var result = await _kafkaProducer.SendCourtCaseToMlServiceAsync(message, cancellationToken);
            
            if (result)
            {
                _logger.LogInformation("Дело успешно отправлено в ML сервис: {CaseId}", message.Id);
            }
            else
            {
                _logger.LogWarning("Не удалось отправить дело в ML сервис: {CaseId}", message.Id);
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