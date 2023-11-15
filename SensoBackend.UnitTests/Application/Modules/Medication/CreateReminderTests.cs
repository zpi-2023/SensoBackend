using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class CreateReminderTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateReminderHandler _sut;

    public CreateReminderTests() => _sut = new CreateReminderHandler(_context);

    [Fact]
    public async Task Handle_ShouldCreateReminder_WhenSeniorWantsToCreateIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);

        var reminderDto = Generators.CreateReminderDto.Generate();

        var reminder = await _sut.Handle(
            new CreateReminderRequest
            {
                AccountId = senior.Id,
                SeniorId = senior.Id,
                Dto = reminderDto
            },
            CancellationToken.None
        );

        _context.Reminders
            .Any(r => r.Id == reminder.Id && r.Senior == senior)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCreateReminder_WhenCaretakerWantsToCreateIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);

        var reminderDto = Generators.CreateReminderDto.Generate();

        var reminder = await _sut.Handle(
            new CreateReminderRequest
            {
                AccountId = caretaker.Id,
                SeniorId = senior.Id,
                Dto = reminderDto
            },
            CancellationToken.None
        );

        _context.Reminders
            .Any(r => r.Id == reminder.Id && r.Senior == senior)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserWantsToCreateIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();

        var reminderDto = Generators.CreateReminderDto.Generate();

        var action = async () => await _sut.Handle(
            new CreateReminderRequest
            {
                AccountId = account.Id,
                SeniorId = senior.Id,
                Dto = reminderDto
            },
            CancellationToken.None
        );

        await action.Should().ThrowAsync<SeniorReminderAccessDeniedException>();
    }
}
