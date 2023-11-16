using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetSeniorRemindersTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetSeniorRemindersHandler _sut;

    public GetSeniorRemindersTests() => _sut = new GetSeniorRemindersHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnReminder_WhenSeniorWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();

        await _context.SetUpReminder(senior, senior, medication);
        await _context.SetUpReminder(senior, senior, medication);

        var result = await _sut.Handle(
            new GetSeniorRemindersRequest
            {
                AccountId = senior.Id,
                SeniorId = senior.Id,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = 3 }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnReminder_WhenCaretakerWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();

        await _context.SetUpReminder(senior, senior, medication);
        await _context.SetUpReminder(senior, senior, medication);

        var result = await _sut.Handle(
            new GetSeniorRemindersRequest
            {
                AccountId = caretaker.Id,
                SeniorId = senior.Id,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = 3 }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();
        var medication = await _context.SetUpMedication();

        await _context.SetUpReminder(senior, senior, medication);
        await _context.SetUpReminder(senior, senior, medication);

        var action = async () =>
            await _sut.Handle(
                new GetSeniorRemindersRequest
                {
                    AccountId = account.Id,
                    SeniorId = senior.Id,
                    PaginationQuery = new PaginationQuery { Offset = 0, Limit = 3 }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<SeniorReminderAccessDeniedException>();
    }
}
