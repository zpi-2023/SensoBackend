using FluentValidation;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Users.CreateUser;

public sealed record CreateUserRequest(CreateUserDto Dto) : IRequest;

[UsedImplicitly]
public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(r => r.Dto.Name).NotEmpty().MaximumLength(50);
    }
}

[UsedImplicitly]
public sealed class CreateUserHandler : IRequestHandler<CreateUserRequest>
{
    private readonly AppDbContext _context;

    public CreateUserHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateUserRequest request, CancellationToken ct)
    {
        await _context.Users.AddAsync(request.Dto.Adapt<User>(), ct);
        await _context.SaveChangesAsync(ct);
    }
}
