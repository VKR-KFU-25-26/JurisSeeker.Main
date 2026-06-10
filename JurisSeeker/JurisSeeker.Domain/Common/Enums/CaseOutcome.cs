namespace JurisSeeker.Domain.Common.Enums;

/// <summary>
/// Тип исхода дела
/// </summary>
public enum CaseOutcome
{
    /// <summary>
    /// Неизвестно
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Истец выиграл
    /// </summary>
    PlaintiffWon = 1,
    
    /// <summary>
    /// Ответчик выиграл
    /// </summary>
    DefendantWon = 2
}