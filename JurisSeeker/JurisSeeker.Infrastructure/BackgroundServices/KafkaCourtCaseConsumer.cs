using System.Text.Json;
using Confluent.Kafka;
using JurisSeeker.Core.Dto.Messages;
using JurisSeeker.Core.Interfaces;
using JurisSeeker.Domain.Common.Enums;
using JurisSeeker.Domain.ElasticEntity;
using JurisSeeker.Domain.Entities;
using JurisSeeker.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JurisSeeker.Infrastructure.BackgroundServices;

public class KafkaCourtCaseConsumer : BackgroundService
{
    private readonly ILogger<KafkaCourtCaseConsumer> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private readonly IConsumer<string, string> _consumer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceProvider _serviceProvider; 
    
    public KafkaCourtCaseConsumer(
        ILogger<KafkaCourtCaseConsumer> logger,
        IOptions<KafkaOptions> kafkaOptions,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _kafkaOptions = kafkaOptions.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
            .Build();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer started. Listening to topic: {Topic}", _kafkaOptions.Topic);

        _consumer.Subscribe(_kafkaOptions.Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null)
                        continue;

                    var message = JsonSerializer.Deserialize<CourtCaseMessage>(result.Message.Value, _jsonOptions);

                    if (message == null)
                    {
                        _logger.LogWarning("Не удалось десериализовать сообщение: {Json}", result.Message.Value);
                        continue;
                    }

                    _logger.LogInformation("Получено сообщение: {CaseNumber} ({Region})", message.CaseNumber, message.Region);

                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<IDbContext>();
                    var elastic = scope.ServiceProvider.GetRequiredService<IElasticsearchContext>();

                    var id = await SaveCaseToDatabaseAsync(db, message, stoppingToken);
                    if (id == Guid.Empty) continue;
                    
                    await IndexToElasticAsync(elastic, id, message, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Ошибка при чтении сообщения из Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Неожиданная ошибка при обработке сообщения");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer остановлен");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private async Task<Guid> SaveCaseToDatabaseAsync(IDbContext db, CourtCaseMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await db.CourtCases.AnyAsync(x => x.CaseNumber == message.CaseNumber, cancellationToken);
        
            if (exists)
            {
                _logger.LogWarning("Дело с номером {CaseNumber} уже существует", message.CaseNumber);
                return Guid.Empty;
            }
        
            var courtCase = new CourtCase
            {
                Title = message.Title,
                Link = message.Link,
                CaseNumber = message.CaseNumber,
                CourtType = message.CourtType,
                Description = message.Description,
                OriginalCaseLink = message.OriginalCaseLink,
                Subject = message.Subject,
                HasDecision = message.HasDecision,
                DecisionLink = message.DecisionLink,
                DecisionDate = message.DecisionDate,
                DecisionType = message.DecisionType,
                FederalDistrict = message.FederalDistrict,
                Region = message.Region,
                Plaintiff = message.Plaintiff,
                Defendant = message.Defendant,
                ThirdParties = message.ThirdParties,
                Representatives = message.Representatives,
                CaseResult = message.CaseResult,
                ReceivedDate = message.ReceivedDate,
                CaseCategory = message.CaseCategory,
                CaseSubcategory = message.CaseSubcategory,
                Outcome = CaseOutcome.Unknown,
                Articles = [],
                DecisionContent = null,
                StartDate = message.StartDate,
                JudgeName = message.JudgeName,
                Movements = message.CaseMovements
            };
        
            await db.CourtCases.AddAsync(courtCase, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Дело {CaseNumber} успешно сохранено в БД", message.CaseNumber);
            return courtCase.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении дела {CaseNumber} в БД", message.CaseNumber);
            return Guid.Empty;
        }
    }
    
    private async Task IndexToElasticAsync(
        IElasticsearchContext elastic,
        Guid id,
        CourtCaseMessage message,
        CancellationToken token)
    {
        var doc = new CourtDecisionDocument
        {
            CourtCaseId = id,
            CaseNumber = message.CaseNumber,
            Region = message.Region,
            DecisionDate = message.DecisionDate,
            DecisionType = message.DecisionType,
            DecisionContent = message.DecisionContent ?? string.Empty,
            Link = message.Link,
            CourtType = message.CourtType,
            Description = message.Description,
            OriginalCaseLink = message.OriginalCaseLink,
            Subject = message.Subject, HasDecision = message.HasDecision,
            DecisionLink = message.DecisionLink,
            FederalDistrict = message.FederalDistrict,
            Plaintiff = message.Plaintiff,
            Defendant = message.Defendant,
            ThirdParties = message.ThirdParties,
            Representatives = message.Representatives,
            JudgeName = message.JudgeName,
            ReceivedDate = message.ReceivedDate,
            StartDate = message.StartDate,
            CaseCategory = message.CaseCategory,
            CaseSubcategory = message.CaseSubcategory,
            Outcome = CaseOutcome.Unknown,
            CaseResult = message.CaseResult,
            Articles = [],
            Movements = message.CaseMovements.Select(x => new ElasticCourtCaseMovement
            {
                EventName = x.EventName,
                EventResult = x.EventResult,
                Basis = x.Basis,
                EventDate = x.EventDate
            }).ToList(),
            Timestamp = message.Timestamp
        };
        
        await elastic.IndexCourtDecisionDocumentAsync(doc, token);

        _logger.LogInformation("Документ дела {CaseNumber} отправлен в Elasticsearch", doc.CaseNumber);
    }
}