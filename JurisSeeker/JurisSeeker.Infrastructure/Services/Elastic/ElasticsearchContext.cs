using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.ElasticEntity;
using JurisSeeker.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Infrastructure.Services.Elastic;

/// <inheritdoc cref="IElasticsearchContext"/> 
public class ElasticsearchContext : IElasticsearchContext
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSearchOptions _options;
    private readonly ILogger<ElasticsearchContext> _logger;
    private readonly string _index;

    public ElasticsearchContext(
        ElasticsearchClient client,
        IOptions<ElasticSearchOptions> options,
        ILogger<ElasticsearchContext> logger
        )
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
        _index = _options.DefaultIndex;
    }
    
    /// <inheritdoc />
    public async Task IndexCourtDecisionDocumentAsync(CourtDecisionDocument document, CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexAsync(document, 
            i => i
                .Index(_index)
                .Id(document.CourtCaseId.ToString()),
            cancellationToken
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Ошибка индексации документа дела {CaseNumber}: {Error}", 
                document.CaseNumber, response.DebugInformation);
        }
    }

    /// <inheritdoc />
    public async Task BulkIndexCourtDecisionDocumentsAsync(
        List<CourtDecisionDocument> documents, 
        CancellationToken cancellationToken = default)
    {
        var bulkRequest = new BulkRequest(_index)
        {
            Operations = new List<IBulkOperation>()
        };

        foreach (var document in documents)
        {
            bulkRequest.Operations.Add(new BulkIndexOperation<CourtDecisionDocument>(document)
            {
                Id = document.CourtCaseId.ToString()
            });
        }

        var response = await _client.BulkAsync(bulkRequest, cancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError("Ошибка массовой индексации документов: {Error}", response.DebugInformation);
        }
        else
        {
            _logger.LogInformation("Успешно проиндексировано {Count} документов в Elasticsearch", documents.Count);
        }
    }
    
    /// <inheritdoc/>
    public async Task<List<CourtDecisionDocument>> GetAllCourtDecisionsAsync(CancellationToken cancellationToken = default)
    {
        var searchResponse = await _client.SearchAsync<CourtDecisionDocument>(s => s
                .Index(_index)
                .Query(q => q.MatchAll(new MatchAllQuery()))
                .Size(1000), // можно увеличить, если мало данных
            cancellationToken);

        if (!searchResponse.IsValidResponse)
        {
            _logger.LogError("Ошибка при получении всех документов: {Error}", searchResponse.DebugInformation);
            return [];
        }

        return searchResponse.Hits.Select(h => h.Source!).ToList();
    }
    
    /// <inheritdoc/>
    public async Task<CourtDecisionDocument?> GetCourtDecisionAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync<CourtDecisionDocument>(
            id.ToString(), 
            g => g.Index(_index), 
            cancellationToken);

        return !response.Found ? null : response.Source;
    }
}