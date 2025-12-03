using JurisSeeker.Application.Dto.Messages;
using MediatR;

namespace JurisSeeker.Application.Features.CourtCaseFromParser.Command;

public record ProcessCourtCaseCommand(CourtCaseMessage Message) : IRequest<Guid>;
