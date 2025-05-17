namespace api_egc.Models
{
    public class AsistenciaDto
    {
        public long INTIdIntegrante { get; set; }
        
        public required string INTNombres { get; set; }
        
        public required string INTApellidos { get; set; }
        
        public long? ASIIdAsistencia { get; set; }  
        
        public DateTime? ASIFechaAsistencia { get; set; }
        
        public required int Asistencia { get; set; }

        public required long ASIEVEId { get; set; }

        public required long ASIINTIdIntegranteRegistro { get; set; }

        public required int ASIEsExtraordinaria { get; set; }

        public string? ASIComentario { get; set; }

        public DateTime? ASIFechaRegistroExtraordinaria { get; set; }



    }
}
