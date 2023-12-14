using MediatR;
using Microsoft.Extensions.Logging;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Medications;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class CreateReminderTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IHangfireWrapper _hangfireWrapper = Substitute.For<IHangfireWrapper>();
    private readonly ILogger<CreateReminderHandler> _logger = Substitute.For<
        ILogger<CreateReminderHandler>
    >();
    private readonly CreateReminderHandler _sut;

    public CreateReminderTests() =>
        _sut = new CreateReminderHandler(_context, _mediator, _hangfireWrapper, _logger);

    public void Dispose() => _context.Dispose();

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

        _context.Reminders.Any(r => r.Id == reminder.Id && r.Senior == senior).Should().BeTrue();
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

        _context.Reminders.Any(r => r.Id == reminder.Id && r.Senior == senior).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenRandomUserWantsToCreateIt()
    {
        var senior = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(senior);
        var account = await _context.SetUpAccount();

        var reminderDto = Generators.CreateReminderDto.Generate();

        var action = async () =>
            await _sut.Handle(
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
