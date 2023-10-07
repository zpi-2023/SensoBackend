using MediatR;
using SensoBackend.Application.Modules.Users.Contracts;

namespace SensoBackend.Application.Modules.Users.CreateUser;

public sealed record CreateUserRequest(CreateUserDto Dto) : IRequest;
