using System.Text.Json.Serialization;

namespace api_egc.Models.Uniforms.DTO
{
    public class SaveOrdenDTO
    {
        [JsonPropertyName("uniform")]
        public required UniformOrder Uniform { get; set; }

        [JsonPropertyName("details")]
        public required List<UniformOrderDetails> Details { get; set; }
    }
}
