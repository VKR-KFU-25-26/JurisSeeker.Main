using JurisSeeker.Application.Dto.Messages;
using MediatR;

namespace JurisSeeker.Application.Features.CourtCase.Command;

public record ProcessCourtCaseCommand(CourtCaseMessage Message) : IRequest<Guid>;
