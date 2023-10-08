namespace SensoBackend.Application.Modules.Healthcheck.Contracts;

public sealed record HealthcheckDto
{
    public required HealthcheckStatus Server { get; init; }
    public required HealthcheckStatus Database { get; init; }
}
