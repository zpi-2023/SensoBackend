using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class CreateIntakeTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateIntakeHandler _sut;

    public CreateIntakeTests() => _sut = new CreateIntakeHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldCreateIntake_WhenSeniorCreatesIntake()
    {
        var medication = await _context.SetUpMedication();
        var account = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(account);

        var reminder = await _context.SetUpReminder(account, account, medication);
        var createIntakeDto = Generators.CreateIntakeDto.Generate();

        var intake = await _sut.Handle(
            new CreateIntakeRequest
            {
                AccountId = account.Id,
                ReminderId = reminder.Id,
                Dto = createIntakeDto
            },
            CancellationToken.None
        );

        _context.IntakeRecords
            .Any(ir => ir.Id == intake.Id && ir.Reminder == reminder)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCaretakerCreatesIntake()
    {
        var medication = await _context.SetUpMedication();
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(account, senior);
        await _context.SaveChangesAsync();

        var reminder = await _context.SetUpReminder(account, senior, medication);
        var createIntakeDto = Generators.CreateIntakeDto.Generate();

        var action = async () =>
            await _sut.Handle(
                new CreateIntakeRequest
                {
                    AccountId = account.Id,
                    ReminderId = reminder.Id,
                    Dto = createIntakeDto
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenReminderIdDoesNotExist()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);

        var createIntakeDto = Generators.CreateIntakeDto.Generate();

        var action = async () =>
            await _sut.Handle(
                new CreateIntakeRequest
                {
                    AccountId = senior.Id,
                    ReminderId = 2137, //there's nothing in reminders table anyway
                    Dto = createIntakeDto
                },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderNotFoundException>();
    }
}
