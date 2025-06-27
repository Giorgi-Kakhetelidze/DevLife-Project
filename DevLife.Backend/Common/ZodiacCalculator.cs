namespace DevLife.Backend.Common;

public static class ZodiacCalculator
{
    public static string CalculateZodiac(DateTime birthDate)
    {
        int day = birthDate.Day;
        int month = birthDate.Month;

        return (month, day) switch
        {
            (1, <= 19) => "Capricorn",
            (1, _) => "Aquarius",
            (2, <= 18) => "Aquarius",
            (2, _) => "Pisces",
            (3, <= 20) => "Pisces",
            (3, _) => "Aries",
            (4, <= 19) => "Aries",
            (4, _) => "Taurus",
            (5, <= 20) => "Taurus",
            (5, _) => "Gemini",
            (6, <= 20) => "Gemini",
            (6, _) => "Cancer",
            (7, <= 22) => "Cancer",
            (7, _) => "Leo",
            (8, <= 22) => "Leo",
            (8, _) => "Virgo",
            (9, <= 22) => "Virgo",
            (9, _) => "Libra",
            (10, <= 22) => "Libra",
            (10, _) => "Scorpio",
            (11, <= 21) => "Scorpio",
            (11, _) => "Sagittarius",
            (12, <= 21) => "Sagittarius",
            _ => "Capricorn"
        };
    }

    public static string GetZodiacEmoji(string zodiac) => zodiac switch
    {
        "Aries" => "♈",         
        "Taurus" => "♉",        
        "Gemini" => "♊",        
        "Cancer" => "♋",        
        "Leo" => "♌",           
        "Virgo" => "♍",         
        "Libra" => "♎",         
        "Scorpio" => "♏",       
        "Sagittarius" => "♐",   
        "Capricorn" => "♑",     
        "Aquarius" => "♒",      
        "Pisces" => "♓",        
        _ => "✨"               
    };



    public static (string Prediction, string CodingTip, string LuckyTech) GetDailyInfo(string zodiac)
    {
        return zodiac switch
        {
            "Aries" => ("Expect progress through persistence.", "Try using async/await patterns.", "TypeScript"),
            "Taurus" => ("Stability leads to breakthroughs.", "Review your unit tests.", "PostgreSQL"),
            "Gemini" => ("Share your knowledge today.", "Read up on design patterns.", "JavaScript"),
            "Cancer" => ("Collaboration will open new doors.", "Refactor for readability.", "MongoDB"),
            "Leo" => ("Your leadership shines—take charge.", "Optimize UI/UX today.", "Flutter"),
            "Virgo" => ("Attention to detail pays off.", "Write documentation.", "C#"),
            "Libra" => ("Balance tasks wisely.", "Contribute to an open source project.", "Python"),
            "Scorpio" => ("Trust your instincts in debugging.", "Explore performance tuning.", "Rust"),
            "Sagittarius" => ("Adventure awaits—learn a new tech.", "Try solving a LeetCode problem.", "Go"),
            "Capricorn" => ("Discipline drives success.", "Automate repetitive tasks.", "Docker"),
            "Aquarius" => ("Innovate and inspire others.", "Experiment with a new framework.", "React"),
            "Pisces" => ("Creativity will solve a problem.", "Sketch out your software ideas.", "Kotlin"),
            _ => ("Keep learning and growing.", "Clean up your codebase.", "Python")
        };
    }
}
