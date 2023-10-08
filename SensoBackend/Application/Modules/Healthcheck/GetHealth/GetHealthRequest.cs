using MediatR;
using SensoBackend.Application.Modules.Healthcheck.Contracts;

namespace SensoBackend.Application.Modules.Healthcheck.GetHealth;

public sealed record GetHealthRequest : IRequest<HealthcheckDto>;
