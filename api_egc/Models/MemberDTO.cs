namespace api_egc.Models
{
    public class MemberDTO
    {
        public required long INTIdIntegrante { get; set; }

		public required string INTNombres { get; set; }

		public required string INTApellidos { get; set; }

		public string? INTTelefono { get; set; }

		public required long INTESTIdEstablecimiento { get; set; }

		public required string ESTNombreEstablecimiento { get; set; }

        public string? INTEstablecimientoNombre { get; set; }

        public required long INTCARIdCarrera { get; set; }

        public required string CARNombreCarrera { get; set; }

        public string? INTCarreraNombre { get; set; }

        public required long INTGRAIdGrado { get; set; }

        public required string GRANombreGrado { get; set; }

        public string? INTGradoNombre { get; set; }

        public required string INTSeccion { get; set; }

        public required long INTESCIdEscuadra { get; set; }

        public required string ESCNombre { get; set; }

        public required int INTEsNuevo { get; set; }

        public string? INTNombreEncargado { get; set; }

        public string? INTTelefonoEncargado { get; set; }

        public required int INTEstadoIntegrante { get; set; }

        public required long INTPUIdPuesto { get; set; }

        public required string PUNombre { get; set; }
    }
}
