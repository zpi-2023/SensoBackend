using SensoBackend.Application.Modules.Users.Contracts;
using SensoBackend.Application.Modules.Users.CreateUser;
using SensoBackend.Domain.Entities;
using SensoBackend.Infrastructure.Data;

namespace SensoBackend.Tests.Application.Modules.Users.CreateUser;

public sealed class CreateUserHandlerTests
{
    private readonly Mock<AppDbContext> _contextMock = new();
    private readonly CreateUserHandler _sut;

    public CreateUserHandlerTests() => _sut = new CreateUserHandler(_contextMock.Object);

    [Fact]
    public async Task Handle_ShouldCreateUser()
    {
        _contextMock.Setup(
            db => db.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())
        );
        _contextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var dto = new CreateUserDto { Name = "Bernadetta Maleszka" };
        await _sut.Handle(new CreateUserRequest(dto), CancellationToken.None);

        _contextMock.Verify(
            db => db.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _contextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
