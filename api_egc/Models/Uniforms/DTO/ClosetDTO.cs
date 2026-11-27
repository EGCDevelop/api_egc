namespace api_egc.Models.Uniforms.DTO
{
    public class ClosetDTO
    {

        public required int Id { get; set; }

        public required int UniformId { get; set; }

        public required string Name { get; set; }

        public required long SquadId { get; set; }

        public required decimal Price { get; set; }
    }
}
