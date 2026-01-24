using Elastic.Clients.Elasticsearch;
using JurisSeeker.Application.Interfaces;
using JurisSeeker.Infrastructure.Elastic;
using JurisSeeker.Infrastructure.GraphQL.Queries;
using JurisSeeker.Infrastructure.Services.Auth;
using JurisSeeker.Shared.GraphQL;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JurisSeeker.Infrastructure;

public static class Entry
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IJwtService, JwtService>();
        
        services.AddGraphQl();
        services.AddElasticSearch(configuration);
    }
    
    private static void AddGraphQl(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType(m => m.Name(OperationTypeNames.Query))
            //.AddMutationType(m => m.Name(OperationTypeNames.Mutation))
            .AddType<UserTypes>()
            .AddTypeExtension<UserQuery>()
            .AddTypeExtension<CourtCaseQuery>()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddErrorFilter(error => error.WithMessage(error.Exception?.Message ?? "Unknown error"))
            .BindRuntimeType<Guid, IdType>()
            .ModifyRequestOptions(opt =>
            {
                opt.IncludeExceptionDetails = true;
                opt.ExecutionTimeout = TimeSpan.FromMinutes(2);
            });
    }

    private static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Elasticsearch");
        services.Configure<ElasticSearchOptions>(section);

        var options = section.Get<ElasticSearchOptions>()!;
        
        services.AddSingleton<ElasticsearchClient>(_ =>
        {
            var settings = new ElasticsearchClientSettings(new
                    Uri(options.Url))
                .DefaultIndex(options.DefaultIndex)
                .EnableDebugMode()
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            return new ElasticsearchClient(settings);
        });

        services.AddScoped<IElasticsearchContext, ElasticsearchContext>();
    }
}