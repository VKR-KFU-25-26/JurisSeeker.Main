using System.ComponentModel.DataAnnotations.Schema;
using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities.NotMapped;

/// <summary>
/// Статистика по Outcome - кто выигрывает
/// </summary>
[NotMapped]
public class OutcomeStats : AnalyticsBase
{
    public string FederalDistrict { get; set; } = null!;
    public string Region { get; set; } = null!;
    
    /// <summary>
    /// Дела с известным исходом (Outcome не 0)
    /// </summary>
    public int CasesWithKnownOutcome { get; set; }
    
    /// <summary>
    /// Победы истцов
    /// </summary>
    public int PlaintiffWins { get; set; }
    public decimal PlaintiffWinPercentage { get; set; }
    
    /// <summary>
    /// Победы ответчиков
    /// </summary>
    public int DefendantWins { get; set; }
    public decimal DefendantWinPercentage { get; set; }
    
    /// <summary>
    /// Мировые соглашения
    /// </summary>
    public int Settlements { get; set; }
    public decimal SettlementPercentage { get; set; }
    
    /// <summary>
    /// Прекращенные дела
    /// </summary>
    public int Dismissed { get; set; }
    public decimal DismissedPercentage { get; set; }
}