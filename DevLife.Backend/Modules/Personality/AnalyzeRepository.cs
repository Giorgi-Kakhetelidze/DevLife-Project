using DevLife.Backend.Services;

namespace DevLife.Backend.Modules.Personality;

public static class AnalyzeRepository
{
    public static void MapAnalyzeRepoEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/personality/analyze", async (
            GitHubAnalyzerService analyzer,
            AnalyzeRepoRequest req) =>
        {
            var commits = await analyzer.GetCommitMessagesAsync(req.Owner, req.Repo, req.Token);

            if (commits.Count == 0)
                return Results.BadRequest(new { error = "No commits found or repository inaccessible." });

            var result = await analyzer.AnalyzePersonalityAsync(commits);
            return Results.Ok(result);
        })
        .WithTags("Personality analyzer");

    }

    public record AnalyzeRepoRequest(string Owner, string Repo, string Token);
}