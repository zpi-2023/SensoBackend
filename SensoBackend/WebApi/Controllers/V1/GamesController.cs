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
