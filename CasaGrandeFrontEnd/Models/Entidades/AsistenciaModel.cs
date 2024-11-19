namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{

    public class AsistenciaModel
    {
        public string ConsultaId { get; set; }   // ID de la consulta
        public string EmpleadoId { get; set; }   // ID del técnico/empleado (opcional según el caso)
        public string PacienteId { get; set; }   // ID del paciente (opcional según el caso)
        public string Asistencia { get; set; }   // Asistencia: "Presente" o "No Asiste"
        public string Motivo { get; set; }       // Motivo de inasistencia (opcional si es "No Asiste")
        public IFormFile Certificado { get; set; }
    }

}

