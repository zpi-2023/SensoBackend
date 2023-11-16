using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Domain.Enums;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Accounts.CreateAccount;

public sealed record CreateAccountRequest(CreateAccountDto Dto) : IRequest;

[UsedImplicitly]
public sealed class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(r => r.Dto.Email)
            .NotEmpty()
            .WithMessage("Email is empty.")
            .EmailAddress()
            .WithMessage("Email is invalid.");
        RuleFor(r => r.Dto.Password)
            .NotEmpty()
            .WithMessage("Password is empty.")
            .MinimumLength(8)
            .WithMessage("Password is too short.")
            .MaximumLength(50)
            .WithMessage("Password is too long.");
        RuleFor(r => r.Dto.PhoneNumber)
            .Matches("^[0-9]{9}$")
            .WithMessage("Phone number is invalid.");
        RuleFor(r => r.Dto.DisplayName).NotEmpty().WithMessage("DisplayName is empty.");
    }
}

[UsedImplicitly]
public sealed class CreateAccountHandler : IRequestHandler<CreateAccountRequest>
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public CreateAccountHandler(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task Handle(CreateAccountRequest request, CancellationToken ct)
    {
        if (await _context.Accounts.AnyAsync(a => a.Email == request.Dto.Email, ct))
        {
            throw new ValidationException("Email is already taken");
        }

        var now = _timeProvider.GetUtcNow();

        var account = request.Dto.Adapt<Account>();
        account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
        account.CreatedAt = now;
        account.LastLoginAt = now;
        account.LastPasswordChangeAt = now;
        account.Verified = false;
        account.Active = true;
        account.DisplayName = request.Dto.DisplayName;
        account.Role = Role.Member;

        await _context.Accounts.AddAsync(account, ct);
        await _context.SaveChangesAsync(ct);
    }
}
