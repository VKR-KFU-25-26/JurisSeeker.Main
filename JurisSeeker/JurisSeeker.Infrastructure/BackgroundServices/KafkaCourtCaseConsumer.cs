using System.Text.Json;
using Confluent.Kafka;
using JurisSeeker.Core.Dto.Messages;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Infrastructure.BackgroundServices;

public class KafkaCourtCaseConsumer : BackgroundService
{
    private readonly ILogger<KafkaCourtCaseConsumer> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private readonly IConsumer<string, string> _consumer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceProvider _serviceProvider; 


    public KafkaCourtCaseConsumer(
        ILogger<KafkaCourtCaseConsumer> logger,
        IOptions<KafkaOptions> kafkaOptions,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _kafkaOptions = kafkaOptions.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
            .Build();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer started. Listening to topic: {Topic}", _kafkaOptions.Topic);

        _consumer.Subscribe(_kafkaOptions.Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null)
                        continue;

                    var json = result.Message.Value;
                    var message = JsonSerializer.Deserialize<CourtCaseMessage>(json, _jsonOptions);

                    if (message == null)
                    {
                        _logger.LogWarning("Не удалось десериализовать сообщение: {Json}", json);
                        continue;
                    }
                
                    _logger.LogInformation("Получено сообщение: {CaseNumber} ({Region})", message.CaseNumber,
                        message.Region);

                    using var scope = _serviceProvider.CreateScope();
                    var processingService = scope.ServiceProvider.GetRequiredService<ICourtCaseProcessingService>();

                    var success = await processingService.ProcessCourtCaseAsync(message, stoppingToken);

                    if (success)
                    {
                        _consumer.Commit(result);
                        _logger.LogDebug("Офсет закоммичен для дела {CaseNumber}", message.CaseNumber);
                    }
                    else
                    {
                        _logger.LogWarning("Сообщение для дела {CaseNumber} не было обработано", message.CaseNumber);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Ошибка при чтении сообщения из Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Неожиданная ошибка при обработке сообщения");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer остановлен");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}