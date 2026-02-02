namespace VideoGameExchange.Server.Models;

public class TradeOfferResource
{
    public string Id { get; set; } = string.Empty;
    public string OfferorUserId { get; set; } = string.Empty;
    public string OfferorUserName { get; set; } = string.Empty;
    public string OffereeUserId { get; set; } = string.Empty;
    public string OffereeUserName { get; set; } = string.Empty;
    public List<GameResource> OfferedGames { get; set; } = new();
    public List<GameResource> RequestedGames { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Link> Links { get; set; } = new();

    public static TradeOfferResource FromTradeOffer(TradeOffer tradeOffer, User offeror, User offeree, 
                                                     List<Game> offeredGames, List<Game> requestedGames)
    {
        return new TradeOfferResource
        {
            Id = tradeOffer.Id,
            OfferorUserId = tradeOffer.OfferorUserId,
            OfferorUserName = offeror.Name,
            OffereeUserId = tradeOffer.OffereeUserId,
            OffereeUserName = offeree.Name,
            OfferedGames = offeredGames.Select(g => GameResource.FromGame(g)).ToList(),
            RequestedGames = requestedGames.Select(g => GameResource.FromGame(g)).ToList(),
            Status = tradeOffer.Status,
            Message = tradeOffer.Message,
            CreatedAt = tradeOffer.CreatedAt,
            UpdatedAt = tradeOffer.UpdatedAt
        };
    }
}

public class UserWithGamesResource
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<GameResource> Games { get; set; } = new();
    public List<Link> Links { get; set; } = new();
}
