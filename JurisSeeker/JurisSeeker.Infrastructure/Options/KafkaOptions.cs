namespace JurisSeeker.Infrastructure.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "court-cases-topic";
    public string GroupId { get; set; } = "court-decisions-group";
}