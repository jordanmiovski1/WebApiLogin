namespace WebApi.Entities;
using System.Text.Json.Serialization;

public class User
{
    public int id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}