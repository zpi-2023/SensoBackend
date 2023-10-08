using SensoBackend.Application.Modules.Users.CreateUser;
using SensoBackend.Infrastructure.Data;
using SensoBackend.Tests.Utils;

namespace SensoBackend.Tests.Application.Modules.Users.CreateUser;

public sealed class CreateUserHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateUserHandler _sut;

    public CreateUserHandlerTests() => _sut = new CreateUserHandler(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldCreateUser()
    {
        var dto = Generators.CreateUserDto.Generate();
        await _sut.Handle(new CreateUserRequest(dto), CancellationToken.None);

        _context.Users.Any(u => u.Name == dto.Name).Should().BeTrue();
    }
}
