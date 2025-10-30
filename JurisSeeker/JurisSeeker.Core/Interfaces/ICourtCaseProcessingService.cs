using JurisSeeker.Core.Dto.Messages;

namespace JurisSeeker.Core.Interfaces;

public interface ICourtCaseProcessingService
{
    Task<bool> ProcessCourtCaseAsync(CourtCaseMessage message, CancellationToken cancellationToken = default);
}