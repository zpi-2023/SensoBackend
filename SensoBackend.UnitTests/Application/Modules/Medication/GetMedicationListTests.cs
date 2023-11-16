using SensoBackend.Application.Modules.Medications;
using SensoBackend.Infrastructure.Data;
using SensoBackend.UnitTests.Utils;

namespace SensoBackend.Tests.Application.Modules.Medication;

public sealed class GetMedicationListTests
{
    private readonly AppDbContext _context = Database.CreateFixture();
    private readonly GetMedicationListHandler _sut;

    public GetMedicationListTests() => _sut = new GetMedicationListHandler(_context);

    [Fact]
    public async Task Handle_ShouldReturnCorrectMedicationList_WhenEverythingIsOk()
    {
        await _context.SetUpMedication("This");
        await _context.SetUpMedication("is");
        await _context.SetUpMedication("test");

        var result = await _sut.Handle(
            new GetMedicationListRequest { SearchTerm = "is" },
            CancellationToken.None
        );

        result.Medications.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectMedicationList_EvenIfMedicationNameDiffersInCase()
    {
        await _context.SetUpMedication("This");
        await _context.SetUpMedication("is");
        await _context.SetUpMedication("test");
        await _context.SetUpMedication("THIS IS UPPERCASE");

        var result = await _sut.Handle(
            new GetMedicationListRequest { SearchTerm = "is" },
            CancellationToken.None
        );

        result.Medications.Should().HaveCount(3);
    }
}
