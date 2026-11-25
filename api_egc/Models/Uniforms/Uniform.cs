using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// Table egc_uniforme
    /// </summary>
    public class Uniform
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public required string Name { get; set; }

        [JsonPropertyName("price")]
        [Range(0, 9999999999999999.99)]
        public required decimal Price { get; set; }

        [JsonPropertyName("state")]
        public required byte State { get; set; }

    }
}
