using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogAPI.Core.Model;

#pragma warning disable CS8618
[ExcludeFromCodeCoverage]
public class User
{
    [BsonId]
    public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

    [Required]
    public string UserName { get; set; }

    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.com$", ErrorMessage = "The Email field is not a valid e-mail address.")]
    public string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password should be Alphanumeric and atleast 8 characters.")]
    public string Password { get; set; }
}
#pragma warning restore CS8618
