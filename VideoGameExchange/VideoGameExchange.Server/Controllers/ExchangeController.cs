using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Services;

namespace VideoGameExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ExchangeController : ControllerBase
{
    private readonly ExchangeService _exchangeService;
    private readonly ILogger<ExchangeController> _logger;

    public ExchangeController(ExchangeService exchangeService, ILogger<ExchangeController> logger)
    {
        _exchangeService = exchangeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users and their games for browsing
    /// </summary>
    [HttpGet("users", Name = "GetUsersWithGames")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<UserWithGamesResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserWithGamesResource>>> GetUsersWithGames()
    {
        _logger.LogInformation("Fetching all users with their games");
        var users = await _exchangeService.GetAllUsersWithGamesAsync();
        return Ok(users);
    }

    /// <summary>
    /// Search for games by various criteria
    /// </summary>
    [HttpGet("search", Name = "SearchGames")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<GameResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GameResource>>> SearchGames(
        [FromQuery] string? title,
        [FromQuery] string? platform,
        [FromQuery] string? publisher,
        [FromQuery] int? year)
    {
        _logger.LogInformation("Searching games with criteria - Title: {Title}, Platform: {Platform}, Publisher: {Publisher}, Year: {Year}",
            title, platform, publisher, year);
        
        var games = await _exchangeService.SearchGamesAsync(title, platform, publisher, year);
        return Ok(games);
    }

    /// <summary>
    /// Create a new trade offer
    /// </summary>
    [HttpPost("offers", Name = "CreateTradeOffer")]
    [ProducesResponseType(typeof(TradeOfferResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TradeOfferResource>> CreateTradeOffer([FromBody] TradeOfferInput input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid trade offer data"
            });
        }

        _logger.LogInformation("Creating trade offer from {OfferorId} to {OffereeId}",
            input.OfferorUserId, input.OffereeUserId);

        var tradeOffer = await _exchangeService.CreateTradeOfferAsync(input);
        
        if (tradeOffer == null)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid trade offer. Verify that all games exist and belong to the correct users."
            });
        }

        var resource = await _exchangeService.GetTradeOfferAsync(tradeOffer.Id);
        return CreatedAtRoute("GetTradeOffer", new { id = tradeOffer.Id }, resource);
    }

    /// <summary>
    /// Get a specific trade offer by ID
    /// </summary>
    [HttpGet("offers/{id}", Name = "GetTradeOffer")]
    [ProducesResponseType(typeof(TradeOfferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TradeOfferResource>> GetTradeOffer(string id)
    {
        _logger.LogInformation("Fetching trade offer {Id}", id);
        var tradeOffer = await _exchangeService.GetTradeOfferAsync(id);
        
        if (tradeOffer == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"Trade offer with ID {id} not found"
            });
        }

        return Ok(tradeOffer);
    }

    /// <summary>
    /// Get all trade offers for a user (sent and received)
    /// </summary>
    [HttpGet("offers/user/{userId}", Name = "GetUserTradeOffers")]
    [ProducesResponseType(typeof(List<TradeOfferResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TradeOfferResource>>> GetUserTradeOffers(string userId)
    {
        _logger.LogInformation("Fetching trade offers for user {UserId}", userId);
        var offers = await _exchangeService.GetTradeOffersForUserAsync(userId);
        return Ok(offers);
    }

    /// <summary>
    /// Accept a trade offer (executes the trade)
    /// </summary>
    [HttpPut("offers/{id}/accept", Name = "AcceptTradeOffer")]
    [ProducesResponseType(typeof(TradeOfferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TradeOfferResource>> AcceptTradeOffer(string id, [FromQuery] string userId)
    {
        _logger.LogInformation("User {UserId} accepting trade offer {Id}", userId, id);
        
        var tradeOffer = await _exchangeService.AcceptTradeOfferAsync(id, userId);
        
        if (tradeOffer == null)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Could not accept trade offer. Verify that the offer exists, you are the recipient, and it is still pending."
            });
        }

        var resource = await _exchangeService.GetTradeOfferAsync(id);
        return Ok(resource);
    }

    /// <summary>
    /// Reject a trade offer
    /// </summary>
    [HttpPut("offers/{id}/reject", Name = "RejectTradeOffer")]
    [ProducesResponseType(typeof(TradeOfferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TradeOfferResource>> RejectTradeOffer(string id, [FromQuery] string userId)
    {
        _logger.LogInformation("User {UserId} rejecting trade offer {Id}", userId, id);
        
        var tradeOffer = await _exchangeService.RejectTradeOfferAsync(id, userId);
        
        if (tradeOffer == null)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Could not reject trade offer. Verify that the offer exists, you are the recipient, and it is still pending."
            });
        }

        var resource = await _exchangeService.GetTradeOfferAsync(id);
        return Ok(resource);
    }

    /// <summary>
    /// Cancel a trade offer (by the offeror)
    /// </summary>
    [HttpPut("offers/{id}/cancel", Name = "CancelTradeOffer")]
    [ProducesResponseType(typeof(TradeOfferResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TradeOfferResource>> CancelTradeOffer(string id, [FromQuery] string userId)
    {
        _logger.LogInformation("User {UserId} cancelling trade offer {Id}", userId, id);
        
        var tradeOffer = await _exchangeService.CancelTradeOfferAsync(id, userId);
        
        if (tradeOffer == null)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Could not cancel trade offer. Verify that the offer exists, you created it, and it is still pending."
            });
        }

        var resource = await _exchangeService.GetTradeOfferAsync(id);
        return Ok(resource);
    }
}
