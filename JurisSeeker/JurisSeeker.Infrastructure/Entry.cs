using Elastic.Clients.Elasticsearch;
using JurisSeeker.Infrastructure.GraphQL.Queries;
using JurisSeeker.Infrastructure.Services.Statistic;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JurisSeeker.Infrastructure;

public static class Entry
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<StatisticUpdatedWorker>();
        services.AddGraphQl();
    }
    
    private static void AddGraphQl(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType(m => m.Name(OperationTypeNames.Query))
            //.AddMutationType(m => m.Name(OperationTypeNames.Mutation))
            .AddTypeExtension<CourtCaseQuery>()
            .AddTypeExtension<StatisticQuery>()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddErrorFilter(error => error.WithMessage(error.Exception?.Message ?? "Unknown error"))
            .BindRuntimeType<Guid, IdType>()
            .ModifyRequestOptions(opt =>
            {
                opt.IncludeExceptionDetails = true;
                opt.ExecutionTimeout = TimeSpan.FromMinutes(2);
            })
            .ModifyCostOptions(opt =>
            {
                opt.MaxFieldCost = 10000;
                opt.MaxTypeCost = 10000;
            });
    }
}