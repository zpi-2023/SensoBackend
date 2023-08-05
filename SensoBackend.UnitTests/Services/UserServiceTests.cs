using SensoBackend.Contracts.User;
using SensoBackend.Data;
using SensoBackend.Models;
using SensoBackend.Services;

namespace SensoBackend.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<AppDbContext> _appDbContextMock = new();

    private IUserService CreateSut()
    {
        return new UserService(_appDbContextMock.Object);
    }

    private static IList<User> GetUsersList()
    {
        return new List<User>
        {
            new() { Id = 0, Name = "Bernadetta Maleszka" },
            new() { Id = 1, Name = "Mariusz Fraś" }
        };
    }

    [Fact]
    private void GetAll_ShouldReturnAllUsers()
    {
        _appDbContextMock.Setup(db => db.Users).ReturnsDbSet(GetUsersList());

        var sut = CreateSut();
        var users = sut.GetAll();

        users.Should().NotBeNull();
        users.Should().HaveCount(GetUsersList().Count);
    }

    [Fact]
    private void GetAll_ShouldReturnEmptyWhenNoUsers()
    {
        _appDbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>());

        var sut = CreateSut();
        var users = sut.GetAll();

        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    private void Create_ShouldCreateUserAndReturnTrue()
    {
        _appDbContextMock.Setup(db => db.Users.Add(It.IsAny<User>()));
        _appDbContextMock.Setup(db => db.SaveChanges());

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var result = sut.Create(newUser);

        result.Should().BeTrue();
        _appDbContextMock.Verify(db => db.Users.Add(It.IsAny<User>()), Times.Exactly(1));
        _appDbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(1));
    }
}
