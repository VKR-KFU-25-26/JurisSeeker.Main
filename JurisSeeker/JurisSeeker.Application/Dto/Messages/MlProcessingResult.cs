namespace JurisSeeker.Application.Dto.Messages;

/// <summary>
/// Ответ из Ml сервиса
/// </summary>
public class MlProcessingResult
{
    public string Id { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public List<string> Articles { get; set; } = [];
}