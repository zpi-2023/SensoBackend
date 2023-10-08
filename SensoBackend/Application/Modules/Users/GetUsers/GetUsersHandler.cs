using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Application.Modules.Users.GetUsers;

[UsedImplicitly]
public sealed class GetUsersHandler : IRequestHandler<GetUsersRequest, IList<UserDto>>
{
    private readonly AppDbContext _context;

    public GetUsersHandler(AppDbContext context) => _context = context;

    public async Task<IList<UserDto>> Handle(GetUsersRequest request, CancellationToken ct)
    {
        var users = await _context.Users.ToListAsync(ct);
        return users.Adapt<IList<UserDto>>();
    }
}
