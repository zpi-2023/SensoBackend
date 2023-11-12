using SensoBackend.Application.Modules.Dashboard;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Dashboard;

public sealed class GetDashboardHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetDashboardHandler _sut;

    public GetDashboardHandlerTests() => _sut = new GetDashboardHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnEmptyLists_WhenAccountHasNoGadgets()
    {
        var account = Generators.Account.Generate();
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var result = await _sut.Handle(new GetDashboardRequest(account.Id), CancellationToken.None);

        result.Gadgets.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnGadgetsInCorrectOrder_WhenAccountHasThem()
    {
        var account = Generators.Account.Generate();
        await _context.Accounts.AddAsync(account);
        await _context.DashboardItems.AddRangeAsync(
            Enumerable
                .Range(0, 3)
                .Select(
                    idx =>
                        new DashboardItem
                        {
                            Id = 0,
                            AccountId = account.Id,
                            Gadget = (Gadget)idx,
                            Position = idx
                        }
                )
        );
        await _context.SaveChangesAsync();

        var result = await _sut.Handle(new GetDashboardRequest(account.Id), CancellationToken.None);

        result.Gadgets
            .Should()
            .BeEquivalentTo(Enum.GetValues<Gadget>().Take(3).Select(g => g.ToString("f")));
    }
}
