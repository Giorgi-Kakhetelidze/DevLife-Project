using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DevLife.Backend.Services;

public class AiSnippetService
{
    private readonly string _apiKey;
    private readonly HttpClient _http;

    public AiSnippetService(IConfiguration config)
    {
        _apiKey = config["OpenAI:ApiKey"]!;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    private string CleanSnippet(string snippet)
    {
        var lines = snippet.Split('\n')
            .Where(line => !line.TrimStart().StartsWith("//"))
            .ToArray();
        return string.Join('\n', lines).Trim();
    }

    public async Task<(string Correct, string Buggy)> GenerateSnippetsAsync(string language, string experience)
    {
        var prompt = $"""
        Generate two {language} code snippets for a {experience}-level developer, but do not return with comments which is correct or bugged one, generate only code. 
        One of them should be correct, and the other should contain a bug.
        Do not return in response which is correct or bugged one, never!. Return only the two code snippets, separated by the delimiter ###.
        Do not use markdown.```.

        Example format:
        ###
        def add(a, b): return a + b
        ###
        def add(a, b): print(a + b)

        Now generate the code:
        """;

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a code generator." },
                new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 800
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var message = doc.RootElement
                         .GetProperty("choices")[0]
                         .GetProperty("message")
                         .GetProperty("content")
                         .GetString();

        var parts = message!.Split("###", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            throw new Exception("Invalid AI response");

        var correctClean = CleanSnippet(parts[0]);
        var buggyClean = CleanSnippet(parts[1]);

        return (correctClean, buggyClean);
    }
}
