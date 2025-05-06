namespace api_egc.Models
{
    public class IntegranteDto
    {
        public required long INTIdIntegrante { get; set; }

		public required string INTNombres { get; set; }

		public required string INTApellidos { get; set; }

        public required int INTEdad { get; set; }

        public required string INTTelefono { get; set; }

        public required long INTESTIdEstablecimiento { get; set; }

        public string? INTEstablecimientoNombre { get; set; }

        public required long INTCARIdCarrera { get; set; }

        public string? INTCarreraNombre { get; set; }

        public required long INTGRAIdGrado { get; set; }

        public string? INTGradoNombre { get; set; }

        public required string INTSeccion { get; set; }

        public required long INTESCIdEscuadra { get; set; }

        public required int INTEsNuevo { get; set; }

        public required string INTNombreEncargado { get; set; }

        public required string INTTelefonoEncargado { get; set; }

        public required int INTEstadoIntegrante { get; set; }

        public required long INTPUIdPuesto { get; set; }
    }
}
