using JurisSeeker.Domain.Entities;

namespace JurisSeeker.Core.Interfaces;

public interface IElasticCourtCaseService
{
    Task IndexCourtCaseAsync(CourtCase entity, CancellationToken cancellationToken = default);
}