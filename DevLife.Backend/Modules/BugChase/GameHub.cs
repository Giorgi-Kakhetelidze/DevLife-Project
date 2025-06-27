using Microsoft.AspNetCore.SignalR;

namespace DevLife.Backend.Modules.BugChase;

public class GameHub : Hub
{
    public async Task SendScore(string username, int score)
    {
        await Clients.All.SendAsync("ReceiveScore", username, score);
    }
}
