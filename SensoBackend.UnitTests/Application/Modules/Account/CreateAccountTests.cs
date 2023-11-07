using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Accounts.CreateAccount;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Accounts.CreateAccount;

public sealed class CreateAccountHandlerTests : IDisposable
{
    private static readonly DateTimeOffset Now = new(2023, 9, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly CreateAccountHandler _sut;

    public CreateAccountHandlerTests() =>
        _sut = new CreateAccountHandler(_context, new MockTimeProvider { Now = Now });

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldCreateAccount()
    {
        var dto = Generators.CreateAccountDto.Generate();

        await _sut.Handle(new CreateAccountRequest(dto), CancellationToken.None);

        _context.Accounts.Any(a => a.Email == dto.Email).Should().BeTrue();

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == dto.Email);

        account.Should().NotBeNull();
        if (account == null)
            return;

        BCrypt.Net.BCrypt.Verify(dto.Password, account.Password).Should().BeTrue();
        account.PhoneNumber.Should().Be(dto.PhoneNumber);
        account.Active.Should().BeTrue();
        account.Verified.Should().BeFalse();
        account.CreatedAt.Should().Be(Now);
        account.LastLoginAt.Should().Be(Now);
        account.LastPasswordChangeAt.Should().Be(Now);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenEmailIsTaken()
    {
        var dto = Generators.CreateAccountDto.Generate();
        await _context.Accounts.AddAsync(dto.Adapt<Domain.Entities.Account>());
        await _context.SaveChangesAsync();

        var act = async () =>
            await _sut.Handle(new CreateAccountRequest(dto), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}

public sealed class CreateAccountValidatorTests
{
    private readonly CreateAccountValidator _sut = new();

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        var dto = Generators.CreateAccountDto.Generate() with { Email = "invalid-email" };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenEmailIsEmpty()
    {
        var dto = Generators.CreateAccountDto.Generate() with { Email = string.Empty };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenPasswordIsEmpty()
    {
        var dto = Generators.CreateAccountDto.Generate() with { Password = string.Empty };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenPasswordIsTooShort()
    {
        var dto = Generators.CreateAccountDto.Generate() with { Password = "short" };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenPasswordIsTooLong()
    {
        var dto = Generators.CreateAccountDto.Generate() with { Password = new string('a', 51) };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldThrowValidationException_WhenPhoneNumberIsInvalid()
    {
        var dto = Generators.CreateAccountDto.Generate() with
        {
            PhoneNumber = "invalid-phone-number"
        };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenPhoneNumberIsNull()
    {
        var dto = Generators.CreateAccountDto.Generate() with { PhoneNumber = null };

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_ShouldNotThrow_WhenDtoIsValid()
    {
        var dto = Generators.CreateAccountDto.Generate();

        Action act = () => _sut.ValidateAndThrow(new CreateAccountRequest(dto));

        act.Should().NotThrow();
    }
}
