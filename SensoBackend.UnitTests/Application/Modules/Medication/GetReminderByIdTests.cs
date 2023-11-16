using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.Tests.Application.Modules.Medication.Utils;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetReminderByIdTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetReminderByIdHandler _sut;

    public GetReminderByIdTests() => _sut = new GetReminderByIdHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnReminder_WhenSeniorWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var result = await _sut.Handle(
            new GetReminderByIdRequest { AccountId = senior.Id, ReminderId = reminder.Id },
            CancellationToken.None
        );

        result.Id.Should().Be(reminder.Id);
        result.SeniorId.Should().Be(senior.Id);
        result.AmountPerIntake.Should().Be(reminder.AmountPerIntake);
        result.AmountOwned.Should().Be(reminder.AmountOwned);
        result.Cron.Should().Be(reminder.Cron);
        result.Description.Should().Be(reminder.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnReminder_WhenCaretakerWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(caretaker, senior, medication);

        var result = await _sut.Handle(
            new GetReminderByIdRequest { AccountId = caretaker.Id, ReminderId = reminder.Id },
            CancellationToken.None
        );

        result.Id.Should().Be(reminder.Id);
        result.SeniorId.Should().Be(senior.Id);
        result.AmountPerIntake.Should().Be(reminder.AmountPerIntake);
        result.AmountOwned.Should().Be(reminder.AmountOwned);
        result.Cron.Should().Be(reminder.Cron);
        result.Description.Should().Be(reminder.Description);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var action = async () =>
            await _sut.Handle(
                new GetReminderByIdRequest { AccountId = account.Id, ReminderId = reminder.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReminderNotPresent()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var reminder = ReminderTestUtils.GetFakeReminder();

        var action = async () =>
            await _sut.Handle(
                new GetReminderByIdRequest { AccountId = senior.Id, ReminderId = reminder.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderNotFoundException>();
    }
}
