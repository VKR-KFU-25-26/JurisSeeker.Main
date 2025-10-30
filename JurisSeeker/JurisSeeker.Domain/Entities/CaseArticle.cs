using JurisSeeker.Domain.Common;
using JurisSeeker.Domain.Common.Enums;

namespace JurisSeeker.Domain.Entities;

/// <summary>
/// Статья закона по делу
/// </summary>
public class CaseArticle : BaseEntity
{
    /// <summary>
    /// Номер статьи (например, "ст. 159 УК РФ")
    /// </summary>
    public string ArticleNumber { get; set; } = null!;
    
    /// <summary>
    /// Описание статьи
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// По данной статье выиграл истец/ответчик
    /// </summary>
    public ArticleOutcome Outcome { get; set; } = ArticleOutcome.Unknown;
    
    /// <summary>
    /// Часть статьи (если применимо)
    /// </summary>
    public string? Part { get; set; }
    
    /// <summary>
    /// Пункт статьи (если применимо)
    /// </summary>
    public string? Point { get; set; }
    
    /// <summary>
    /// Id дела
    /// </summary>
    public Guid CourtCaseId { get; set; }
    
    /// <summary>
    /// Дело
    /// </summary>
    public CourtCase CourtCase { get; set; } = null!;
}