namespace api_egc.Models
{
    public class AsistenciaDto
    {
        public long INTIdIntegrante { get; set; }
        
        public required string INTNombres { get; set; }
        
        public required string INTApellidos { get; set; }
        
        public long? ASIIdAsistencia { get; set; }  
        
        public DateTime? ASIFechaAsistencia { get; set; }
        
        public int Asistencia { get; set; }

    }
}
