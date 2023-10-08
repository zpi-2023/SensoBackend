using Microsoft.EntityFrameworkCore.Infrastructure;
using SensoBackend.Application.Modules.Healthcheck.Contracts;
using SensoBackend.Application.Modules.Healthcheck.GetHealth;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Tests.Application.Modules.Healthcheck.GetHealth;

public sealed class GetHealthHandlerTests
{
    private readonly DatabaseFacade _db;
    private readonly GetHealthHandler _sut;

    public GetHealthHandlerTests()
    {
        var context = Substitute.For<AppDbContext>();
        _db = Substitute.For<DatabaseFacade>(context);
        context.Database.Returns(_db);
        _sut = new GetHealthHandler(context);
    }

    [Fact]
    public async Task Handle_ShouldReturnDatabaseOk_WhenDatabaseIsHealthy()
    {
        _db.CanConnectAsync(CancellationToken.None).Returns(true);

        var response = await _sut.Handle(new GetHealthRequest(), CancellationToken.None);

        response.Server.Should().Be(HealthcheckStatus.Ok);
        response.Database.Should().Be(HealthcheckStatus.Ok);
    }

    [Fact]
    public async Task Handle_ShouldReturnDatabaseUnhealthy_WhenDatabaseIsHealthy()
    {
        _db.CanConnectAsync(CancellationToken.None).Returns(false);

        var response = await _sut.Handle(new GetHealthRequest(), CancellationToken.None);

        response.Server.Should().Be(HealthcheckStatus.Ok);
        response.Database.Should().Be(HealthcheckStatus.Unhealthy);
    }
}
