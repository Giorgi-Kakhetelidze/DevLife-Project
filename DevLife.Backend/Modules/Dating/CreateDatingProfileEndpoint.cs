using DevLife.Backend.Common.Extensions;
using DevLife.Backend.Domain;
using DevLife.Backend.Persistence;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.IO;

namespace DevLife.Backend.Features.Dating;

public static class CreateDatingProfileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/dating/profile", async (
            [FromBody] Request request,
            [FromServices] AppDbContext db,
            [FromServices] MongoDbContext mongo,
            HttpContext http) =>
        {
            int userId = http.User.GetUserId();

            var user = await db.Users.FindAsync(userId);
            if (user is null)
                return Results.NotFound("User not found");

            var profile = new DatingProfile(userId, request.Gender, request.Bio);
            await mongo.DatingProfiles.InsertOneAsync(profile);

            return Results.Ok(new
            {
                user.Username,
                profile.Gender,
                profile.Preference,
                profile.Bio
            });
        })
        .RequireAuthorization()
        .WithTags("DatingApp");
    }

    public record Request(string Gender, string Bio);
}
