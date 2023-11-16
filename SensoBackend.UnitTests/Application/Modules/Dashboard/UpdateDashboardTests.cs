using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Dashboard;
using SensoBackend.Application.Modules.Dashboard.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Dashboard;

public sealed class UpdateDashboardHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly UpdateDashboardHandler _sut;

    public UpdateDashboardHandlerTests() => _sut = new UpdateDashboardHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldClearDashboard_WhenPassedAnEmptyList()
    {
        var account = Generators.Account.Generate();
        await _context.Accounts.AddAsync(account);
        await _context
            .DashboardItems
            .AddRangeAsync(
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

        await _sut.Handle(
            new UpdateDashboardRequest
            {
                SeniorId = account.Id,
                Dto = new DashboardDto { Gadgets = new() }
            },
            CancellationToken.None
        );

        _context.DashboardItems.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReplaceGadgets_WhenPassedACorrectList()
    {
        var account = Generators.Account.Generate();
        var gadgets = new List<string>
        {
            ((Gadget)3).ToString("f"),
            ((Gadget)3).ToString("f"),
            ((Gadget)0).ToString("f")
        };
        await _context.Accounts.AddAsync(account);
        await _context
            .DashboardItems
            .AddRangeAsync(
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

        await _sut.Handle(
            new UpdateDashboardRequest
            {
                SeniorId = account.Id,
                Dto = new DashboardDto { Gadgets = gadgets }
            },
            CancellationToken.None
        );

        _context
            .DashboardItems
            .OrderBy(d => d.Position)
            .Select(d => d.Gadget!.ToString("f"))
            .Should()
            .BeEquivalentTo(gadgets);
    }
}

public sealed class UpdateDashboardValidatorTests
{
    private readonly UpdateDashboardValidator _sut = new();

    [Fact]
    public void Validate_ShouldReturnValid_ForAValidGadgetList()
    {
        var request = new UpdateDashboardRequest
        {
            SeniorId = 0,
            Dto = Generators.DashboardDto.Generate()
        };

        _sut.Validate(request).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnValid_ForEmptyGadgetList()
    {
        var request = new UpdateDashboardRequest
        {
            SeniorId = 0,
            Dto = new DashboardDto { Gadgets = new() }
        };

        _sut.Validate(request).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnValid_WhenPassedDuplicateGadgets()
    {
        var request = new UpdateDashboardRequest
        {
            SeniorId = 0,
            Dto = new DashboardDto
            {
                Gadgets = new() { ((Gadget)0).ToString("f"), ((Gadget)0).ToString("f") }
            }
        };

        _sut.Validate(request).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_ForTooLongLists()
    {
        var request = new UpdateDashboardRequest
        {
            SeniorId = 0,
            Dto = new DashboardDto
            {
                Gadgets = Enumerable.Range(0, 7).Select(_ => ((Gadget)0).ToString("f")).ToList()
            }
        };

        _sut.Validate(request).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_ForInvalidGadgetNames()
    {
        var request = new UpdateDashboardRequest
        {
            SeniorId = 0,
            Dto = new DashboardDto { Gadgets = new() { "invalid" } }
        };

        _sut.Validate(request).IsValid.Should().BeFalse();
    }
}
