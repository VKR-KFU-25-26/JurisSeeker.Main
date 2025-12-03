using JurisSeeker.Application.Dto.Messages;
using MediatR;

namespace JurisSeeker.Application.Features.CourtDecisionInMlService;

public record ProcessCourtDecisionCommand(CourtDecisionMessage Message) : IRequest<bool>;