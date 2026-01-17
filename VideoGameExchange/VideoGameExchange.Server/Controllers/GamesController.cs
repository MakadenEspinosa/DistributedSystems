using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Services;
using VideoGameExchange.Server.Helpers;

namespace VideoGameExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(GameService gameService, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllGames")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllGames([FromQuery] string? userid = null)
    {
        IEnumerable<Game> games;
        
        if (!string.IsNullOrEmpty(userid))
        {
            games = await _gameService.GetGamesByUserIdAsync(userid);
        }
        else
        {
            games = await _gameService.GetAllGamesAsync();
        }
        
        var gameResources = games.Select(g =>
        {
            var resource = GameResource.FromGame(g);
            resource.Links = HateoasLinkGenerator.GenerateGameLinksWithUser(g.Id, g.UserId, Url);
            return resource;
        }).ToList();

        var collectionLinks = HateoasLinkGenerator.GenerateCollectionLinks(Url, "GetAllGames", "AddGame");

        var response = new
        {
            items = gameResources,
            _links = collectionLinks
        };
        
        return Ok(response);
    }

    [HttpPost(Name = "AddGame")]
    [ProducesResponseType(typeof(GameResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameResource>> AddGame([FromBody] GameInput input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid game input"
            });
        }

        var game = await _gameService.AddGameAsync(input);
        _logger.LogInformation("Created game with ID: {GameId}", game?.Id);
        
        var resource = GameResource.FromGame(game!);
        resource.Links = HateoasLinkGenerator.GenerateGameLinksWithUser(game!.Id, game.UserId, Url);
        
        return CreatedAtRoute("GetGameById", new { gameid = game?.Id }, resource);
    }

    [HttpGet("{gameid}", Name = "GetGameById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GameResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResource>> GetGame(string gameid)
    {
        var game = await _gameService.GetGameAsync(gameid);
        
        if (game == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Game with ID '{gameid}' not found"
            });
        }

        var resource = GameResource.FromGame(game);
        resource.Links = HateoasLinkGenerator.GenerateGameLinksWithUser(game.Id, game.UserId, Url);
        
        return Ok(resource);
    }

    [HttpPut("{gameid}", Name = "UpdateGame")]
    [ProducesResponseType(typeof(GameResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResource>> UpdateGame(string gameid, [FromBody] GameInput input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid game input"
            });
        }

        // Check if game exists and user owns it
        var existingGame = await _gameService.GetGameAsync(gameid);
        if (existingGame == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Game with ID '{gameid}' not found"
            });
        }

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (existingGame.UserId != currentUserId)
        {
            return StatusCode(403, new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = 403,
                Detail = "You can only update your own games"
            });
        }

        var game = await _gameService.UpdateGameAsync(gameid, input);

        _logger.LogInformation("Updated game with ID: {GameId}", gameid);
        
        var resource = GameResource.FromGame(game);
        resource.Links = HateoasLinkGenerator.GenerateGameLinksWithUser(game.Id, game.UserId, Url);
        
        return Ok(resource);
    }

    [HttpPatch("{gameid}", Name = "PatchGame")]
    [Consumes("application/merge-patch+json")]
    [ProducesResponseType(typeof(GameResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResource>> PartialGameUpdate(string gameid, [FromBody] PatchGame patch)
    {
        // Check if game exists
        var existingGame = await _gameService.GetGameAsync(gameid);
        if (existingGame == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Game with ID '{gameid}' not found"
            });
        }

        // Check ownership
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (existingGame.UserId != currentUserId)
        {
            return StatusCode(403, new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = 403,
                Detail = "You can only update your own games"
            });
        }

        var game = await _gameService.PatchGameAsync(gameid, patch);

        _logger.LogInformation("Patched game with ID: {GameId}", gameid);
        
        var resource = GameResource.FromGame(game);
        resource.Links = HateoasLinkGenerator.GenerateGameLinksWithUser(game.Id, game.UserId, Url);
        
        return Ok(resource);
    }

    [HttpDelete("{gameid}", Name = "DeleteGame")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGame(string gameid)
    {
        // Check if game exists
        var existingGame = await _gameService.GetGameAsync(gameid);
        if (existingGame == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Game with ID '{gameid}' not found"
            });
        }

        // Check ownership
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (existingGame.UserId != currentUserId)
        {
            return StatusCode(403, new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = 403,
                Detail = "You can only delete your own games"
            });
        }

        var deleted = await _gameService.DeleteGameAsync(gameid);
        if (!deleted)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Game with ID '{gameid}' not found"
            });
        }

        _logger.LogInformation("Deleted game with ID: {GameId}", gameid);
        return NoContent();
    }
}
