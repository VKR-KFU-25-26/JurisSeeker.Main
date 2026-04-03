using HotChocolate;
using JurisSeeker.Domain.Entities.NotMapped;

namespace JurisSeeker.Application.Dto.Analytics;


/// <summary>
/// Ответ для дашборда аналитики
/// </summary>
[GraphQLDescription("Сводная аналитика для дашборда")]
public class DashboardAnalyticsResponse
{
    /// <summary>
    /// Общая статистика
    /// </summary>
    [GraphQLDescription("Общая статистика по всем делам")]
    public TotalCasesStats? TotalStats { get; set; }

    /// <summary>
    /// Статистика по регионам
    /// </summary>
    [GraphQLDescription("Статистика по регионам - количество дел")]
    public List<CasesByRegionStats> RegionStats { get; set; } = new();

    /// <summary>
    /// Статистика по решениям
    /// </summary>
    [GraphQLDescription("Статистика по решениям - процент дел с решениями")]
    public List<DecisionStats> DecisionStats { get; set; } = new();

    /// <summary>
    /// Статистика по исходам
    /// </summary>
    [GraphQLDescription("Статистика по исходам - кто выигрывает")]
    public List<OutcomeStats> OutcomeStats { get; set; } = new();

    /// <summary>
    /// Дата и время расчета
    /// </summary>
    [GraphQLDescription("Дата и время расчета аналитики")]
    public DateTime CalculatedAt { get; set; }
}