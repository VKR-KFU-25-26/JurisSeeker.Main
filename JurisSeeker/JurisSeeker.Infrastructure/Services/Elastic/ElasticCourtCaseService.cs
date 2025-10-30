using Elastic.Clients.Elasticsearch;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.Entities;
using JurisSeeker.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Infrastructure.Services.Elastic;

public class ElasticCourtCaseService : IElasticCourtCaseService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSearchOptions _options;
    private readonly ILogger<ElasticCourtCaseService> _logger;

    public ElasticCourtCaseService(
        ElasticsearchClient client,
        IOptions<ElasticSearchOptions> options,
        ILogger<ElasticCourtCaseService> logger
        )
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }
    
    public async Task IndexCourtCaseAsync(CourtCase entity, CancellationToken cancellationToken = default)
    {
        var index = _options.IndexName;

        var response = await _client.IndexAsync(entity, i => i
            .Index(index)
            .Id(entity.Id.ToString()), cancellationToken);

        if (!response.IsValidResponse)
            _logger.LogError("Ошибка индексации дела {CaseNumber}: {Error}", entity.CaseNumber, response.DebugInformation);
        else
            _logger.LogInformation("Дело {CaseNumber} проиндексировано в Elasticsearch", entity.CaseNumber);
    }
}