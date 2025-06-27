namespace DevLife.Backend.Modules.Auth;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder app)
    {
        app.MapRegisterUser();
        return app;
    }
}
