using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace DevLife.Backend.Services;

public class GitHubAnalyzerService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public GitHubAnalyzerService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<List<string>> GetCommitMessagesAsync(string owner, string repo, string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DevLifeAnalyzer");

        var url = $"https://api.github.com/repos/{owner}/{repo}/commits";
        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return new List<string>();

        var json = await response.Content.ReadAsStringAsync();
        var commits = JsonDocument.Parse(json).RootElement.EnumerateArray();

        return commits.Select(c => c.GetProperty("commit").GetProperty("message").GetString() ?? "")
                      .ToList();
    }

    public async Task<object> AnalyzePersonalityAsync(List<string> commits)
    {
        var commitMessages = string.Join("\n", commits.Take(15).Select(c => $"- {c}"));

        var prompt = $$"""
        You are an expert developer personality profiler.

        Analyze the following Git commit messages and return the result strictly in JSON format with the following structure:
        {
          "type": "string - the developer personality type",
          "strengths": ["list of strengths"],
          "weaknesses": ["list of weaknesses"],
          "match": "string - similar famous developer",
          "image_url": "string - a dynamic image URL representing the personality type"
        }

        Use the personality type to format image_url like:
        "https://cdn.devlife.app/cards/{type in snake_case}.png"

        Commit messages:
        {{commitMessages}}
        """;

        var request = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful developer personality analyzer." },
                new { role = "user", content = prompt }
            },
            temperature = 0.8
        };

        var requestJson = JsonSerializer.Serialize(request);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);

        var response = await _http.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
            return new { error = $"❌ Personality analysis failed: {response.StatusCode}" };

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var contentStr = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        try
        {
            var resultJson = JsonSerializer.Deserialize<JsonElement>(contentStr!);
            return new
            {
                type = resultJson.GetProperty("type").GetString(),
                strengths = resultJson.GetProperty("strengths").EnumerateArray().Select(x => x.GetString()).ToList(),
                weaknesses = resultJson.GetProperty("weaknesses").EnumerateArray().Select(x => x.GetString()).ToList(),
                match = resultJson.GetProperty("match").GetString(),
                image_url = resultJson.GetProperty("image_url").GetString()

            };
        }
        catch
        {
            return new { error = "❌ Failed to parse OpenAI response as JSON." };
        }
    }
}