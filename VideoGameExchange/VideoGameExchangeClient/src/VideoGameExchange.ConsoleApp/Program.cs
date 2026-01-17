using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VideoGameExchange.Client.Api;
using VideoGameExchange.Client.Client;
using VideoGameExchange.Client.Extensions;
using VideoGameExchange.Client.Model;

namespace VideoGameExchange.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var api = host.Services.GetRequiredService<IDefaultApi>();

            try
            {
                Console.WriteLine("=== Video Game Exchange API Client Demo ===\n");

                // 1. Create a new game
                Console.WriteLine("1. Creating a new game...");
                var newGame = new GameInput(
                    title: "The Legend of Zelda: Breath of the Wild",
                    platform: "switch",
                    condition: new Option<string?>("Mint"),
                    year: new Option<int?>(2017)
                );

                var addResponse = await api.AddGameAsync(newGame);
                var createdGame = addResponse.Created();
                
                if (createdGame != null)
                {
                    Console.WriteLine($"   ✓ Game created with ID: {createdGame.Id}");
                    Console.WriteLine($"   - Title: {createdGame.Title}");
                    Console.WriteLine($"   - Platform: {createdGame.Platform}");
                    Console.WriteLine($"   - Condition: {createdGame.Condition}");
                    Console.WriteLine($"   - Year: {createdGame.Year}\n");

                    // 2. Update the game
                    Console.WriteLine("2. Updating the game...");
                    var updateInput = new GameInput(
                        title: "The Legend of Zelda: Breath of the Wild - Special Edition",
                        platform: "switch",
                        condition: new Option<string?>("Fair"),
                        year: new Option<int?>(2017)
                    );

                    var updateResponse = await api.UpdateGameAsync(createdGame.Id, updateInput);
                    var updatedGame = updateResponse.Ok();
                    
                    if (updatedGame != null)
                    {
                        Console.WriteLine($"   ✓ Game updated");
                        Console.WriteLine($"   - New Title: {updatedGame.Title}");
                        Console.WriteLine($"   - New Condition: {updatedGame.Condition}\n");
                    }

                    // 3. Partial update using PATCH
                    Console.WriteLine("3. Partially updating the game condition...");
                    var patchGame = new PatchGame(
                        condition: new Option<string?>("Mint")
                    );

                    var patchResponse = await api.PartialGameUpdateAsync(createdGame.Id, patchGame);
                    var patchedGame = patchResponse.Ok();
                    
                    if (patchedGame != null)
                    {
                        Console.WriteLine($"   ✓ Game patched");
                        Console.WriteLine($"   - Condition updated to: {patchedGame.Condition}\n");
                    }

                    // 4. Delete the game
                    Console.WriteLine("4. Deleting the game...");
                    var deleteResponse = await api.DeleteGameAsync(createdGame.Id);
                    Console.WriteLine($"   ✓ Game deleted (Status: {deleteResponse.StatusCode})\n");
                }

                Console.WriteLine("=== Demo completed successfully! ===");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"\n❌ API Error: {ex.Message}");
                Console.WriteLine($"   Status Code: {ex.StatusCode}");
                Console.WriteLine($"   Reason: {ex.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureApi((context, services, options) =>
                {
                    options.AddApiHttpClients(client =>
                    {
                        // TODO: Set your API server URL here
                        client.BaseAddress = new Uri("http://localhost:5000");
                    },
                    builder =>
                    {
                        builder
                            .AddRetryPolicy(2)
                            .AddTimeoutPolicy(TimeSpan.FromSeconds(10))
                            .AddCircuitBreakerPolicy(10, TimeSpan.FromSeconds(30));
                    });
                });
    }
}
