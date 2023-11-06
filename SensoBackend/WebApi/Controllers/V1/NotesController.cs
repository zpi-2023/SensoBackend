using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class NotesControlller : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesControlller(IMediator mediator) => _mediator = mediator;

    [HasPermission(Permission.MutateNotes)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(UpsertNoteDto dto)
    {
        return CreatedAtAction(nameof(GetById), new { noteId = 0 }, default(NoteDetailsDto));
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteListDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(new NoteListDto { Notes = new List<NoteDetailsDto>() });
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDetailsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int noteId)
    {
        return Ok(default(NoteDetailsDto));
    }

    [HasPermission(Permission.MutateNotes)]
    [HttpPut("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int noteId, UpsertNoteDto dto)
    {
        return Ok(default(NoteDetailsDto));
    }

    [HasPermission(Permission.MutateNotes)]
    [HttpDelete("{noteId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int noteId)
    {
        return NoContent();
    }
}
