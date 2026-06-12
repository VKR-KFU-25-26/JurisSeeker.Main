using JurisSeeker.Application.Interfaces;
using JurisSeeker.Persistence.EfContext;
using JurisSeeker.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Persistence.Helpers;

/// <summary>
/// Мигратор, для миграций в базу
/// </summary>
public class Migrator(
    ApplicationDbContext context,
    IKafkaTopicService topicService,
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<Migrator> logger)
{
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value; 

    public async Task MigrateAsync()
    {
        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);
            await topicService.CreateTopicIfNotExistsAsync(_kafkaOptions.MlTopic);
            await topicService.CreateTopicIfNotExistsAsync(_kafkaOptions.FromMlTopic);
            
            logger.LogInformation("Миграции и создание топиков завершены успешно");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при применении миграций");
            throw;
        }
    }
}