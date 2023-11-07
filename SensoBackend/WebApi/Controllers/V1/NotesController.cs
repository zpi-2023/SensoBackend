using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Modules.Notes;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(UpsertNoteDto dto)
    {
        var noteDto = await _mediator.Send(new CreateNoteRequest(this.GetAccountId(), dto));
        return CreatedAtAction(nameof(ReadOneByNoteId), new { noteId = noteDto.Id }, noteDto);
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet("senior/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteListDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReadAllBySeniorId(int seniorId)
    {
        var noteListDto = await _mediator.Send(
            new ReadAllNotesBySeniorIdRequest
            {
                AccountId = this.GetAccountId(),
                SeniorId = seniorId
            }
        );

        return Ok(noteListDto);
    }

    [HasPermission(Permission.ReadNotes)]
    [HttpGet("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadOneByNoteId(int noteId)
    {
        var noteDto = await _mediator.Send(
            new ReadOneNoteByNoteIdRequest { AccountId = this.GetAccountId(), NoteId = noteId }
        );

        return Ok(noteDto);
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
        var noteDto = await _mediator.Send(
            new UpdateNoteRequest
            {
                AccountId = this.GetAccountId(),
                NoteId = noteId,
                Dto = dto
            }
        );

        return Ok(noteDto);
    }

    [HasPermission(Permission.MutateNotes)]
    [HttpDelete("{noteId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int noteId)
    {
        await _mediator.Send(
            new DeleteNoteRequest { AccountId = this.GetAccountId(), NoteId = noteId }
        );
        return NoContent();
    }
}
