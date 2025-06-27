using DevLife.Backend.Common.Extensions;
using DevLife.Backend.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DevLife.Backend.Modules.Dating
{
    public static class SwipeProfileEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/dating/swipe", async (
                [FromBody] SwipeRequest request,
                [FromServices] MongoDbContext mongo,
                [FromServices] AppDbContext db,
                HttpContext http) =>
            {
                int currentUserId = http.User.GetUserId();

                var myProfile = await mongo.DatingProfiles
                    .Find(p => p.UserId == currentUserId)
                    .FirstOrDefaultAsync();

                if (myProfile is null)
                    return Results.BadRequest("Your profile not found");

                var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Username == request.TargetUsername);
                if (targetUser is null)
                    return Results.BadRequest("User with that username not found");

                var targetProfile = await mongo.DatingProfiles
                    .Find(p => p.UserId == targetUser.Id)
                    .FirstOrDefaultAsync();

                if (targetProfile is null)
                    return Results.BadRequest("Target profile not found");

                if (!request.Liked)
                    return Results.Ok(new { match = false });

                if (myProfile.LikedUserIds == null)
                    myProfile.LikedUserIds = new List<int>();

                if (myProfile.LikedUserIds.Contains(targetProfile.UserId))
                    return Results.Ok(new { match = false });

                myProfile.LikedUserIds.Add(targetProfile.UserId);
                await mongo.DatingProfiles.ReplaceOneAsync(p => p.Id == myProfile.Id, myProfile);

                bool isMatch = targetProfile.LikedUserIds?.Contains(currentUserId) == true;

                return Results.Ok(new { match = isMatch });
            })
            .RequireAuthorization()
            .WithTags("DatingApp");
        }

        public record SwipeRequest(string TargetUsername, bool Liked);
    }
}
