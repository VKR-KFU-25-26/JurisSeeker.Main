using Confluent.Kafka;
using Confluent.Kafka.Admin;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Shared.Kafka.Models;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Kafka.Helpers;

public class KafkaTopicService : IKafkaTopicService
{
    private readonly IAdminClient _adminClient;
    private readonly KafkaOptions _kafkaOptions;
    private readonly ILogger<KafkaTopicService> _logger;
    private readonly HashSet<string> _createdTopics = [];

    public KafkaTopicService(
        IOptions<KafkaOptions> kafkaOptions,
        ILogger<KafkaTopicService> logger)
    {
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger;

        var config = new AdminClientConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers
        };

        _adminClient = new AdminClientBuilder(config).Build();
    }

    public async Task CreateTopicIfNotExistsAsync(string topicName, int partitions = 3, short replicationFactor = 1)
    {
        if (_createdTopics.Contains(topicName))
        {
            _logger.LogDebug("Топик {TopicName} уже создан в текущей сессии", topicName);
            return;
        }

        try
        {
            if (await TopicExistsAsync(topicName))
            {
                _logger.LogInformation("Топик {TopicName} уже существует", topicName);
                _createdTopics.Add(topicName);
                return;
            }

            var topicSpec = new TopicSpecification
            {
                Name = topicName,
                NumPartitions = partitions,
                ReplicationFactor = replicationFactor,
                Configs = new Dictionary<string, string>
                {
                    ["retention.ms"] = "604800000" // 7 дней retention по умолчанию
                }
            };

            await _adminClient.CreateTopicsAsync([topicSpec]);
            
            _createdTopics.Add(topicName);
            _logger.LogInformation("Топик {TopicName} создан успешно (partitions: {Partitions}, replication: {Replication})", 
                topicName, partitions, replicationFactor);
        }
        catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
        {
            _createdTopics.Add(topicName);
            _logger.LogInformation("Топик {TopicName} уже существует", topicName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания топика {TopicName}", topicName);
            throw;
        }
    }
    
    public Task<bool> TopicExistsAsync(string topicName)
    {
        try
        {
            var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            return Task.FromResult(metadata.Topics.Any(t => t.Topic == topicName && t.Error.Code == ErrorCode.NoError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка проверки топика {TopicName}", topicName);
            return Task.FromResult(false);
        }
    }

    public async Task EnsureRequiredTopicsAsync()
    {
        var requiredTopics = new[]
        {
            KafkaTopicConfig.CourtCasesTopic,
            KafkaTopicConfig.MLTopic
        };

        _logger.LogInformation("Проверка обязательных топиков Kafka...");

        foreach (var topic in requiredTopics)
        {
            await CreateTopicIfNotExistsAsync(topic.Name, topic.Partitions, topic.ReplicationFactor);
        }

        _logger.LogInformation("Все обязательные топики проверены и созданы при необходимости");
    }

    public void Dispose()
    {
        _adminClient.Dispose();
    }
}