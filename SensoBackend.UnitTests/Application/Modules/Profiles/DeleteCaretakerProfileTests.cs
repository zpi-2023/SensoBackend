using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.DeleteCaretakerProfile;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.DeleteCaretakerProfile;

public sealed class DeleteCaretakerProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly DeleteCaretakerProfileHandler _sut;

    public DeleteCaretakerProfileHandlerTests() =>
        _sut = new DeleteCaretakerProfileHandler(_context);

    public void Dispose() => _context.Dispose();
}

public sealed class DeleteCaretakerProfileValidatorTests
{
    private readonly DeleteCaretakerProfileValidator _sut = new();
}
