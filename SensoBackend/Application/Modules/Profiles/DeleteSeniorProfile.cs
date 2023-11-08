using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Profiles.DeleteSeniorProfile;

public sealed record DeleteSeniorProfileRequest(int AccountId) : IRequest;

[UsedImplicitly]
public sealed class DeleteSeniorProfileValidator : AbstractValidator<DeleteSeniorProfileRequest>
{
    public DeleteSeniorProfileValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty().WithMessage("AccountId is empty.");
    }
}

[UsedImplicitly]
public sealed class DeleteSeniorProfileHandler : IRequestHandler<DeleteSeniorProfileRequest>
{
    private readonly AppDbContext _context;

    public DeleteSeniorProfileHandler(AppDbContext context) => _context = context;

    public async Task Handle(DeleteSeniorProfileRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
