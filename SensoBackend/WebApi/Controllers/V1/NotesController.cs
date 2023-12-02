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
public sealed class NotesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new note
    /// </summary>
    /// <param name="dto"> Data needed to create note </param>
    /// <response code="201"> Returns newly created note </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If user does not have a senior profile </response>
    [HasPermission(Permission.MutateNotes)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(UpsertNoteDto dto)
    {
        var noteDto = await mediator.Send(
            new CreateNoteRequest { AccountId = this.GetAccountId(), Dto = dto }
        );
        return CreatedAtAction(nameof(ReadOneByNoteId), new { noteId = noteDto.Id }, noteDto);
    }

    /// <summary>
    /// Returns all senior's notes
    /// </summary>
    /// <param name="seniorId"> Id of a senior </param>
    /// <response code="200"> Returns list of notes </response>
    /// <response code="401"> If user is not logged in </response>
    [HasPermission(Permission.ReadNotes)]
    [HttpGet("senior/{seniorId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteListDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReadAllBySeniorId(int seniorId)
    {
        var noteListDto = await mediator.Send(
            new ReadAllNotesBySeniorIdRequest
            {
                AccountId = this.GetAccountId(),
                SeniorId = seniorId
            }
        );

        return Ok(noteListDto);
    }

    /// <summary>
    /// Returns note with a given id
    /// </summary>
    /// <param name="noteId"> Id of a note </param>
    /// <response code="200"> Returns note with a given id </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user is not allowed to access the note with a given id </response>
    /// <response code="404"> If note with a given id was not found </response>
    [HasPermission(Permission.ReadNotes)]
    [HttpGet("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadOneByNoteId(int noteId)
    {
        var noteDto = await mediator.Send(
            new ReadOneNoteByNoteIdRequest { AccountId = this.GetAccountId(), NoteId = noteId }
        );

        return Ok(noteDto);
    }

    /// <summary>
    /// Updates note with a given id
    /// </summary>
    /// <param name="noteId"> Id of a note </param>
    /// <param name="dto"> Data needed to update the note </param>
    /// <response code="200"> Returns updated note </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user is not allowed to access the note with a given id </response>
    /// <response code="404"> If note with a given id was not found </response>
    [HasPermission(Permission.MutateNotes)]
    [HttpPut("{noteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int noteId, UpsertNoteDto dto)
    {
        var noteDto = await mediator.Send(
            new UpdateNoteRequest
            {
                AccountId = this.GetAccountId(),
                NoteId = noteId,
                Dto = dto
            }
        );

        return Ok(noteDto);
    }

    /// <summary>
    /// Deletes note with a given id
    /// </summary>
    /// <param name="noteId"> Id of a note </param>
    /// <response code="204"> Returns updated note </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="403"> If user is not allowed to access the note with a given id </response>
    /// <response code="404"> If note with a given id was not found </response>
    [HasPermission(Permission.MutateNotes)]
    [HttpDelete("{noteId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int noteId)
    {
        await mediator.Send(
            new DeleteNoteRequest { AccountId = this.GetAccountId(), NoteId = noteId }
        );
        return NoContent();
    }
}
