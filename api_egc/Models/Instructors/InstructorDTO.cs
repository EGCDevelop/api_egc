namespace api_egc.Models.Instructors
{
    public class InstructorDTO
    {
        public required long INSId { get; set; }
		
		public required string INSNombre { get; set; }

		public required string INSApellido { get; set; }

		public string? INSTelefono { get; set; }

		public string? INSCorreo { get; set; }

		public required string INSPassword { get; set; }

        public required int INTPIId { get; set; }

		public required string INSArea { get; set; }
    }
}
