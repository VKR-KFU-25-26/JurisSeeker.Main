using JurisSeeker.Application.Dto.Messages;

namespace JurisSeeker.Application.Interfaces;

/// <summary>
/// Продюсер для отправки в ml сервис
/// </summary>
public interface IKafkaCourtMlProducer
{
    /// <summary>
    /// Метод для отправки в мл сервиср
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> SendCourtCaseToMlServiceAsync(CourtDecisionMessage message, CancellationToken cancellationToken = default);
}