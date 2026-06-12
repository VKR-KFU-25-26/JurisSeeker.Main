using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities;

/// <summary>
/// Модель для представления события движения судебного дела
/// Содержит информацию о каждом этапе процесса рассмотрения дела в суде
/// </summary>
public class CourtCaseMovement : BaseEntity
{
    /// <summary>
    /// Наименование процессуального события или действия
    /// </summary>
    public string? EventName { get; set; } = "";
    
    /// <summary>
    /// Результат или итог процессуального события
    /// </summary>
    public string? EventResult { get; set; } = "";
    
    /// <summary>
    /// Правовые основания или нормативные акты, на которых основано событие
    /// </summary>
    public string? Basis { get; set; } = "";
    
    /// <summary>
    /// Дата совершения процессуального события
    /// Является ключевой информацией для построения хронологии дела
    /// </summary>
    public DateTime? EventDate { get; set; }
    
    /// <summary>
    /// Id дела
    /// </summary>
    public Guid CourtCaseId { get; set; }
    
    /// <summary>
    /// Дело
    /// </summary>
    public CourtCase CourtCase { get; set; } = null!;
}