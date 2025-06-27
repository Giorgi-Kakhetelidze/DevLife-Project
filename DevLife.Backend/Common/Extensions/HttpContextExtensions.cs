using System.Security.Claims;

namespace DevLife.Backend.Common.Extensions;

public static class HttpContextExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdStr))
            throw new UnauthorizedAccessException("User ID claim not found in token.");

        if (!int.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("User ID in token is not a valid integer.");

        return userId;
    }
}
