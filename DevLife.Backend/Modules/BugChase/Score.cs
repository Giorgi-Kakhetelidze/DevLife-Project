using DevLife.Backend.Domain;

namespace DevLife.Backend.Modules.BugChase;

public class Score
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int HighScore { get; set; }    

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public User? User { get; set; } 

}
