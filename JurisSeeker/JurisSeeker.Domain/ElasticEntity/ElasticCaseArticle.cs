
using JurisSeeker.Domain.Common.Enums;

namespace JurisSeeker.Domain.ElasticEntity;

public class ElasticCaseArticle
{
    /// <summary>
    /// Номер статьи
    /// </summary>
    public string ArticleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Описание статьи
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Исход по статье
    /// </summary>
    public ArticleOutcome Outcome { get; set; } = ArticleOutcome.Unknown;

    /// <summary>
    /// Часть статьи
    /// </summary>
    public string? Part { get; set; }

    /// <summary>
    /// Пункт статьи
    /// </summary>
    public string? Point { get; set; }
}