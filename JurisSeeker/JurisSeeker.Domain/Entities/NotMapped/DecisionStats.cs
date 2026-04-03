using System.ComponentModel.DataAnnotations.Schema;
using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities.NotMapped;

/// <summary>
/// Статистика по решениям - сколько дел с решениями по регионам
/// </summary>
[NotMapped]
public class DecisionStats : AnalyticsBase
{
    public string FederalDistrict { get; set; } = null!;
    public string Region { get; set; } = null!;
    
    /// <summary>
    /// Всего дел в регионе
    /// </summary>
    public int TotalCases { get; set; }
    
    /// <summary>
    /// Дела с решениями
    /// </summary>
    public int CasesWithDecision { get; set; }
    
    /// <summary>
    /// Процент дел с решениями
    /// </summary>
    public decimal DecisionRate { get; set; }
}