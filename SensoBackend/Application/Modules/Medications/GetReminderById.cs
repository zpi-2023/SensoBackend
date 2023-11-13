using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Application.Modules.Medications.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetReminderByIdRequest : IRequest<ReminderDto>
{
    public required int ReminderId { get; init; }

    public required int AccountId { get; init; }
}

[UsedImplicitly]
public sealed class GetReminderByIdHandler : IRequestHandler<GetReminderByIdRequest, ReminderDto>
{
    private readonly AppDbContext _context;

    public GetReminderByIdHandler(AppDbContext context) => _context = context;

    public async Task<ReminderDto> Handle(GetReminderByIdRequest request, CancellationToken ct)
    {
        var reminder =
            await _context.Reminders.FindAsync(request.ReminderId, ct)
            ?? throw new ReminderNotFoundException(request.ReminderId);

        var neededProfile = await _context.Profiles.FirstOrDefaultAsync(
            p => p.AccountId == request.AccountId && p.SeniorId == reminder.SeniorId
        );

        return neededProfile is null
            ? throw new ReminderAccessDeniedException(request.ReminderId)
            : ReminderUtils.AdaptToDto(_context, reminder).Result;
    }
}
