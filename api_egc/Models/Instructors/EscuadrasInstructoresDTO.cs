namespace api_egc.Models.Instructors
{
    public class EscuadrasInstructoresDTO
    {
        public required int Id { get; set; }

		public required int IdEscuadra { get; set; }

		public required long IdInstructor { get; set; }

		public required int Principal { get; set; }
        
        public required string Nombre { get; set; }
    }
}
