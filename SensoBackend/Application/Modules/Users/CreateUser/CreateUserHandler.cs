using JetBrains.Annotations;
using Mapster;
using MediatR;
using SensoBackend.Data;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Users.CreateUser;

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
