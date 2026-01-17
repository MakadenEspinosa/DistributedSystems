namespace VideoGameExchange.Server.Models;

public class GameResource
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? Condition { get; set; }
    public int? Year { get; set; }
    public string? Publisher { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<Link> Links { get; set; } = new();

    public static GameResource FromGame(Game game)
    {
        return new GameResource
        {
            Id = game.Id,
            Title = game.Title,
            Platform = game.Platform,
            Condition = game.Condition,
            Year = game.Year,
            Publisher = game.Publisher,
            UserId = game.UserId
        };
    }
}
