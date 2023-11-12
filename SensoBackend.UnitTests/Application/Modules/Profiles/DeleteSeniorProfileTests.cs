using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.DeleteSeniorProfile;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.DeleteSeniorProfile;

public sealed class DeleteSeniorProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteSeniorProfileHandler _sut;

    public DeleteSeniorProfileHandlerTests() => _sut = new DeleteSeniorProfileHandler(_context);

    public void Dispose() => _context.Dispose();
}

public sealed class DeleteSeniorProfileValidatorTests
{
    private readonly DeleteSeniorProfileValidator _sut = new();
}
