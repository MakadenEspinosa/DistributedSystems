using System.ComponentModel.DataAnnotations;

namespace VideoGameExchange.Server.Models;

public class UserInput
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string StreetAddress { get; set; } = string.Empty;
}
