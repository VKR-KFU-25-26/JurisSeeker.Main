using JurisSeeker.Application.Interfaces;
using JurisSeeker.Domain.Entities.NotMapped;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JurisSeeker.Infrastructure.Services.Statistic;

public class StatisticUpdatedWorker(IServiceProvider services, ILogger<StatisticUpdatedWorker> logger)
    : BackgroundService
{
    private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(15);
    private bool _isFirstRun = true;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_isFirstRun)
        {
            var initialDelay = TimeSpan.FromMinutes(2);
            logger.LogInformation("First run, waiting {Delay} minutes for application to fully initialize", 
                initialDelay.TotalMinutes);
            await Task.Delay(initialDelay, stoppingToken);
            _isFirstRun = false;
        }
        
        logger.LogInformation("Statistic Update Worker started. Interval: {Interval} hours", _refreshInterval.TotalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateAllStatisticsAsync(stoppingToken);
                await Task.Delay(_refreshInterval, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error updating statistics. Retry in 5 minutes");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
    
    private async Task UpdateAllStatisticsAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDbContext>();

        var viewTypes = new[]
        {
            typeof(TotalCasesStats),
            typeof(CasesByRegionStats),
            typeof(DecisionStats),
            typeof(OutcomeStats)
        };

        foreach (var viewType  in viewTypes)
        {
            var viewName = $"\"{viewType.Name}\"";
            logger.LogDebug("Refreshing view: {ViewName}", viewName);
            await context.ExecuteSqlRawAsync($"REFRESH MATERIALIZED VIEW {viewName};", cancellationToken);
        }
    }
}