namespace api_egc.Models.Uniforms.DTO
{
    public class ClosetSummary
    {
        public required long Id { get; set; }

        public required string Escuadra { get; set; }

        public required int Cantidad { get; set; }

        public required decimal Precio { get; set; }
    }
}
