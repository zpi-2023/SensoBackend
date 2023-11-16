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
        RuleFor(r => r.ReminderId).NotEmpty().WithMessage("ReminderId cannot be empty");
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId cannot be empty");
        RuleFor(r => r.Dto.AmountPerIntake)
            .NotEmpty()
            .WithMessage("AmountPerIntake cannot be empty");
    }
}

[UsedImplicitly]
public sealed class UpdateReminderByIdHandler
    : IRequestHandler<UpdateReminderByIdRequest, ReminderDto>
{
    private readonly AppDbContext _context;

    public UpdateReminderByIdHandler(AppDbContext context) => _context = context;

    public async Task<ReminderDto> Handle(UpdateReminderByIdRequest request, CancellationToken ct)
    {
        var reminder = await ReminderUtils.CheckReminderAndProfile(
            _context,
            request.AccountId,
            request.ReminderId,
            ct
        );

        reminder.AmountPerIntake = request.Dto.AmountPerIntake;
        reminder.AmountOwned = request.Dto.AmountOwned;
        reminder.Cron = request.Dto.Cron;
        reminder.Description = request.Dto.Description;
        await _context.SaveChangesAsync(ct);

        return ReminderUtils.AdaptToDto(_context, reminder).Result;
    }
}
