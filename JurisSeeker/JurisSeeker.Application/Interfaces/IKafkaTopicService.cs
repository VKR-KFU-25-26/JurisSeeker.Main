namespace JurisSeeker.Application.Interfaces;

public interface IKafkaTopicService : IDisposable
{
    Task CreateTopicIfNotExistsAsync(string topicName, int partitions = 3, short replicationFactor = 1);
    Task<bool> TopicExistsAsync(string topicName);
    Task EnsureRequiredTopicsAsync();
}