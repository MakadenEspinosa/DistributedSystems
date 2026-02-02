using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace VideoGameExchange.Server.Services;

public class ExchangeService
{
    private readonly IMongoCollection<TradeOffer> _tradeOffersCollection;
    private readonly IMongoCollection<Game> _gamesCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public ExchangeService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _tradeOffersCollection = mongoDatabase.GetCollection<TradeOffer>(mongoDBSettings.Value.TradeOffersCollectionName);
        _gamesCollection = mongoDatabase.GetCollection<Game>(mongoDBSettings.Value.GamesCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoDBSettings.Value.UsersCollectionName);
    }

    // Get all users with their games
    public async Task<List<UserWithGamesResource>> GetAllUsersWithGamesAsync()
    {
        var users = await _usersCollection.Find(_ => true).ToListAsync();
        var result = new List<UserWithGamesResource>();

        foreach (var user in users)
        {
            var games = await _gamesCollection.Find(g => g.UserId == user.Id).ToListAsync();
            result.Add(new UserWithGamesResource
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Games = games.Select(g => GameResource.FromGame(g)).ToList()
            });
        }

        return result;
    }

    // Search for games across all users
    public async Task<List<GameResource>> SearchGamesAsync(string? title, string? platform, 
                                                            string? publisher, int? year)
    {
        var filterBuilder = Builders<Game>.Filter;
        var filters = new List<FilterDefinition<Game>>();

        if (!string.IsNullOrEmpty(title))
            filters.Add(filterBuilder.Regex(g => g.Title, new MongoDB.Bson.BsonRegularExpression(title, "i")));
        
        if (!string.IsNullOrEmpty(platform))
            filters.Add(filterBuilder.Regex(g => g.Platform, new MongoDB.Bson.BsonRegularExpression(platform, "i")));
        
        if (!string.IsNullOrEmpty(publisher))
            filters.Add(filterBuilder.Regex(g => g.Publisher, new MongoDB.Bson.BsonRegularExpression(publisher, "i")));
        
        if (year.HasValue)
            filters.Add(filterBuilder.Eq(g => g.Year, year));

        var combinedFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;
        var games = await _gamesCollection.Find(combinedFilter).ToListAsync();
        
        return games.Select(g => GameResource.FromGame(g)).ToList();
    }

    // Create a trade offer
    public async Task<TradeOffer?> CreateTradeOfferAsync(TradeOfferInput input)
    {
        // Validate that all games exist and belong to the correct users
        var offeredGames = await _gamesCollection.Find(g => input.OfferedGameIds.Contains(g.Id)).ToListAsync();
        var requestedGames = await _gamesCollection.Find(g => input.RequestedGameIds.Contains(g.Id)).ToListAsync();

        if (offeredGames.Count != input.OfferedGameIds.Count)
            return null; // Some offered games don't exist

        if (requestedGames.Count != input.RequestedGameIds.Count)
            return null; // Some requested games don't exist

        // Verify offered games belong to offeror
        if (offeredGames.Any(g => g.UserId != input.OfferorUserId))
            return null;

        // Verify requested games belong to offeree
        if (requestedGames.Any(g => g.UserId != input.OffereeUserId))
            return null;

        var tradeOffer = new TradeOffer
        {
            OfferorUserId = input.OfferorUserId,
            OffereeUserId = input.OffereeUserId,
            OfferedGameIds = input.OfferedGameIds,
            RequestedGameIds = input.RequestedGameIds,
            Message = input.Message,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _tradeOffersCollection.InsertOneAsync(tradeOffer);
        return tradeOffer;
    }

    // Get trade offer by ID with full details
    public async Task<TradeOfferResource?> GetTradeOfferAsync(string id)
    {
        var tradeOffer = await _tradeOffersCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        if (tradeOffer == null)
            return null;

        return await BuildTradeOfferResource(tradeOffer);
    }

    // Get all trade offers for a user (sent or received)
    public async Task<List<TradeOfferResource>> GetTradeOffersForUserAsync(string userId)
    {
        var tradeOffers = await _tradeOffersCollection.Find(t => 
            t.OfferorUserId == userId || t.OffereeUserId == userId).ToListAsync();

        var result = new List<TradeOfferResource>();
        foreach (var offer in tradeOffers)
        {
            var resource = await BuildTradeOfferResource(offer);
            if (resource != null)
                result.Add(resource);
        }

        return result;
    }

    // Accept a trade offer and execute the trade
    public async Task<TradeOffer?> AcceptTradeOfferAsync(string id, string userId)
    {
        var tradeOffer = await _tradeOffersCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        
        if (tradeOffer == null)
            return null;

        // Only the offeree can accept
        if (tradeOffer.OffereeUserId != userId)
            return null;

        // Can only accept pending offers
        if (tradeOffer.Status != "pending")
            return null;

        // Execute the trade - swap game ownership
        var updateTasks = new List<Task>();

        // Transfer offered games to offeree
        foreach (var gameId in tradeOffer.OfferedGameIds)
        {
            updateTasks.Add(_gamesCollection.UpdateOneAsync(
                g => g.Id == gameId,
                Builders<Game>.Update.Set(g => g.UserId, tradeOffer.OffereeUserId)
            ));
        }

        // Transfer requested games to offeror
        foreach (var gameId in tradeOffer.RequestedGameIds)
        {
            updateTasks.Add(_gamesCollection.UpdateOneAsync(
                g => g.Id == gameId,
                Builders<Game>.Update.Set(g => g.UserId, tradeOffer.OfferorUserId)
            ));
        }

        await Task.WhenAll(updateTasks);

        // Update trade offer status
        tradeOffer.Status = "accepted";
        tradeOffer.UpdatedAt = DateTime.UtcNow;
        await _tradeOffersCollection.ReplaceOneAsync(t => t.Id == id, tradeOffer);

        return tradeOffer;
    }

    // Reject a trade offer
    public async Task<TradeOffer?> RejectTradeOfferAsync(string id, string userId)
    {
        var tradeOffer = await _tradeOffersCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        
        if (tradeOffer == null)
            return null;

        // Only the offeree can reject
        if (tradeOffer.OffereeUserId != userId)
            return null;

        // Can only reject pending offers
        if (tradeOffer.Status != "pending")
            return null;

        tradeOffer.Status = "rejected";
        tradeOffer.UpdatedAt = DateTime.UtcNow;
        await _tradeOffersCollection.ReplaceOneAsync(t => t.Id == id, tradeOffer);

        return tradeOffer;
    }

    // Cancel a trade offer (by offeror)
    public async Task<TradeOffer?> CancelTradeOfferAsync(string id, string userId)
    {
        var tradeOffer = await _tradeOffersCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        
        if (tradeOffer == null)
            return null;

        // Only the offeror can cancel
        if (tradeOffer.OfferorUserId != userId)
            return null;

        // Can only cancel pending offers
        if (tradeOffer.Status != "pending")
            return null;

        tradeOffer.Status = "cancelled";
        tradeOffer.UpdatedAt = DateTime.UtcNow;
        await _tradeOffersCollection.ReplaceOneAsync(t => t.Id == id, tradeOffer);

        return tradeOffer;
    }

    private async Task<TradeOfferResource?> BuildTradeOfferResource(TradeOffer tradeOffer)
    {
        var offeror = await _usersCollection.Find(u => u.Id == tradeOffer.OfferorUserId).FirstOrDefaultAsync();
        var offeree = await _usersCollection.Find(u => u.Id == tradeOffer.OffereeUserId).FirstOrDefaultAsync();
        
        if (offeror == null || offeree == null)
            return null;

        var offeredGames = await _gamesCollection.Find(g => tradeOffer.OfferedGameIds.Contains(g.Id)).ToListAsync();
        var requestedGames = await _gamesCollection.Find(g => tradeOffer.RequestedGameIds.Contains(g.Id)).ToListAsync();

        return TradeOfferResource.FromTradeOffer(tradeOffer, offeror, offeree, offeredGames, requestedGames);
    }
}
