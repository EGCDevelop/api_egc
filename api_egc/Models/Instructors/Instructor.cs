namespace api_egc.Models.Instructors
{
    public class Instructor
    {
        public required long Id { get; set; }

        public required string Nombre { get; set; }

        public required string Apellido { get; set; }

        public required string Telefono { get; set; }

        public required string Correo { get; set; }

        public required int Estado { get; set; }

        public required string Usuario { get; set; }

        public required int IdPuesto { get; set; }

        public required string Area { get; set; }

        public required int Rol { get; set; }

        public required string Escuadras { get; set; }
    }
}
