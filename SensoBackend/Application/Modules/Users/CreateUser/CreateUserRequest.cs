using MediatR;

namespace SensoBackend.Application.Modules.Users.CreateUser;

public sealed record CreateUserRequest : IRequest
{
    public required string Name { get; init; }
}
