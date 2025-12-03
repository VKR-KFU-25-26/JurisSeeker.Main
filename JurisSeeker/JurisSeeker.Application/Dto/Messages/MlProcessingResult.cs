namespace JurisSeeker.Application.Dto.Messages;

public class MlProcessingResult
{
    public string CaseId { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public List<string> Articles { get; set; } = [];
}