﻿using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetIntakeByIdRequest : IRequest<IntakeDto>
{
    public required int AccountId { get; init; }

    public required int IntakeId { get; init; }
}

[UsedImplicitly]
public sealed class GetIntakeByIdValidator : AbstractValidator<GetIntakeByIdRequest>
{
    public GetIntakeByIdValidator()
    {
        RuleFor(r => r.AccountId)
            .NotEmpty()
            .WithMessage("AccountId cannot be empty")
            .GreaterThan(0)
            .WithMessage("AccountId has to be greater than 0");
        RuleFor(r => r.IntakeId)
            .NotEmpty()
            .WithMessage("IntakeId cannot be empty")
            .GreaterThan(0)
            .WithMessage("IntakeId has to be greater than 0");
    }
}

[UsedImplicitly]
public sealed class GetIntakeByIdHandler : IRequestHandler<GetIntakeByIdRequest, IntakeDto>
{
    private readonly AppDbContext _context;

    public GetIntakeByIdHandler(AppDbContext context) => _context = context;

    public async Task<IntakeDto> Handle(GetIntakeByIdRequest request, CancellationToken ct)
    {
        var intake = await _context.IntakeRecords.FindAsync(request.IntakeId)
            ?? throw new IntakeRecordNotFoundException(request.IntakeId);

        await ReminderUtils.CheckReminderAndProfile(
            context: _context,
            accountId: request.AccountId,
            reminderId: intake.ReminderId,
            ct: ct
        );

        var intakeDto = ReminderUtils.AdaptToDto(_context, intake).Result;
        return intakeDto;
    }
}
