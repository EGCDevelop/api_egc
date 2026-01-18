using System.Text.Json.Serialization;

namespace api_egc.Models.Instructors
{
    public class InstructorPayload
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty; 

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty; 

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty; 

        [JsonPropertyName("tel")]
        public string Tel { get; set; } = string.Empty;  

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;  

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;  

        [JsonPropertyName("position")]
        public int Position { get; set; }  

        [JsonPropertyName("area")]
        public string Area { get; set; } = string.Empty;  

        [JsonPropertyName("rol")]
        public int Rol { get; set; } 

        [JsonPropertyName("state")]
        public int State { get; set; } 

        [JsonPropertyName("squads")]
        public List<AssignedSquadRequest> Squads { get; set; } = new();
    }
}
