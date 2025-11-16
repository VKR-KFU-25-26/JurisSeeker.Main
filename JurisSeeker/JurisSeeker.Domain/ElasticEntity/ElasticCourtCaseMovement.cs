
namespace JurisSeeker.Domain.ElasticEntity;

public class ElasticCourtCaseMovement
{
    /// <summary>
    /// Наименование события
    /// </summary>
    public string? EventName { get; set; } = string.Empty;

    /// <summary>
    /// Результат события
    /// </summary>
    public string? EventResult { get; set; } = string.Empty;

    /// <summary>
    /// Правовые основания
    /// </summary>
    public string? Basis { get; set; } = string.Empty;

    /// <summary>
    /// Дата события
    /// </summary>
    public DateTime? EventDate { get; set; }
}