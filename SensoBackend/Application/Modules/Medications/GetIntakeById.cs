﻿using JetBrains.Annotations;
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
public sealed class GetIntakeByIdHandler(AppDbContext context)
    : IRequestHandler<GetIntakeByIdRequest, IntakeDto>
{
    public async Task<IntakeDto> Handle(GetIntakeByIdRequest request, CancellationToken ct)
    {
        var intake =
            await context.IntakeRecords.FindAsync(request.IntakeId, ct)
            ?? throw new IntakeRecordNotFoundException(request.IntakeId);

        await ReminderUtils.CheckReminderAndProfile(
            context: context,
            accountId: request.AccountId,
            reminderId: intake.ReminderId,
            ct: ct
        );

        var intakeDto = ReminderUtils.AdaptToDto(context, intake).Result;
        return intakeDto;
    }
}
