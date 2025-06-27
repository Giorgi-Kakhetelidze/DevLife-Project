using DevLife.Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Backend.Modules.Casino
{
    public static class GetLeaderboard
    {

        public static RouteGroupBuilder MapLeaderboard(this RouteGroupBuilder group)
        {
            group.MapGet("/leaderboard", async (AppDbContext db) =>
            {
                var topUsers = await db.Users
                    .OrderByDescending(u => u.Points)
                    .Select(u => new
                    {
                        u.Username,
                        u.Points,
                        u.Streak
                    })
                    .Take(10)
                    .ToListAsync();

                return Results.Ok(topUsers);
            });

            return group;
        }
    }
}
