namespace JurisSeeker.Application.Interfaces;

/// <summary>
/// Интерфейс для создания топиков кафки
/// </summary>
public interface IKafkaTopicService : IDisposable
{
    Task CreateTopicIfNotExistsAsync(string topicName, int partitions = 3, short replicationFactor = 1);
}