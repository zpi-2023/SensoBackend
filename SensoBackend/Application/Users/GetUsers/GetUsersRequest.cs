using MediatR;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Users.GetUsers;

public sealed record GetUsersRequest : IRequest<IList<User>>;
