namespace api_egc.Models.Uniforms.DTO
{
    public class SaveOrdenDTO
    {
        public required UniformOrder Uniform { get; set; }

        public required List<UniformOrderDetails> Details { get; set; }
    }
}
