namespace JurisSeeker.Domain.Common;

/// <summary>
/// Базовые поля сущности
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Id сущности
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата и время последнего обновления записи
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}