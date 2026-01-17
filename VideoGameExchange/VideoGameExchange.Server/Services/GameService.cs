using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace VideoGameExchange.Server.Services;

public class GameService
{
    private readonly IMongoCollection<Game> _gamesCollection;

    public GameService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _gamesCollection = mongoDatabase.GetCollection<Game>(mongoDBSettings.Value.GamesCollectionName);
    }

    public async Task<Game?> AddGameAsync(GameInput input)
    {
        var game = new Game
        {
            Title = input.Title,
            Platform = input.Platform,
            Condition = input.Condition,
            Year = input.Year,
            Publisher = input.Publisher,
            UserId = input.UserId
        };

        await _gamesCollection.InsertOneAsync(game);
        return game;
    }

    public async Task<Game?> GetGameAsync(string id)
    {
        return await _gamesCollection.Find(g => g.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Game?> UpdateGameAsync(string id, GameInput input)
    {
        var game = new Game
        {
            Id = id,
            Title = input.Title,
            Platform = input.Platform,
            Condition = input.Condition,
            Year = input.Year,
            Publisher = input.Publisher,
            UserId = input.UserId
        };

        var result = await _gamesCollection.ReplaceOneAsync(g => g.Id == id, game);
        return result.MatchedCount > 0 ? game : null;
    }

    public async Task<Game?> PatchGameAsync(string id, PatchGame patch)
    {
        var game = await GetGameAsync(id);
        if (game == null)
            return null;

        var updateDefinition = Builders<Game>.Update;
        var updates = new List<UpdateDefinition<Game>>();

        if (patch.Title != null)
            updates.Add(updateDefinition.Set(g => g.Title, patch.Title));
        if (patch.Platform != null)
            updates.Add(updateDefinition.Set(g => g.Platform, patch.Platform));
        if (patch.Condition != null)
            updates.Add(updateDefinition.Set(g => g.Condition, patch.Condition));
        if (patch.Year.HasValue)
            updates.Add(updateDefinition.Set(g => g.Year, patch.Year));
        if (patch.Publisher != null)
            updates.Add(updateDefinition.Set(g => g.Publisher, patch.Publisher));

        if (updates.Any())
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            await _gamesCollection.UpdateOneAsync(g => g.Id == id, combinedUpdate);
        }

        return await GetGameAsync(id);
    }

    public async Task<bool> DeleteGameAsync(string id)
    {
        var result = await _gamesCollection.DeleteOneAsync(g => g.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        return await _gamesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetGamesByUserIdAsync(string userId)
    {
        return await _gamesCollection.Find(g => g.UserId == userId).ToListAsync();
    }
}
