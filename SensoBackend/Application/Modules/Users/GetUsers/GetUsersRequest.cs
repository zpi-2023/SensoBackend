using MediatR;
using SensoBackend.Application.Modules.Users.Contracts;

namespace SensoBackend.Application.Modules.Users.GetUsers;

public sealed record GetUsersRequest : IRequest<IList<UserDto>>;
