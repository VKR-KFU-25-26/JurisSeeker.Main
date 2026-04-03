namespace JurisSeeker.Domain.Common;

/// <summary>
/// Базовая модель для аналитики 
/// </summary>
public class AnalyticsBase
{
    /// <summary>
    /// Дата расчета аналитики (можно добавить при обновлении)
    /// </summary>
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}