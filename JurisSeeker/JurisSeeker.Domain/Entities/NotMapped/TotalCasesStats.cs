using System.ComponentModel.DataAnnotations.Schema;
using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities.NotMapped;

/// <summary>
/// Общая статистика по всем делам
/// </summary>
[NotMapped]
public class TotalCasesStats : AnalyticsBase
{
    /// <summary>
    /// Сколько всего дел
    /// </summary>
    public int TotalCases { get; set; }
    
    /// <summary>
    /// Сколько дел с решениями
    /// </summary>
    public int TotalCasesWithDecision { get; set; }
    
    /// <summary>
    /// Процент дел с решениями
    /// </summary>
    public decimal DecisionPercentage { get; set; }
}