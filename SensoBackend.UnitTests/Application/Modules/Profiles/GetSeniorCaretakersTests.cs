using Mapster;
using SensoBackend.Application.Modules.Profiles;
using SensoBackend.Application.Modules.Profiles.Contracts;
using SensoBackend.Application.Modules.Profiles.Utils;
using SensoBackend.Domain.Exceptions;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.UnitTests.Application.Modules.Profiles;

public sealed class GetSeniorCaretakersHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetSeniorCaretakersHandler _sut;

    public GetSeniorCaretakersHandlerTests()
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.RuleMap.Clear();
        typeAdapterConfig.Apply(new MappingConfig());

        _sut = new(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ShouldReturnListOfCaretakersProfiles()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var caretakerAccount = await _context.SetUpAccount();
        await _context.SetUpCaretakerProfile(caretakerAccount, seniorAccount);

        var expected = new ExtendedProfilesDto
        {
            Profiles =
            [
                new()
                {
                    AccountId = caretakerAccount.Id,
                    SeniorId = seniorAccount.Id,
                    Type = "caretaker",
                    DisplayName = caretakerAccount.DisplayName,
                    Email = caretakerAccount.Email,
                    PhoneNumber = caretakerAccount.PhoneNumber
                }
            ]
        };

        var profiles = await _sut.Handle(
            new GetSeniorCaretakersRequest { AccountId = seniorAccount.Id },
            CancellationToken.None
        );

        profiles.Should().BeOfType<ExtendedProfilesDto>();
        profiles.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldThrowProfileNotFoundException_WhenSeniorDoesNotHaveProfile()
    {
        var seniorAccount = await _context.SetUpAccount();

        var act = async () =>
            await _sut.Handle(
                new GetSeniorCaretakersRequest { AccountId = seniorAccount.Id },
                CancellationToken.None
            );

        await act.Should().ThrowAsync<ProfileNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSeniorHasNoCaretakers()
    {
        var seniorAccount = await _context.SetUpAccount();
        await _context.SetUpSeniorProfile(seniorAccount);

        var profiles = await _sut.Handle(
            new GetSeniorCaretakersRequest { AccountId = seniorAccount.Id },
            CancellationToken.None
        );

        profiles.Should().BeOfType<ExtendedProfilesDto>();
        profiles.Profiles.Should().BeEmpty();
    }
}
