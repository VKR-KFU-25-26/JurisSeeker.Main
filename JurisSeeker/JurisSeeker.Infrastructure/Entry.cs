using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Infrastructure.BackgroundServices;
using JurisSeeker.Infrastructure.Options;
using JurisSeeker.Infrastructure.Services;
using JurisSeeker.Infrastructure.Services.Elastic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JurisSeeker.Infrastructure;

public static class Entry
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICourtCaseProcessingService, CourtCaseProcessingService>();
        
        services.AddKafka(configuration);
        //services.AddElasticSearch(configuration);
    }

    private static void AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection("KafkaOptions"));
        
        services.AddHostedService<KafkaCourtCaseConsumer>();
    }

    private static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticSearchOptions>(configuration.GetSection("ElasticSearchOptions"));
        
        var section = configuration.GetSection("Elasticsearch");
        services.Configure<ElasticSearchOptions>(section);

        var options = section.Get<ElasticSearchOptions>()!;

        var settings = new ElasticsearchClientSettings(new Uri(options.Url))
            .Authentication(new BasicAuthentication(options.Username, options.Password))
            .DefaultIndex(options.IndexName)
            .PrettyJson();

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
        
        services.AddScoped<ElasticSetupService>();
        services.AddScoped<IElasticCourtCaseService, ElasticCourtCaseService>();
    }
}