namespace api_egc.Models
{
    public class Event
    {
        public required long EVEId { get; set; }

        public required string EVETitulo { get; set; }

        public required string EVEDescripcion { get; set; }

        public required DateTime EVEFechaEvento { get; set; }

        public required TimeSpan EVEHoraEntradaComandantes { get; set; }

        public required int EVESoloComandantes { get; set; }

        public TimeSpan? EVEHoraEntradaIntegrantes { get; set; }

        public required string EVEUsuarioCreacion { get; set; }

        public required DateTime EVEFechaCreacion { get; set; }

        public string? EVEUsuarioModificacion { get; set; }

        public DateTime? EVEFechaModificacon { get; set; }

        public required int EVEBandaGeneral { get; set; }
}
}
