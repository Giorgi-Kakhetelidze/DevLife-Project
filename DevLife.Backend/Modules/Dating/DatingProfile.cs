using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Backend.Domain;

public class DatingProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int UserId { get; set; }

    public string Bio { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Preference { get; set; } = string.Empty;
    public List<int> LikedUserIds { get; set; } = new();

    public DatingProfile(int userId, string gender, string bio)
    {
        UserId = userId;
        Gender = gender;
        Preference = gender.ToLower() switch
        {
            "male" => "female",
            "female" => "male",
            _ => "any"
        };
        Bio = bio;
        LikedUserIds = new();

    }
}
