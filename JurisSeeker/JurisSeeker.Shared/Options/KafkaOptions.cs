namespace JurisSeeker.Shared.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "court-cases-topic";
    public string GroupId { get; set; } = "court-decisions-group";
    
    public string MlTopic { get; set; } = "court-cases-ml";
    public string FromMlTopic { get; set; } = "court-cases-ml-result";
    public string MlGroupId { get; set; } = "ml-service-group";
}