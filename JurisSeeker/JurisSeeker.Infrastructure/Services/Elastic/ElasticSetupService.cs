using Elastic.Clients.Elasticsearch;
using JurisSeeker.Domain.ElasticEntity;
using JurisSeeker.Domain.Entities;
using JurisSeeker.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Infrastructure.Services.Elastic;

public class ElasticSetupService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSearchOptions _options;
    private readonly ILogger<ElasticSetupService> _logger;

    public ElasticSetupService(ElasticsearchClient client, IOptions<ElasticSearchOptions> options, ILogger<ElasticSetupService> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }
    
    public async Task EnsureIndexExistsAsync(CancellationToken cancellationToken = default)
    {
        var index = _options.IndexName;

        var existsResponse = await _client.Indices.ExistsAsync(index, cancellationToken);
        if (existsResponse.Exists)
        {
            _logger.LogInformation("Индекс {IndexName} уже существует", index);
            return;
        }

        var createResponse = await _client.Indices.CreateAsync(index, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
            )
            .Mappings(m => m
                .Properties<CourtDecisionDocument>(p => p
                    .Keyword(k => k.CourtCaseId)
                    .Keyword(k => k.CaseNumber)
                    .Keyword(k => k.Region)
                    .Date(k => k.DecisionDate)
                    .Keyword(k => k.DecisionType)
                    .Text(k => k.DecisionText)
                )
            ), cancellationToken);

        if (!createResponse.IsValidResponse)
        {
            _logger.LogError("Ошибка при создании индекса {IndexName}: {Error}", index, createResponse.DebugInformation);
            throw new InvalidOperationException($"Ошибка создания индекса {index}");
        }

        _logger.LogInformation("Индекс {IndexName} успешно создан", index);
    }
}