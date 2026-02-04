using System.Text.Json;
using Confluent.Kafka;
using JurisSeeker.Application.Dto.Messages;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Kafka.KafkaMessaging.Producers;

public class KafkaCourtMlProducer : IKafkaCourtMlProducer
{
    private readonly ILogger<KafkaCourtMlProducer> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _kafkaOptions;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaCourtMlProducer(
        ILogger<KafkaCourtMlProducer> logger,
        IOptions<KafkaOptions> kafkaOptions
        )
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions.Value;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            MessageTimeoutMs = 5000,
            RequestTimeoutMs = 3000,
            RetryBackoffMs = 1000,
            MessageSendMaxRetries = 3,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Reason}", e.Reason))
            .Build();
    }
    
    public async Task<bool> SendCourtCaseToMlServiceAsync(CourtDecisionMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message, _jsonOptions);
            var topicName = _kafkaOptions.MlTopic;
            
            var kafkaMessage = new Message<string, string>
            {
                Key = message.Id.ToString(),
                Value = jsonMessage,
                Headers = new Headers
                {
                    { "message-type", "court-case-ml"u8.ToArray() },
                    { "timestamp", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
                }
            };

            var deliveryResult = await _producer.ProduceAsync(topicName, kafkaMessage, cancellationToken);
            
            _logger.LogInformation(
                "Сообщение отправлено в ML сервис. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}", 
                deliveryResult.Topic, 
                deliveryResult.Partition.Value, 
                deliveryResult.Offset.Value
            );

            return deliveryResult.Status == PersistenceStatus.Persisted;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, 
                "Ошибка при отправке сообщения в ML сервис. Error: {ErrorCode}, Reason: {Reason}", 
                ex.Error.Code, 
                ex.Error.Reason
            );
            return false;
        }
        catch (KafkaException ex)
        {
            _logger.LogError(ex, "Kafka ошибка при отправке сообщения в ML сервис");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при отправке сообщения в ML сервис");
            return false;
        }
    }
    
    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
            _logger.LogInformation("Kafka producer disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при dispose Kafka producer");
        }
    }
}