namespace DevLife.Backend.Modules.RunFromMeetings;

public record GenerateExcuseRequest(string Category, string Type);

public record GenerateExcuseResponse(string Excuse, double BelievabilityScore);
