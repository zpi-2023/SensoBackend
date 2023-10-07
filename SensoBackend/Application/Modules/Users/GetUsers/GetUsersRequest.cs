using MediatR;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Users.GetUsers;

public sealed record GetUsersRequest : IRequest<IList<User>>;
