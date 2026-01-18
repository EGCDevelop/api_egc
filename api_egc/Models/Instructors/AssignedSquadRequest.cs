using System.Text.Json.Serialization;

namespace api_egc.Models.Instructors
{
    public class AssignedSquadRequest
    {
        [JsonPropertyName("squadId")]
        public int SquadId { get; set; }

        [JsonPropertyName("squadName")]
        public string SquadName { get; set; } = string.Empty;

        [JsonPropertyName("isPrincipal")]
        public bool IsPrincipal { get; set; }
    }
}
