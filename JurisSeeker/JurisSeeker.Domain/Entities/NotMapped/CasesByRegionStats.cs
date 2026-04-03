using System.ComponentModel.DataAnnotations.Schema;
using JurisSeeker.Domain.Common;

namespace JurisSeeker.Domain.Entities.NotMapped;

/// <summary>
/// Статистика по регионам - сколько дел в каждом
/// </summary>
[NotMapped]
public class CasesByRegionStats : AnalyticsBase
{
    public string Region { get; set; } = null!;
    public string FederalDistrict { get; set; } = null!;
    
    /// <summary>
    /// Количество дел в регионе
    /// </summary>
    public int CasesInRegion { get; set; }
    
    /// <summary>
    /// Процент от общего числа дел
    /// </summary>
    public decimal PercentageOfTotal { get; set; }
}