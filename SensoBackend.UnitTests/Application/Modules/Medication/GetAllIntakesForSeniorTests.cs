using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetAllIntakesForSeniorTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetAllIntakesForSeniorHandler _sut;

    public GetAllIntakesForSeniorTests() => _sut = new GetAllIntakesForSeniorHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnAllIntakes_WhenSeniorTriesToGetThem()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var intake = await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);

        var result = await _sut.Handle(
            new GetAllIntakesForSeniorRequest
            {
                AccountId = senior.Id,
                SeniorId = senior.Id,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllIntakes_WhenCaretakerTriesToGetThem()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(caretaker, senior, medication);

        var intake = await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);

        var result = await _sut.Handle(
            new GetAllIntakesForSeniorRequest
            {
                AccountId = caretaker.Id,
                SeniorId = senior.Id,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserTriesToAccessThem()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new GetAllIntakesForSeniorRequest
                {
                    AccountId = account.Id,
                    SeniorId = senior.Id,
                    PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<SeniorReminderAccessDeniedException>();
    }
}
