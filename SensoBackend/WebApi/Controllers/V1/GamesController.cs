using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensoBackend.Application.Common.Pagination;
using SensoBackend.Application.Modules.Games;
using SensoBackend.Application.Modules.Games.Contracts;
using SensoBackend.WebApi.Authorization;
using SensoBackend.WebApi.Authorization.Data;

namespace SensoBackend.WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class GamesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns user's best score in a given game
    /// </summary>
    /// <param name="gameName"> The name of a game </param>
    /// <response code="200"> Returns dashboard for a given senior </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If the name of a game was not found </response>
    [HasPermission(Permission.ManageGames)]
    [HttpGet("{gameName}/score")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScoreDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadBestScore(string gameName)
    {
        var score = await mediator.Send(
            new ReadUserBestScoreRequest { AccountId = this.GetAccountId(), GameName = gameName }
        );

        return Ok(score);
    }

    /// <summary>
    /// Updates user's best score in the given game if the new score is better
    /// </summary>
    /// <param name="gameName"> The name of a game </param>
    /// <param name="dto"> Achieved score </param>
    /// <response code="200"> Returns dashboard for a given senior </response>
    /// <response code="400"> If validation failed </response>
    /// <response code="401"> If user is not logged in </response>
    /// <response code="404"> If the name of a game was not found </response>
    [HasPermission(Permission.ManageGames)]
    [HttpPost("{gameName}/score")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TryUpdateBestScore(string gameName, ScoreDto dto)
    {
        await mediator.Send(
            new TryUpdateUserBestScoreRequest
            {
                AccountId = this.GetAccountId(),
                GameName = gameName,
                Dto = dto
            }
        );

        return NoContent();
    }

    /// <summary>
    /// Returns leaderbord
    /// </summary>
    /// <param name="gameName"> The name of a game </param>
    /// <response code="200"> Returns part of the leaderboard </response>
    /// <response code="404"> If the name of a game was not found </response>
    [HttpGet("{gameName}/leaderboard")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(PaginatedDto<LeaderboardEntryDto>)
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReadLeaderboard(
        string gameName,
        [FromQuery] PaginationQuery query
    )
    {
        var leaderboard = await mediator.Send(
            new ReadLeaderboardRequest { GameName = gameName, Pagination = query }
        );

        return Ok(leaderboard);
    }
}
