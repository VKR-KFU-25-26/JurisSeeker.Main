using System.Text.Json;
using Confluent.Kafka;
using JurisSeeker.Application.Dto.Messages;
using JurisSeeker.Application.Features.SaveFromMlService;
using JurisSeeker.Shared.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Kafka.KafkaMessaging.Consumers;

/// <summary>
/// Консьюмер, принимающий из ml сервиса
/// </summary>
public class KafkaMlResultConsumer : BackgroundService
{
    private readonly ILogger<KafkaCourtCaseConsumer> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private readonly IConsumer<string, string> _consumer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceProvider _serviceProvider; 
    
    public KafkaMlResultConsumer(
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
            GroupId = _kafkaOptions.MlGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("JurisSeeker.Kafka error: {Reason}", e.Reason))
            .Build();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        _logger.LogInformation("JurisSeeker.Kafka consumer started. Listening to topic: {Topic}", _kafkaOptions.FromMlTopic);

        _consumer.Subscribe(_kafkaOptions.FromMlTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null)
                        continue;

                    var message = JsonSerializer.Deserialize<MlProcessingResult>(result.Message.Value, _jsonOptions);

                    if (message == null)
                    {
                        _logger.LogWarning("Не удалось десериализовать сообщение: {Json}", result.Message.Value);
                        continue;
                    }
                    
                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new SaveResultFromMlServiceCommand(message), stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Ошибка при чтении сообщения из JurisSeeker.Kafka");
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