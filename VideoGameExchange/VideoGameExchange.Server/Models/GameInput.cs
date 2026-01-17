using System.ComponentModel.DataAnnotations;

namespace VideoGameExchange.Server.Models;

public class GameInput
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Platform { get; set; } = string.Empty;
    
    public string? Condition { get; set; }
    
    [Range(1970, 2100)]
    public int? Year { get; set; }
    
    public string? Publisher { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
}
