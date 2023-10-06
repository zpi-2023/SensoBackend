using Mapster;
using MediatR;
using JetBrains.Annotations;
using SensoBackend.Data;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Users.CreateUser;

[UsedImplicitly]
public sealed class CreateUserHandler : IRequestHandler<CreateUserRequest>
{
    private readonly AppDbContext _context;

    public CreateUserHandler(AppDbContext context) => _context = context;

    public async Task Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _context.Users.Add(request.Adapt<User>());
        _context.SaveChanges();
    }
}
