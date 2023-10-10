using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.CreateAccount;

public sealed record CreateAccountRequest(CreateAccountDto Dto) : IRequest;

[UsedImplicitly]
public sealed class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    //TODO add validation rules
}

[UsedImplicitly]
public sealed class CreateAccountHandler : IRequestHandler<CreateAccountRequest>
{
    private readonly AppDbContext _context;

    public CreateAccountHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateAccountRequest request, CancellationToken ct)
    {
        if (await _context.Accounts.AnyAsync(a => a.Email == request.Dto.Email, ct))
        {
            throw new ValidationException("Email is already taken");
        }

        var account = request.Dto.Adapt<Account>();
        account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
        account.CreatedAt = DateTime.UtcNow;
        account.LastLoginAt = DateTime.UtcNow;
        account.LastPasswordChangeAt = DateTime.UtcNow;
        account.Verified = false;
        account.Active = true;

        await _context.Accounts.AddAsync(account, ct);
        await _context.SaveChangesAsync(ct);
    }
}
