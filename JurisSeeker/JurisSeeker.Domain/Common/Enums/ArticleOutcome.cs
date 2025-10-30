namespace JurisSeeker.Domain.Common.Enums;

/// <summary>
/// Исход по конкретной статье
/// </summary>
public enum ArticleOutcome
{
    /// <summary>
    /// Неизвестно
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Удовлетворено (в пользу истца)
    /// </summary>
    Satisfied = 1,
    
    /// <summary>
    /// Отказано (в пользу ответчика)
    /// </summary>
    Rejected = 2,
    
    /// <summary>
    /// Частично удовлетворено
    /// </summary>
    PartiallySatisfied = 3,
    
    /// <summary>
    /// Статья не применялась
    /// </summary>
    NotApplied = 4
}