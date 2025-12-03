using JurisSeeker.Application.Interfaces;
using JurisSeeker.Kafka.Helpers;
using JurisSeeker.Kafka.KafkaMessaging.Consumers;
using JurisSeeker.Kafka.KafkaMessaging.Producers;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JurisSeeker.Kafka;

public static class Entry
{
    public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection("KafkaOptions"));

        services
            .AddScoped<IKafkaTopicService, KafkaTopicService>()
            .AddScoped<IKafkaCourtMlProducer, KafkaCourtMlProducer>();
        
        services.AddHostedService<KafkaCourtCaseConsumer>();
        services.AddHostedService<KafkaMlResultConsumer>();
    }
}