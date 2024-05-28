using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static BlogAPI.Core.Utility.Helper;

namespace BlogAPI.Core.Model;

#pragma warning disable CS8618
[ExcludeFromCodeCoverage]
public class Blog
{
    [BsonId]
    public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

    public ObjectId UserId { get; set; }

    [MinLength(20)]
    public string BlogName { get; set; }

    [Required]
    [MinLength(20)]
    public string Category { get; set; }

    [Required]
    [MinWords(1000)]
    public string Article { get; set; }

    [Required]
    public string AuthorName { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}
#pragma warning restore CS8618
