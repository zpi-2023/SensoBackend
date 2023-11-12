using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SensoBackend.Application.Modules.Profiles.EditCaretakerProfile;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Profiles.EditCaretakerProfile;

public sealed class EditCaretakerProfileHandlerTests : IDisposable
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly EditCaretakerProfileHandler _sut;

    public EditCaretakerProfileHandlerTests() => _sut = new EditCaretakerProfileHandler(_context);

    public void Dispose() => _context.Dispose();
}

public sealed class EditCaretakerProfileValidatorTests
{
    private readonly EditCaretakerProfileValidator _sut = new();
}
