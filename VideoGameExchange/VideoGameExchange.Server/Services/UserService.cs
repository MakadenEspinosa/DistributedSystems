using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace VideoGameExchange.Server.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoDBSettings.Value.UsersCollectionName);
    }

    public async Task<User?> AddUserAsync(UserInput input)
    {
        var user = new User
        {
            Name = input.Name,
            Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
            Email = input.Email,
            StreetAddress = input.StreetAddress
        };

        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> GetUserAsync(string id)
    {
        return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> PatchUserAsync(string id, PatchUser patch)
    {
        var user = await GetUserAsync(id);
        if (user == null)
            return null;

        var updateDefinition = Builders<User>.Update;
        var updates = new List<UpdateDefinition<User>>();

        if (patch.Name != null)
            updates.Add(updateDefinition.Set(u => u.Name, patch.Name));
        if (patch.Password != null)
            updates.Add(updateDefinition.Set(u => u.Password, BCrypt.Net.BCrypt.HashPassword(patch.Password)));
        if (patch.StreetAddress != null)
            updates.Add(updateDefinition.Set(u => u.StreetAddress, patch.StreetAddress));
        // Email is intentionally not included - it cannot be updated

        if (updates.Any())
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            await _usersCollection.UpdateOneAsync(u => u.Id == id, combinedUpdate);
        }

        return await GetUserAsync(id);
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }
}
