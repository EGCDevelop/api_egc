namespace api_egc.Models
{
    public class AttendanceChartDTO
    {
        public required long ESCIdEscuadra { get; set; }

        public required string ESCNombre { get; set; }

        public required int TotalIntegrantes { get; set; }

        public required int Asistencias { get; set; }

        public required int Faltan { get; set; }

        public required long EventoId { get; set; }

        public required string NombreEvento { get; set; }
    }
}
