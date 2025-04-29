namespace api_egc.Models
{
    public class Versiones
    {
        public required long VERIdVersion { get; set; }

		public required string VERNumero { get; set; }

		public required int VERObligatoria { get; set; }

		public required string  VERDescripcion { get; set; }
    }
}
