namespace api_egc.Models.Uniforms.DTO
{
    public class OrderSummary
    {
        public required long Id { get; set; }

        public required long IdIntegrante { get; set; }

        public required int Implementos { get; set; }

        public required DateTime FechaCreacion { get; set; }

        public required int EstadoFinanciero { get; set; }

        public required string NombreEstadoFinanciero { get; set; }

        public required int EstadoEntrega { get; set; }

        public required string NombreEstadoEntrega { get; set; }

        public required decimal GranTotal { get; set; }

        public required decimal MontoAbonado { get; set; }

        public required decimal MontoPendiente { get; set; }
    }
}
