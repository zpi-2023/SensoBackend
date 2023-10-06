using Microsoft.EntityFrameworkCore;
using MediatR;
using JetBrains.Annotations;
using SensoBackend.Data;
using SensoBackend.Domain.Entities;

namespace SensoBackend.Application.Users.GetUsers;

[UsedImplicitly]
public sealed class GetUsersHandler : IRequestHandler<GetUsersRequest, IList<User>>
{
    private readonly AppDbContext _context;

    public GetUsersHandler(AppDbContext context) => _context = context;

    public async Task<IList<User>> Handle(
        GetUsersRequest request,
        CancellationToken cancellationToken
    ) => await _context.Users.ToListAsync();
}
