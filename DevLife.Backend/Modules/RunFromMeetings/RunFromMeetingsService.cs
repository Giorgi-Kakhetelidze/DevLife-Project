

namespace DevLife.Backend.Modules.RunFromMeetings;

public class RunFromMeetingsService
{
    private static readonly Dictionary<string, List<string>> ExcuseTemplates = new()
    {
        ["Technical"] = new()
        {
            "The server caught fire.",
            "Our database crashed unexpectedly.",
            "The network went down due to a power outage."
        },
        ["Personal"] = new()
        {
            "My cat got into production.",
            "I had a family emergency.",
            "My car broke down on the way."
        },
        ["Creative"] = new()
        {
            "AI gained consciousness and needs help.",
            "I was abducted by aliens.",
            "My laptop started speaking in tongues."
        }
    };

    public GenerateExcuseResponse GenerateExcuse(string category, string type)
    {
        if (!ExcuseTemplates.ContainsKey(type))
            throw new ArgumentException("Invalid excuse type.");

        var templates = ExcuseTemplates[type];
        var random = new Random();
        var excuse = templates[random.Next(templates.Count)];

        double score = type switch
        {
            "Technical" => 8.5,
            "Personal" => 7.0,
            "Creative" => 5.0,
            _ => 0
        };

        return new GenerateExcuseResponse(excuse, score);
    }
}
