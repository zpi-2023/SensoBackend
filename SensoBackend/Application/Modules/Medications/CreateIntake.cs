using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record CreateIntakeRequest : IRequest<IntakeDto>
{
    public required int AccountId { get; init; }

    public required int ReminderId { get; init; }

    public required CreateIntakeDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class CreateIntakeValidator : AbstractValidator<CreateIntakeRequest>
{
    public CreateIntakeValidator()
    {
        RuleFor(r => r.Dto.AmountTaken)
            .NotEmpty()
            .WithMessage("AmountTaken cannot be empty")
            .GreaterThan(0)
            .WithMessage("AmountTaken has to be greater than 0");
        RuleFor(r => r.Dto.TakenAt).NotEmpty().WithMessage("TakenAt cannot be empty");
    }
}

[UsedImplicitly]
public sealed class CreateIntakeHandler(AppDbContext context)
    : IRequestHandler<CreateIntakeRequest, IntakeDto>
{
    public async Task<IntakeDto> Handle(CreateIntakeRequest request, CancellationToken ct)
    {
        var reminder =
            await context.Reminders.FindAsync(request.ReminderId, ct)
            ?? throw new ReminderNotFoundException(request.ReminderId);

        if (!reminder.IsActive)
            throw new ReminderNotActiveException(request.ReminderId);

        var neededProfile =
            await context
                .Profiles
                .FirstOrDefaultAsync(
                    p => p.SeniorId == request.AccountId && p.SeniorId == reminder.SeniorId,
                    ct
                ) ?? throw new ReminderAccessDeniedException(request.ReminderId);

        var intakeRecord = request.Dto.Adapt<IntakeRecord>();
        intakeRecord.ReminderId = request.ReminderId;

        await context.IntakeRecords.AddAsync(intakeRecord, ct);
        await context.SaveChangesAsync(ct);

        var intakeDto = ReminderUtils.AdaptToDto(context, intakeRecord!).Result;
        return intakeDto;
    }
}
