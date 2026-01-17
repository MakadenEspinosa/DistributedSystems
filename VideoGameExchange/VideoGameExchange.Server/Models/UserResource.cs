namespace VideoGameExchange.Server.Models;

public class UserResource
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public List<Link> Links { get; set; } = new();

    public static UserResource FromUser(User user)
    {
        return new UserResource
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            StreetAddress = user.StreetAddress
        };
    }
}
