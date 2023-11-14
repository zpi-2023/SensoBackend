using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Medications.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Medications;

public sealed record GetMedicationListRequest : IRequest<MedicationListDto>
{
    public required string SearchTerm { get; init; }
}

[UsedImplicitly]
public sealed class GetMedicationListHanddler
    : IRequestHandler<GetMedicationListRequest, MedicationListDto>
{
    private readonly AppDbContext _context;

    public GetMedicationListHanddler(AppDbContext context) => _context = context;

    public async Task<MedicationListDto> Handle(
        GetMedicationListRequest request,
        CancellationToken ct
    )
    {
        var searchTermLowerCase = request.SearchTerm.ToLower();

        var medicationList = await _context.Medications
            .Where(m => m.Name.ToLower().Contains(searchTermLowerCase))
            .Select(m => m.Adapt<MedicationDto>())
            .ToListAsync(ct);

        return new MedicationListDto { Medications = medicationList };
    }
}
