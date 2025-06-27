using DevLife.Backend.Common.Extensions;
using DevLife.Backend.Modules.BugChase;
using DevLife.Backend.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class BugChaseEndpoints
{
    public static IEndpointRouteBuilder MapBugChaseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/bugchase/submit", async (AppDbContext db, HttpContext ctx, [FromBody] ScoreDto dto) =>
        {
            var userId = ctx.User.GetUserId();

            var existingScore = await db.Scores.FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingScore is null)
            {
                var score = new Score
                {
                    UserId = userId,
                    HighScore = dto.HighScore,
                };
                db.Scores.Add(score);
            }
            else
            {
                if (dto.HighScore > existingScore.HighScore)
                    existingScore.HighScore = dto.HighScore;
            }

            await db.SaveChangesAsync();
            return Results.Ok();
        })
        .RequireAuthorization()
        .WithTags("Bug Chase");

        app.MapGet("/bugchase/leaderboard", async (AppDbContext db) =>
        {
            var topScores = await db.Scores
                .Include(s => s.User)
                .OrderByDescending(s => s.HighScore)
                .Take(10)
                .Select(s => new
                {
                    Username = s.User.Username,
                    Score = s.HighScore
                })
                .ToListAsync();

            return Results.Ok(topScores);
        })
        .WithTags("Bug Chase");

        app.MapGet("/bugchase/highscore", async (AppDbContext db, HttpContext ctx) =>
        {
            var userId = ctx.User.GetUserId();

            var score = await db.Scores
                .Where(s => s.UserId == userId)
                .Select(s => new { s.HighScore })
                .FirstOrDefaultAsync();

            if (score == null)
                return Results.NotFound("No score found for the user.");

            string achievement = score.HighScore switch
            {
                >= 1000 => "Bug Master",
                >= 500 => "Bug Hunter",
                _ => "Bug Novice"
            };

            return Results.Ok(new
            {
                HighScore = score.HighScore,
                Achievement = achievement
            });
        })
        .RequireAuthorization()
        .WithTags("Bug Chase");

        return app;
    }

    public record ScoreDto(int HighScore);
}
