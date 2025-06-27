using DevLife.Backend.Common.Extensions;
using DevLife.Backend.Domain;
using DevLife.Backend.Helpers;
using DevLife.Backend.Modules.Casino;
using DevLife.Backend.Persistence;
using DevLife.Backend.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DevLife.Backend.Features.CodeCasino;

public static class SnippetEndpoint
{
    public static RouteGroupBuilder MapSnippetEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/generate", GenerateSnippets);
        group.MapPost("/choose", ChooseSnippet);
        return group;
    }

    public record GenerateSnippetRequest(string Language, string Experience);
    public record ChooseSnippetRequest(string SnippetId, string SelectedOption, int PointsBet);

    private static async Task<IResult> GenerateSnippets(
    GenerateSnippetRequest request,
    AiSnippetService aiService,
    MongoDbContext mongo,
    AppDbContext db,
    HttpContext ctx)
    {
        int userId = ctx.User.GetUserId();
        var user = await db.Users.FindAsync(userId);
        if (user is null)
            return Results.Unauthorized();

        var (correctSnippet, buggySnippet) = await aiService.GenerateSnippetsAsync(request.Language, request.Experience);

        var pair = new[] { ("A", correctSnippet), ("B", buggySnippet) }
                   .OrderBy(_ => Guid.NewGuid())
                   .ToArray();

        string correctOption = pair[0].Item2 == correctSnippet ? "A" : "B";

        var optionA = pair[0].Item2;
        var optionB = pair[1].Item2;

        var snippet = new CodeSnippet
        {
            UserId = userId,
            Language = request.Language,
            CorrectSnippet = correctSnippet,
            BuggySnippet = buggySnippet
        };

        await mongo.Snippets.InsertOneAsync(snippet);

        return Results.Ok(new
        {
            SnippetId = snippet.Id,
            OptionA = optionA,
            OptionB = optionB,
            CorrectOne = correctOption
        });
    }


    private static async Task<IResult> ChooseSnippet(
    ChooseSnippetRequest request,
    MongoDbContext mongo,
    AppDbContext db,
    HttpContext ctx)
    {
        int userId = ctx.User.GetUserId();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return Results.Unauthorized();

        var snippet = await mongo.Snippets
            .Find(s => s.Id == request.SnippetId && s.UserId == userId)
            .FirstOrDefaultAsync();

        if (snippet is null)
            return Results.NotFound("Snippet not found or not yours.");

        if (request.PointsBet > user.Points)
        {
            return Results.BadRequest("You cannot bet more points than you currently have.");
        }

        if (snippet.HasGuessed)
            return Results.BadRequest("You already guessed.");

        var pair = new[] { snippet.CorrectSnippet, snippet.BuggySnippet }.OrderBy(_ => snippet.Id).ToArray();
        var selected = request.SelectedOption.ToUpperInvariant() switch
        {
            "A" => pair[0],
            "B" => pair[1],
            _ => null
        };

        if (selected is null)
            return Results.BadRequest("Invalid option selected. Choose 'A' or 'B'.");

        var isCorrect = selected == snippet.CorrectSnippet;
        var multiplier = ZodiacLuck.GetMultiplier(user.Zodiac);
        var reward = isCorrect ? request.PointsBet * 2 * multiplier : -request.PointsBet;

        user.Points += (int)reward;
        db.Users.Update(user);
        UpdateStreak.ApplyStreak(user, isCorrect);


        await mongo.Snippets.DeleteOneAsync(s => s.Id == request.SnippetId);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            IsCorrect = isCorrect,
            ChosenOption = request.SelectedOption.ToUpper(),
            PointsChanged = reward,
            NewTotal = user.Points,
            Streak = user.Streak
        });
    }
}
