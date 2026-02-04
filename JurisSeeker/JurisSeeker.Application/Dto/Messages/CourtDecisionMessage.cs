namespace JurisSeeker.Application.Dto.Messages;

/// <summary>
/// Сообщение кафка в мл сервис
/// </summary>
public class CourtDecisionMessage
{
    /// <summary>
    /// Id дела
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Решение
    /// </summary>
    public required string Decision { get; set; }
}