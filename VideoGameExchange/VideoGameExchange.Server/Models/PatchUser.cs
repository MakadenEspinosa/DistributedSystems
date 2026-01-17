namespace VideoGameExchange.Server.Models;

public class PatchUser
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? StreetAddress { get; set; }
    // Email is intentionally excluded - it cannot be updated
}
