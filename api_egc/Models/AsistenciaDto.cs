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

        public DateTime? ASIFechaSalida { get; set; }

        public string? ASIUsuarioSalida { get; set; }

        public string? ASIComentarioSalida { get; set; }

        public string? ASIJustificacionFalta { get; set; }

        public byte? ASITieneJustificacion { get; set; }

        public string? ASIUsuarioRegistroJustificacion { get; set; }

        public override string ToString()
        {
            return $@"AsistenciaDto Details:
                ---------------------------------
                Integrante: {INTIdIntegrante} - {INTNombres} {INTApellidos}
                ID Asistencia: {ASIIdAsistencia?.ToString() ?? "N/A"}
                Fecha Asistencia: {ASIFechaAsistencia?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Sin registro"}
                Estado Asistencia: {(Asistencia == 1 ? "Presente" : "Ausente")}
                Evento ID: {ASIEVEId}
                Registrado por: {ASIINTIdIntegranteRegistro}
                Es Extraordinaria: {(ASIEsExtraordinaria == 1 ? "Sí" : "No")}
                Comentario: {ASIComentario ?? "Sin comentarios"}
                Registro Extraord: {ASIFechaRegistroExtraordinaria?.ToString() ?? "N/A"}
                ASIJustificacionFalta: {ASIJustificacionFalta?.ToString() ?? "N/A"}
                ASITieneJustificacion: {ASITieneJustificacion?.ToString() ?? "N/A"}
                ASIUsuarioRegistroJustificacion: {ASIUsuarioRegistroJustificacion?.ToString() ?? "N/A"}
                ---------------------------------
                Salida: {ASIFechaSalida?.ToString() ?? "Sin salida"}
                Usuario Salida: {ASIUsuarioSalida ?? "N/A"}
                Comentario Salida: {ASIComentarioSalida ?? "N/A"}";
        }

    }
}
