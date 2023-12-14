using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class DeleteReminderTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly IHangfireWrapper _hangfireWrapper = Substitute.For<IHangfireWrapper>();
    private readonly DeleteReminderHandler _sut;

    public DeleteReminderTests() => _sut = new DeleteReminderHandler(_context, _hangfireWrapper);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldDeleteReminer_WhenSeniorWantsToDdeleteIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        await _sut.Handle(
            new DeleteReminderRequest { AccountId = senior.Id, ReminderId = reminder.Id },
            CancellationToken.None
        );

        _context.Reminders.First(r => r.Id == reminder.Id).IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldDeleteReminer_WhenCaretakerWantsToDeleteIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(caretaker, senior, medication);

        await _sut.Handle(
            new DeleteReminderRequest { AccountId = caretaker.Id, ReminderId = reminder.Id },
            CancellationToken.None
        );

        _context.Reminders.First(r => r.Id == reminder.Id).IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReminderDoesNotExist()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);

        var action = async () =>
            await _sut.Handle(
                new DeleteReminderRequest { AccountId = senior.Id, ReminderId = 2137 },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAccountDoesNotHavePermissionToDelete()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);
        var account = await _context.SetUpAccount();

        var action = async () =>
            await _sut.Handle(
                new DeleteReminderRequest { AccountId = account.Id, ReminderId = reminder.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }
}
