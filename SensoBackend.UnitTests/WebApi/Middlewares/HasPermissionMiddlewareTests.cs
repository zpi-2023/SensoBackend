using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SensoBackend.Application.Abstractions;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Enums;
using SensoBackend.WebApi.Authorization.Data;
using SensoBackend.WebApi.Middlewares;

namespace SensoBackend.UnitTests.WebApi.Middlewares;

public sealed class HasPermissionMiddlewareTests
{
    private const int MockAccountId = 123;

    private readonly HasPermissionMiddleware _sut =
        new(Substitute.For<RequestDelegate>(), Substitute.For<ILogger<HasPermissionMiddleware>>());

    [Fact]
    public async Task ValidateAsync_ShouldReturnNext_WhenNoAttributeIsPresent()
    {
        var result = await _sut.ValidateAsync(
            Substitute.For<IAuthorizationService>(),
            null,
            null,
            null
        );

        result.Should().Be(HasPermissionResult.Next);
    }

    [Theory]
    [InlineData(Permission.ManageDashboard, null)]
    [InlineData(Permission.ReadNotes, "invalid claim")]
    public async Task ValidateAsync_ShouldReturnBlockWithUnauthorized_WhenAccountIsAbsentOrInvalid(
        Permission permission,
        string? accountIdClaim
    )
    {
        var result = await _sut.ValidateAsync(
            Substitute.For<IAuthorizationService>(),
            new(permission),
            null,
            accountIdClaim
        );

        result.Should().Be(HasPermissionResult.BlockWithUnauthorized);
    }

    [Theory]
    [InlineData(Permission.MutateNotes, null)]
    [InlineData(Permission.ReadNotes, "13")]
    [InlineData(Permission.ManageProfiles, "21")]
    public async Task ValidateAsync_ShouldReturnNext_WhenAccountIsAnAdmin(
        Permission permission,
        string? seniorIdParam
    )
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(MockAccountId).Returns(Role.Admin);

        var result = await _sut.ValidateAsync(
            authorizationService,
            new(permission),
            seniorIdParam,
            MockAccountId.ToString()
        );

        result.Should().Be(HasPermissionResult.Next);
    }

    [Fact]
    public async Task ValidateAsync_ShouldThrow_WhenRoleIsInvalid()
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(MockAccountId).Returns((Role)123456);

        var action = async () =>
        {
            await _sut.ValidateAsync(
                authorizationService,
                new(Permission.ReadNotes),
                null,
                MockAccountId.ToString()
            );
        };

        await action.Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnBlockWithUnauthorized_WhenAccountHasInsufficientPermissions()
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(MockAccountId).Returns(Role.Member);

        var result = await _sut.ValidateAsync(
            authorizationService,
            new((Permission)123123),
            null,
            MockAccountId.ToString()
        );

        result.Should().Be(HasPermissionResult.BlockWithUnauthorized);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnNext_WhenAccountIsMemberWithSufficientPermissions()
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(MockAccountId).Returns(Role.Member);

        var result = await _sut.ValidateAsync(
            authorizationService,
            new(Permission.ReadNotes),
            null,
            MockAccountId.ToString()
        );

        result.Should().Be(HasPermissionResult.Next);
    }

    [Theory]
    [InlineData(Permission.ManageDashboard, "")]
    [InlineData(Permission.ReadNotes, "test")]
    [InlineData(Permission.MutateNotes, "     ")]
    [InlineData(Permission.ManageProfiles, "3.5")]
    public async Task ValidateAsync_ShouldReturnBlockWithBadRequest_WhenSeniorIdIsAbsentOrInvalid(
        Permission permission,
        string? seniorIdParam
    )
    {
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(MockAccountId).Returns(Role.Member);

        var result = await _sut.ValidateAsync(
            authorizationService,
            new(permission),
            seniorIdParam,
            MockAccountId.ToString()
        );

        result.Should().Be(HasPermissionResult.BlockWithBadRequest);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnBlockWithUnauthorized_WhenRequestorIsNotSeniorsCaretaker()
    {
        const int seniorId = 34;
        const int impostorId = 55;
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(Arg.Any<int>()).Returns(Role.Member);
        authorizationService.GetProfilesByAccountId(impostorId).Returns(new List<ProfileInfo>());

        var result = await _sut.ValidateAsync(
            authorizationService,
            new(Permission.ManageProfiles),
            seniorId.ToString(),
            impostorId.ToString()
        );

        result.Should().Be(HasPermissionResult.BlockWithUnauthorized);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnNext_WhenRequestorIsSeniorsCaretaker()
    {
        const int seniorId = 89;
        const int caretakerId = 144;
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationService.GetRoleAsync(Arg.Any<int>()).Returns(Role.Member);
        authorizationService
            .GetProfilesByAccountId(caretakerId)
            .Returns(
                new List<ProfileInfo>
                {
                    new()
                    {
                        Id = 3,
                        AccountId = caretakerId,
                        SeniorId = seniorId,
                        Alias = string.Empty
                    }
                }
            );

        var result = await _sut.ValidateAsync(
            authorizationService,
            new(Permission.ReadNotes),
            seniorId.ToString(),
            caretakerId.ToString()
        );

        result.Should().Be(HasPermissionResult.Next);
    }
}
