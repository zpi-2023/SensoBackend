using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class UpdateReminderByIdTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly UpdateReminderByIdHandler _sut;

    public UpdateReminderByIdTests() => _sut = new UpdateReminderByIdHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldUpdate_WhenSeniorWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var result = await _sut.Handle(
            new UpdateReminderByIdRequest
            {
                AccountId = senior.Id,
                ReminderId = reminder.Id,
                Dto = Generators.UpdateReminderDto.Generate()
            },
            CancellationToken.None
        );

        result.Id.Should().Be(reminder.Id);
        result.SeniorId.Should().Be(senior.Id);
        result.AmountPerIntake.Should().NotBe(reminder.Id);
    }

    [Fact]
    public async Task Handle_ShouldUpdate_WhenCaretakerWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var medication = await _context.SetUpMedication();
        var reminder = await _context.SetUpReminder(senior, senior, medication);

        var result = await _sut.Handle(
            new UpdateReminderByIdRequest
            {
                AccountId = caretaker.Id,
                ReminderId = reminder.Id,
                Dto = Generators.UpdateReminderDto.Generate()
            },
            CancellationToken.None
        );

        result.Id.Should().Be(reminder.Id);
        result.SeniorId.Should().Be(senior.Id);
        result.AmountPerIntake.Should().NotBe(reminder.Id);
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
                new UpdateReminderByIdRequest
                {
                    AccountId = account.Id,
                    ReminderId = reminder.Id,
                    Dto = Generators.UpdateReminderDto.Generate()
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReminderDoesNotExist()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var medication = await _context.SetUpMedication();

        var action = async () =>
            await _sut.Handle(
                new UpdateReminderByIdRequest
                {
                    AccountId = senior.Id,
                    ReminderId = 2137,
                    Dto = Generators.UpdateReminderDto.Generate()
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderNotFoundException>();
    }
}
