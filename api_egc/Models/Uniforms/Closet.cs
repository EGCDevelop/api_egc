namespace api_egc.Models.Uniforms
{
    public class Closet
    {
        public required int Id { get; set; }

        public required int UniformId { get; set; }

        public required long SquadId { get; set; }

        public required byte Estado { get; set; }
    }
}
