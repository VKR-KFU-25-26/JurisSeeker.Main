namespace JurisSeeker.Domain.ElasticEntity;

/// <summary>
/// Сущность дела в ElasticSearch
/// </summary>
public class CourtDecisionDocument
{
    /// <summary>
    /// Id в постгрес
    /// </summary>
    public Guid CourtCaseId { get; set; }    
    
    /// <summary>
    /// Номер дела
    /// </summary>
    public string CaseNumber { get; set; } = null!;
    
    /// <summary>
    /// Регион
    /// </summary>
    public string? Region { get; set; }
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime? DecisionDate { get; set; }
    
    /// <summary>
    /// Тип решения/документа
    /// </summary>
    public string? DecisionType { get; set; }
    
    /// <summary>
    /// Текст решения/документа
    /// </summary>
    public string DecisionText { get; set; } = null!;
}