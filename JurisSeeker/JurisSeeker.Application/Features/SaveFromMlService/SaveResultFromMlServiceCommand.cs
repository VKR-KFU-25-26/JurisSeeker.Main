using JurisSeeker.Application.Dto.Messages;
using MediatR;

namespace JurisSeeker.Application.Features.SaveFromMlService;

public record SaveResultFromMlServiceCommand(MlProcessingResult MlProcessingResult) : IRequest;

    
