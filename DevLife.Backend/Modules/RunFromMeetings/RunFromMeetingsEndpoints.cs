using DevLife.Backend.Modules.RunFromMeetings;
using Microsoft.AspNetCore.Mvc;

namespace DevLife.Backend.Modules.RunFromMeetings;

public static class RunFromMeetingsEndpoints
{
    public static IEndpointRouteBuilder MapRunFromMeetingsEndpoints(this IEndpointRouteBuilder app)
    {
        var service = new RunFromMeetingsService();

        app.MapPost("/generate-excuse", ([FromBody] GenerateExcuseRequest req) =>
        {
            try
            {
                var result = service.GenerateExcuse(req.Category, req.Type);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithTags("Run From Meetings");

        return app;
    }
}
