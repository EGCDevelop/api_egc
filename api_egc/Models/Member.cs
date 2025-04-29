namespace api_egc.Models
{
    public class Member
    {
        public required long INTIdIntegrante { get; set; }

		public required string INTNombres { get; set; }

		public required string INTApellidos { get; set; }

		public required long INTESCIdEscuadra { get; set; }

		public required long INTPUIdPuesto { get; set; }

        public required string INTPassword { get; set; }
    }
}
