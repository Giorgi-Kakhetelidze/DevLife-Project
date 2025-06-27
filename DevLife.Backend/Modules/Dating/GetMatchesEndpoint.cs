using DevLife.Backend.Common.Extensions;
using DevLife.Backend.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DevLife.Backend.Modules.Dating;

public static class GetMatchesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/dating/matches", async (
            [FromServices] MongoDbContext mongo,
            [FromServices] AppDbContext db,
            HttpContext http) =>
        {
            int userId = http.User.GetUserId();

            // get your profile
            var currentProfile = await mongo.DatingProfiles
                .Find(p => p.UserId == userId)
                .FirstOrDefaultAsync();

            if (currentProfile is null)
                return Results.BadRequest("Dating profile not found");

            // get ALL profiles with gender matching your preference (excluding self)
            var matches = await mongo.DatingProfiles.Find(p =>
                p.UserId != userId &&
                p.Gender.ToLower() == currentProfile.Preference.ToLower()
            ).ToListAsync();

            // get the UserIds
            var userIds = matches.Select(p => p.UserId).ToList();

            // fetch User details from PostgreSQL
            var users = await db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            // merge profiles with user info
            var result = matches.Select(p =>
            {
                var user = users.FirstOrDefault(u => u.Id == p.UserId);
                if (user is null) return null;

                return new
                {
                    ProfileId = p.Id,
                    user.FirstName,
                    user.LastName,
                    Age = CalculateAge(user.BirthDate),
                    user.Zodiac,
                    user.Stack,
                    user.Experience,
                    p.Bio,
                    p.Gender
                };
            })
            .Where(r => r is not null);

            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithTags("DatingApp");
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return age;
    }
}
