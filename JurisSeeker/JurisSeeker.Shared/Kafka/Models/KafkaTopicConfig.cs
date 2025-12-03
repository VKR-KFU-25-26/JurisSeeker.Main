
namespace JurisSeeker.Shared.Kafka.Models;

public class KafkaTopicConfig
{
    public string Name { get; set; } = string.Empty;
    public int Partitions { get; set; } = 3;
    public short ReplicationFactor { get; set; } = 1;

    public static KafkaTopicConfig CourtCasesTopic => new()
    {
        Name = "court-cases",
        Partitions = 3
    };

    public static KafkaTopicConfig MLTopic => new()
    {
        Name = "court-cases-ml", 
        Partitions = 3
    };
}