using SensoBackend.Application.Modules.Users.GetUsers;
using SensoBackend.Infrastructure.Data;
using SensoBackend.Tests.Utils;

namespace SensoBackend.Tests.Application.Modules.Users.GetUsers;

public sealed class GetUsersHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetUsersHandler _sut;

    public GetUsersHandlerTests() => _sut = new GetUsersHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        const int count = 5;
        var entities = Generators.User.Generate(count);
        await _context.Users.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        var users = await _sut.Handle(new GetUsersRequest(), CancellationToken.None);

        users.Should().NotBeNull();
        users.Should().HaveCount(count);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyWhenNoUsers()
    {
        var users = await _sut.Handle(new GetUsersRequest(), CancellationToken.None);

        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }
}
