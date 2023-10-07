using SensoBackend.Application.Modules.Users.GetUsers;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Tests.Application.Modules.Users.GetUsers;

public sealed class GetUsersHandlerTests
{
    private readonly Mock<AppDbContext> _contextMock = new();
    private readonly GetUsersHandler _sut;

    public GetUsersHandlerTests() => _sut = new GetUsersHandler(_contextMock.Object);

    private static IList<User> GetUserList() =>
        new List<User>
        {
            new() { Id = 0, Name = "Bernadetta Maleszka" },
            new() { Id = 1, Name = "Mariusz Fraś" }
        };

    [Fact]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        _contextMock.Setup(db => db.Users).ReturnsDbSet(GetUserList());

        var users = await _sut.Handle(new GetUsersRequest(), CancellationToken.None);

        users.Should().NotBeNull();
        users.Should().HaveCount(GetUserList().Count);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyWhenNoUsers()
    {
        _contextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>());

        var users = await _sut.Handle(new GetUsersRequest(), CancellationToken.None);

        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }
}
