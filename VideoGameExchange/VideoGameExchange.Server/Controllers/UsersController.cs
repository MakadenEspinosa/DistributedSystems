using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VideoGameExchange.Server.Models;
using VideoGameExchange.Server.Services;
using VideoGameExchange.Server.Helpers;

namespace VideoGameExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllUsers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        
        var userResources = users.Select(u =>
        {
            var resource = UserResource.FromUser(u);
            resource.Links = HateoasLinkGenerator.GenerateUserLinks(u.Id, Url);
            return resource;
        }).ToList();

        var collectionLinks = HateoasLinkGenerator.GenerateCollectionLinks(Url, "GetAllUsers", "AddUser");

        var response = new
        {
            items = userResources,
            _links = collectionLinks
        };
        
        return Ok(response);
    }

    [HttpPost(Name = "AddUser")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResource>> AddUser([FromBody] UserInput input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid user input"
            });
        }

        // Check if email already exists
        var existingUser = await _userService.GetUserByEmailAsync(input.Email);
        if (existingUser != null)
        {
            return BadRequest(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = 400,
                Detail = "A user with this email already exists"
            });
        }

        var user = await _userService.AddUserAsync(input);
        _logger.LogInformation("Created user with ID: {UserId}", user?.Id);
        
        var resource = UserResource.FromUser(user!);
        resource.Links = HateoasLinkGenerator.GenerateUserLinks(user!.Id, Url);
        
        return CreatedAtRoute("GetUserById", new { userid = user?.Id }, resource);
    }

    [HttpGet("{userid}", Name = "GetUserById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResource>> GetUser(string userid)
    {
        var user = await _userService.GetUserAsync(userid);
        
        if (user == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"User with ID {userid} not found"
            });
        }

        var resource = UserResource.FromUser(user);
        resource.Links = HateoasLinkGenerator.GenerateUserLinks(user.Id, Url);
        
        return Ok(resource);
    }

    [HttpPatch("{userid}", Name = "PatchUser")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResource>> PatchUser(string userid, [FromBody] PatchUser patch)
    {
        // Check if the authenticated user is trying to update their own profile
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != userid)
        {
            return StatusCode(403, new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = 403,
                Detail = "You can only update your own profile"
            });
        }

        var user = await _userService.PatchUserAsync(userid, patch);
        
        if (user == null)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"User with ID {userid} not found"
            });
        }

        _logger.LogInformation("Updated user with ID: {UserId}", userid);
        
        var resource = UserResource.FromUser(user);
        resource.Links = HateoasLinkGenerator.GenerateUserLinks(user.Id, Url);
        
        return Ok(resource);
    }

    [HttpDelete("{userid}", Name = "DeleteUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Problem), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(string userid)
    {
        // Check if the authenticated user is trying to delete their own profile
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != userid)
        {
            return StatusCode(403, new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Forbidden",
                Status = 403,
                Detail = "You can only delete your own profile"
            });
        }

        var deleted = await _userService.DeleteUserAsync(userid);
        
        if (!deleted)
        {
            return NotFound(new Problem
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = $"User with ID {userid} not found"
            });
        }

        _logger.LogInformation("Deleted user with ID: {UserId}", userid);
        return NoContent();
    }
}
