using Mapster;
using Moq;
using Moq.EntityFrameworkCore;
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

    private static IEnumerable<User> GetUsersList()
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

        Assert.NotNull(users);
        Assert.Equal(2, users.Count());
    }

    [Fact]
    private void GetAll_ShouldReturnEmptyWhenNoUsers()
    {
        _appDbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>());

        var sut = CreateSut();
        var users = sut.GetAll();

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    private void Create_ShouldCreateUserAndReturnTrue()
    {
        _appDbContextMock.Setup(db => db.Users.Add(It.IsAny<User>()));
        _appDbContextMock.Setup(db => db.SaveChanges());

        var sut = CreateSut();
        var newUser = new CreateUserDto { Name = "Bernadetta Maleszka" };
        var result = sut.Create(newUser);

        Assert.True(result);
        _appDbContextMock.Verify(db => db.Users.Add(It.IsAny<User>()), Times.Exactly(1));
    }
}
