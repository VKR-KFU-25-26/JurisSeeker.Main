using JurisSeeker.Domain.Common;
using JurisSeeker.Domain.Common.Enums;

namespace JurisSeeker.Domain.Entities;

/// <summary>
/// Сырое дело, получаемое напрямую с сайта суда
/// </summary>
public class CourtCase : BaseEntity
{
    /// <summary>
    /// Первичные данные
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Ссылка на дело 
    /// </summary>
    public string Link { get; set; } = null!;
    
    /// <summary>
    /// Номер дела
    /// </summary>
    public string CaseNumber { get; set; } = null!;
    
    /// <summary>
    /// Тип дела
    /// </summary>
    public string CourtType { get; set; } = null!;
    
    /// <summary>
    /// Описание дела
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Ссылка на оригинальный сайт суда
    /// </summary>
    public string OriginalCaseLink { get; set; } = null!;

    /// <summary>
    /// Суть спора (например: "о несостоятельности (банкротстве) физических лиц")
    /// </summary>
    public string Subject { get; set; } = null!;

    /// <summary>
    /// Есть ли решение
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
    /// Тип решения (мотивированное, определение и т.д.)
    /// </summary>
    public string? DecisionType { get; set; }

    /// <summary>
    /// Федеральный округ
    /// </summary>
    public string FederalDistrict { get; set; } = null!;

    /// <summary>
    /// Регион (субъект РФ)
    /// </summary>
    public string Region { get; set; } = null!;
    
    /// <summary>
    /// Истец (отдельное поле)
    /// </summary>
    public string Plaintiff { get; set; } = null!;
    
    /// <summary>
    /// Ответчик (отдельное поле)
    /// </summary>
    public string Defendant { get; set; } = null!;
    
    /// <summary>
    /// Третьи лица (если есть)
    /// </summary>
    public string? ThirdParties { get; set; } 
    
    /// <summary>
    /// Представители (если есть)
    /// </summary>
    public string? Representatives { get; set; } = String.Empty;
    
    /// <summary>
    /// Дата поступления дела
    /// </summary>
    public DateTime? ReceivedDate { get; set; }
    
    /// <summary>
    /// Категория дела
    /// </summary>
    public string CaseCategory { get; set; } = null!;
    
    /// <summary>
    /// Подкатегория дела
    /// </summary>
    public string CaseSubcategory { get; set; } = null!;
    
    /// <summary>
    /// Решение контент
    /// </summary>
    public string? DecisionContent { get; set; }
    
    /// <summary>
    /// Исход дела - кто выиграл
    /// </summary>
    public CaseOutcome Outcome { get; set; } = CaseOutcome.Unknown;
    
    /// <summary>
    /// Результат дела (например: "Заявление ВОЗВРАЩЕНО заявителю")
    /// </summary>
    public string CaseResult { get; set; } = string.Empty;

    /// <summary>
    /// Дата начало 
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Судья
    /// </summary>
    public string? JudgeName { get; set; } = string.Empty;  
    
    /// <summary>
    /// Статьи по делу
    /// </summary>
    public ICollection<CaseArticle> Articles { get; set; } = [];
    
    /// <summary>
    /// Движение дела
    /// </summary>
    public ICollection<CourtCaseMovement> Movements { get; set; } = [];
}