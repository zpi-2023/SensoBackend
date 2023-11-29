using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.Tests.Application.Modules.Medication.Utils;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetAllIntakesForReminderTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetAllIntakesForReminderHandler _sut;

    public GetAllIntakesForReminderTests() => _sut = new GetAllIntakesForReminderHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnAllIntakes_WhenLimitIsLargerThanTheirCount()
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
            new GetAllIntakesForReminderRequest
            {
                AccountId = senior.Id,
                ReminderId = intake.ReminderId,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlySomeIntakes_WhenLimitIsLowerThanTheirCount()
    {
        var limit = 3;
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var intake = await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);

        var result = await _sut.Handle(
            new GetAllIntakesForReminderRequest
            {
                AccountId = senior.Id,
                ReminderId = intake.ReminderId,
                PaginationQuery = new PaginationQuery { Offset = 0, Limit = limit }
            },
            CancellationToken.None
        );

        result.Items.Should().HaveCount(limit);
    }

    [Fact]
    public async Task Handle_ResultShouldNotContainNewestIntake_WhenOffsetIsOne()
    {
        var limit = 3;
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        var intake = await _context.SetUpIntakeForReminder(reminder);

        var result = await _sut.Handle(
            new GetAllIntakesForReminderRequest
            {
                AccountId = senior.Id,
                ReminderId = intake.ReminderId,
                PaginationQuery = new PaginationQuery { Offset = 1, Limit = limit }
            },
            CancellationToken.None
        );

        result.Items.Should().NotContain(i => i.Id == intake.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllIntakes_WhenCaretakerTriesToAccessThem()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var intake = await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);
        await _context.SetUpIntakeForReminder(reminder);

        var result = await _sut.Handle(
            new GetAllIntakesForReminderRequest
            {
                AccountId = caretaker.Id,
                ReminderId = intake.ReminderId,
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
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var intake = await _context.SetUpIntakeForReminder(reminder);

        var action = async () =>
            await _sut.Handle(
                new GetAllIntakesForReminderRequest
                {
                    AccountId = account.Id,
                    ReminderId = intake.ReminderId,
                    PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReminderIsNotPresent()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();
        var medication = await _context.SetUpMedication();
        var reminder = ReminderTestUtils.GetFakeReminder();

        var action = async () =>
            await _sut.Handle(
                new GetAllIntakesForReminderRequest
                {
                    AccountId = account.Id,
                    ReminderId = reminder.Id,
                    PaginationQuery = new PaginationQuery { Offset = 0, Limit = 5 }
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderNotFoundException>();
    }
}
