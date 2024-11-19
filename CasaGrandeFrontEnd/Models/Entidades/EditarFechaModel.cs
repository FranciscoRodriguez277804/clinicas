namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class EditarFechaModel
    {
        public string Id { get; set; }
        public DateTime? NuevaFecha { get; set; }
        public string? Asistencia_usuario_recepcion { get; set; }
        public string? Motivo_inasistencia { get; set; }
        public string? Certificado_recepcion { get; set; }
        public string? Comentarios_recepcion { get; set; }
        public string? Asistencia_tecnicos_recepcion { get; set; }
        public string? Motivo_inasistencia_tecnico_recepcion { get; set; }
        public string? Comentarios_tecnicos_recepcion { get; set; }
        public string? Hora_inicio { get; set; }
        public string? Hora_fin { get; set; }

        public DateTime? fecha_Inicio { get; set; }
        public DateTime? fecha_Fin { get; set; }
        public string? sucursal { get; set; }
    }
}
