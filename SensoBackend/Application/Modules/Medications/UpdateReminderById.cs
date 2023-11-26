using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record UpdateReminderByIdRequest : IRequest<ReminderDto>
{
    public required int ReminderId { get; init; }

    public required int AccountId { get; init; }

    public required UpdateReminderDto Dto { get; init; }
}

[UsedImplicitly]
public sealed class UpdateReminderValidator : AbstractValidator<UpdateReminderByIdRequest>
{
    public UpdateReminderValidator()
    {
        RuleFor(r => r.Dto.AmountPerIntake)
            .NotEmpty()
            .WithMessage("AmountPerIntake cannot be empty");
    }
}

[UsedImplicitly]
public sealed class UpdateReminderByIdHandler(AppDbContext context)
    : IRequestHandler<UpdateReminderByIdRequest, ReminderDto>
{
    public async Task<ReminderDto> Handle(UpdateReminderByIdRequest request, CancellationToken ct)
    {
        var reminder = await ReminderUtils.CheckReminderAndProfile(
            context,
            request.AccountId,
            request.ReminderId,
            ct
        );

        if (!reminder.IsActive)
            throw new ReminderNotActiveException(request.ReminderId);

        reminder.AmountPerIntake = request.Dto.AmountPerIntake;
        reminder.AmountOwned = request.Dto.AmountOwned;
        reminder.Cron = request.Dto.Cron;
        reminder.Description = request.Dto.Description;
        await context.SaveChangesAsync(ct);

        return ReminderUtils.AdaptToDto(context, reminder).Result;
    }
}
