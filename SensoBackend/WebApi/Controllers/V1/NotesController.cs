using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Notes.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator) => _mediator = mediator;

    [HasPermission(Permission.MutateNotes)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(UpsertNoteDto dto)
    {
        return CreatedAtAction(nameof(ReadById), new { noteId = 0 }, default(NoteDto));
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteListDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReadAll()
    {
        return Ok(new NoteListDto { Notes = new List<NoteDto>() });
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadById(int noteId)
    {
        return Ok(default(NoteDto));
    }

    [HasPermission(Permission.MutateNotes)]
    [HttpPut("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int noteId, UpsertNoteDto dto)
    {
        return Ok(default(NoteDto));
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
