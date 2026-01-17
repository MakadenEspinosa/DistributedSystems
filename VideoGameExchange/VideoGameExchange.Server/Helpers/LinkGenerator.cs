using Microsoft.AspNetCore.Mvc;
using VideoGameExchange.Server.Models;

namespace VideoGameExchange.Server.Helpers;

public static class HateoasLinkGenerator
{
    public static List<Link> GenerateGameLinks(string gameId, IUrlHelper urlHelper)
    {
        return new List<Link>
        {
            new Link
            {
                Href = urlHelper.Link("GetGameById", new { gameid = gameId }) ?? string.Empty,
                Rel = "self",
                Method = "GET"
            },
            new Link
            {
                Href = urlHelper.Link("UpdateGame", new { gameid = gameId }) ?? string.Empty,
                Rel = "update",
                Method = "PUT"
            },
            new Link
            {
                Href = urlHelper.Link("PatchGame", new { gameid = gameId }) ?? string.Empty,
                Rel = "partial-update",
                Method = "PATCH"
            },
            new Link
            {
                Href = urlHelper.Link("DeleteGame", new { gameid = gameId }) ?? string.Empty,
                Rel = "delete",
                Method = "DELETE"
            },
            new Link
            {
                Href = urlHelper.Link("GetAllGames", null) ?? string.Empty,
                Rel = "collection",
                Method = "GET"
            }
        };
    }

    public static List<Link> GenerateGameLinksWithUser(string gameId, string userId, IUrlHelper urlHelper)
    {
        var links = GenerateGameLinks(gameId, urlHelper);
        
        links.Add(new Link
        {
            Href = urlHelper.Link("GetUserById", new { userid = userId }) ?? string.Empty,
            Rel = "owner",
            Method = "GET"
        });

        return links;
    }

    public static List<Link> GenerateUserLinks(string userId, IUrlHelper urlHelper)
    {
        return new List<Link>
        {
            new Link
            {
                Href = urlHelper.Link("GetUserById", new { userid = userId }) ?? string.Empty,
                Rel = "self",
                Method = "GET"
            },
            new Link
            {
                Href = urlHelper.Link("PatchUser", new { userid = userId }) ?? string.Empty,
                Rel = "update",
                Method = "PATCH"
            },
            new Link
            {
                Href = urlHelper.Link("DeleteUser", new { userid = userId }) ?? string.Empty,
                Rel = "delete",
                Method = "DELETE"
            },
            new Link
            {
                Href = urlHelper.Link("GetAllUsers", null) ?? string.Empty,
                Rel = "collection",
                Method = "GET"
            },
            new Link
            {
                Href = urlHelper.Link("GetAllGames", null) + $"?userid={userId}",
                Rel = "games",
                Method = "GET"
            }
        };
    }

    public static List<Link> GenerateCollectionLinks(IUrlHelper urlHelper, string getLinkName, string addLinkName)
    {
        return new List<Link>
        {
            new Link
            {
                Href = urlHelper.Link(getLinkName, null) ?? string.Empty,
                Rel = "self",
                Method = "GET"
            },
            new Link
            {
                Href = urlHelper.Link(addLinkName, null) ?? string.Empty,
                Rel = "create",
                Method = "POST"
            }
        };
    }
}
