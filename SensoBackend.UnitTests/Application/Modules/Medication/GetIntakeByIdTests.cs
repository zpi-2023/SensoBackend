using Microsoft.AspNetCore.Authentication;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetIntakeByIdTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetIntakeByIdHandler _sut;

    public GetIntakeByIdTests() => _sut = new GetIntakeByIdHandler(_context);

    [Fact]
    public async Task Handle_ShouldReturnIntake_WhenSeniorWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var intake = await _context.SetUpIntake(senior, senior);

        var intakeDto = await _sut.Handle(
            new GetIntakeByIdRequest { AccountId = senior.Id, IntakeId = intake.Id },
            CancellationToken.None
        );

        intakeDto.Id.Should().Be(intake.Id);
        intakeDto.ReminderId.Should().Be(intake.ReminderId);
        intakeDto.TakenAt.Should().Be(intake.TakenAt);
        intakeDto.AmountTaken.Should().Be(intake.AmountTaken);
    }

    [Fact]
    public async Task Handle_ShouldReturnIntake_WhenCaretakerWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var caretaker = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretaker, senior);
        var intake = await _context.SetUpIntake(caretaker, senior);

        var intakeDto = await _sut.Handle(
            new GetIntakeByIdRequest { AccountId = caretaker.Id, IntakeId = intake.Id },
            CancellationToken.None
        );

        intakeDto.Id.Should().Be(intake.Id);
        intakeDto.ReminderId.Should().Be(intake.ReminderId);
        intakeDto.TakenAt.Should().Be(intake.TakenAt);
        intakeDto.AmountTaken.Should().Be(intake.AmountTaken);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserWantsIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();
        var intake = await _context.SetUpIntake(senior, senior);

        var action = async () =>
            await _sut.Handle(
                new GetIntakeByIdRequest { AccountId = account.Id, IntakeId = intake.Id },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<ReminderAccessDeniedException>();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenIntakeDoesNotExist()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);

        var action = async () =>
            await _sut.Handle(
                new GetIntakeByIdRequest { AccountId = senior.Id, IntakeId = 2137 },
                CancellationToken.None
            );

        await action.Should().ThrowAsync<IntakeRecordNotFoundException>();
    }
}
