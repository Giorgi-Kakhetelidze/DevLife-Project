namespace DevLife.Backend.Helpers;

public static class ZodiacLuck
{
    public static double GetMultiplier(string zodiac) => zodiac switch
    {
        "Aries" => 2.0,
        "Taurus" => 1.5,
        "Gemini" => 1.2,
        "Cancer" => 1.3,
        "Leo" => 2.0,
        "Virgo" => 1.1,
        "Libra" => 1.4,
        "Scorpio" => 1.3,
        "Sagittarius" => 2.0,
        "Capricorn" => 1.2,
        "Aquarius" => 1.5,
        "Pisces" => 1.4,
        _ => 1.0
    };
}
