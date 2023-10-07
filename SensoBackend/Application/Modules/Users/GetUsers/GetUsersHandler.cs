using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Data;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Modules.Users.GetUsers;

[UsedImplicitly]
public sealed class GetUsersHandler : IRequestHandler<GetUsersRequest, IList<User>>
{
    private readonly AppDbContext _context;

    public GetUsersHandler(AppDbContext context) => _context = context;

    public async Task<IList<User>> Handle(GetUsersRequest request, CancellationToken ct) =>
        await _context.Users.ToListAsync(ct);
}
