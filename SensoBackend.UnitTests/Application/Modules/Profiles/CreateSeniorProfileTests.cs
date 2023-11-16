using MediatR;
using SensoBackend.Application.Modules.Dashboard;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles;

public sealed class CreateSeniorProfileHandlerTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateSeniorProfileHandler _sut;

    public CreateSeniorProfileHandlerTests() =>
        _sut = new CreateSeniorProfileHandler(_context, _mediator);

    [Fact]
    public async Task Handle_ShouldThrow_WhenSeniorProfileAlreadyExists()
    {
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);

        var action = async () =>
            await _sut.Handle(new CreateSeniorProfileRequest(account.Id), CancellationToken.None);

        await action.Should().ThrowAsync<SeniorProfileAlreadyExistsException>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProfile_WhenSeniorProfileDoesNotExist()
    {
        var account = await _context.SetUpAccount();

        await _sut.Handle(new CreateSeniorProfileRequest(account.Id), CancellationToken.None);

        _context
            .Profiles
            .Any(p => p.AccountId == account.Id && p.SeniorId == account.Id)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenSeniorProfileDoesNotExist()
    {
        var account = await _context.SetUpAccount();

        var dto = await _sut.Handle(
            new CreateSeniorProfileRequest(account.Id),
            CancellationToken.None
        );

        dto.Type.Should().Be("senior");
        dto.SeniorId.Should().Be(account.Id);
        dto.SeniorAlias.Should().Be(account.DisplayName);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDashboard_WhenSeniorProfileDoesNotExist()
    {
        var ct = CancellationToken.None;
        var account = await _context.SetUpAccount();

        await _sut.Handle(new CreateSeniorProfileRequest(account.Id), ct);

        await _mediator.Received().Send(Arg.Any<UpdateDashboardRequest>(), ct);
    }
}
