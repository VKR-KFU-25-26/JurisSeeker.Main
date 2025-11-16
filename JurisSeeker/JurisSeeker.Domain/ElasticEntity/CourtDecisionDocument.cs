using JurisSeeker.Domain.Common.Enums;

namespace JurisSeeker.Domain.ElasticEntity;

/// <summary>
/// Сущность дела в ElasticSearch
/// </summary>
public class CourtDecisionDocument
{
     /// <summary>
    /// ID из PostgreSQL для связи между системами
    /// </summary>
    public Guid CourtCaseId { get; set; }

    // ========== ОСНОВНЫЕ ПОЛЯ ==========
    
    /// <summary>
    /// Заголовок дела
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Ссылка на дело в системе судов
    /// </summary>
    public string Link { get; set; } = string.Empty;

    /// <summary>
    /// Номер дела
    /// </summary>
    public string CaseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Тип суда
    /// </summary>
    public string CourtType { get; set; } = string.Empty;

    /// <summary>
    /// Описание дела
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Ссылка на оригинальный сайт суда
    /// </summary>
    public string OriginalCaseLink { get; set; } = string.Empty;

    /// <summary>
    /// Суть спора
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    // ========== РЕШЕНИЕ ==========
    
    /// <summary>
    /// Флаг наличия решения
    /// </summary>
    public bool HasDecision { get; set; }

    /// <summary>
    /// Ссылка на решение
    /// </summary>
    public string? DecisionLink { get; set; }

    /// <summary>
    /// Дата решения
    /// </summary>
    public DateTime? DecisionDate { get; set; }

    /// <summary>
    /// Тип решения
    /// </summary>
    public string? DecisionType { get; set; }

    /// <summary>
    /// Текст решения (основное поле для поиска)
    /// </summary>
    public string? DecisionContent { get; set; }

    // ========== ГЕОГРАФИЯ ==========
    
    /// <summary>
    /// Федеральный округ
    /// </summary>
    public string FederalDistrict { get; set; } = string.Empty;

    /// <summary>
    /// Регион
    /// </summary>
    public string Region { get; set; } = string.Empty;

    // ========== УЧАСТНИКИ ПРОЦЕССА ==========
    
    /// <summary>
    /// Истец
    /// </summary>
    public string Plaintiff { get; set; } = string.Empty;

    /// <summary>
    /// Ответчик
    /// </summary>
    public string Defendant { get; set; } = string.Empty;

    /// <summary>
    /// Третьи лица
    /// </summary>
    public string? ThirdParties { get; set; }

    /// <summary>
    /// Представители
    /// </summary>
    public string? Representatives { get; set; } = string.Empty;

    /// <summary>
    /// Судья
    /// </summary>
    public string? JudgeName { get; set; } = string.Empty;

    // ========== ДАТЫ ==========
    
    /// <summary>
    /// Дата поступления дела
    /// </summary>
    public DateTime? ReceivedDate { get; set; }

    /// <summary>
    /// Дата начала дела
    /// </summary>
    public DateTime StartDate { get; set; }

    // ========== КАТЕГОРИИ И РЕЗУЛЬТАТ ==========
    
    /// <summary>
    /// Категория дела
    /// </summary>
    public string CaseCategory { get; set; } = string.Empty;

    /// <summary>
    /// Подкатегория дела
    /// </summary>
    public string CaseSubcategory { get; set; } = string.Empty;

    /// <summary>
    /// Исход дела
    /// </summary>
    public CaseOutcome Outcome { get; set; } = CaseOutcome.Unknown;

    /// <summary>
    /// Результат дела
    /// </summary>
    public string CaseResult { get; set; } = string.Empty;

    // ========== КОЛЛЕКЦИИ ==========
    
    /// <summary>
    /// Статьи по делу
    /// </summary>
    public List<ElasticCaseArticle> Articles { get; set; } = new();

    /// <summary>
    /// Движение дела
    /// </summary>
    public List<ElasticCourtCaseMovement> Movements { get; set; } = new();

    /// <summary>
    /// Дата индексации в Elasticsearch
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}