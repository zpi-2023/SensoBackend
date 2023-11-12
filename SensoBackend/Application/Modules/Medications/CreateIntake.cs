﻿using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Entities;
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
        RuleFor(r => r.ReminderId)
            .NotEmpty()
            .WithMessage("ReminderId cannot be empty")
            .GreaterThan(0)
            .WithMessage("ReminderId has to be greater than 0");
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId cannot be empty")
            .GreaterThan(0)
            .WithMessage("AccountId has to be greater than 0");
        RuleFor(r => r.Dto.AmountTaken)
            .NotEmpty()
            .WithMessage("AmountTaken cannot be empty")
            .GreaterThan(0)
            .WithMessage("AmountTaken has to be greater than 0");
        RuleFor(r => r.Dto.TakenAt).NotEmpty().WithMessage("TakenAt cannot be empty");
    }
}

[UsedImplicitly]
public sealed class CreateIntakeHandler : IRequestHandler<CreateIntakeRequest, IntakeDto>
{
    private readonly AppDbContext _context;

    public CreateIntakeHandler(AppDbContext context) => _context = context;

    public async Task<IntakeDto> Handle(CreateIntakeRequest request, CancellationToken ct)
    {
        ReminderUtils.CheckReminderAndProfile(
            context: _context,
            accountId: request.AccountId,
            reminderId: request.ReminderId,
            ct: ct
        );

        var intakeRecord = request.Dto.Adapt<IntakeRecord>();
        intakeRecord.ReminderId = request.ReminderId;

        await _context.IntakeRecords.AddAsync(intakeRecord, ct);
        await _context.SaveChangesAsync(ct);

        var intakeDto = ReminderUtils.AdaptToDto(_context, intakeRecord!).Result;
        return intakeDto;
    }
}