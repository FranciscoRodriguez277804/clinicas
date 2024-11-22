using System.ComponentModel.DataAnnotations;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class ConsultasModel
    {
        public string ID { get; set; }
        public string ID_Grupo { get; set; }
        public string ID_Horiario { get; set; }
        public string ID_Sesion { get; set; }
        public DateTime? Fecha_Inicio { get; set; }
        public DateTime? Fecha_Fin { get; set; }
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string? NombreUsuario { get; set; }

        [Required]
        [Display(Name = "Fecha de Consulta")]
        public DateTime? Fecha_Consulta { get; set; }

        [Required]
        [Display(Name = "Nombre del Empleado")]
        public string? Nombre_empleado { get; set; }
        public string? ID_Usuario { get; set; }
        public string? IDUsuario_convenio { get; set; }
        public string? Dia { get; set; }
        public string? Hora_inicio { get; set; }
        public string? Convenio { get; set; }
        public string? Hora_fin { get; set; }
        public string? Servicio { get; set; }
        public int? n_consultorio { get; set; }
        public string? Pago { get; set; }
        public string? Estado { get; set; }
        public string? Sucursal { get; set; }
        public string? Asistencia_usuario_recepcion { get; set; }
        public string? Motivo_inasistencia { get; set; }
        public string? Certificado_recepcion { get; set; }
        public string? Comentarios_recepcion { get; set; }
        public string? Asistencia_tecnicos_recepcion { get; set; }
        public string? Motivo_inasistencia_tecnico_recepcion { get; set; }
        public string? Comentarios_tecnicos_recepcion { get; set; }
        public string? UserMail { get; set; }
        public string? Insertdate { get; set; }
        public string? Comentarios_Tecnico_Sesion { get; set; }

        public ConsultasModel()
        {

        }
        public ConsultasModel(string nombreUsuario, string nombreTecnico, string Sucursal, DateTime fechaInicio, DateTime fechaFin, string HoraInicio, string HoraFin)
        {
            this.NombreUsuario = nombreUsuario;
            this.Nombre_empleado = nombreTecnico;
            this.Sucursal = Sucursal;
            this.Fecha_Inicio = fechaInicio;
            this.Fecha_Fin = fechaFin;
            this.Hora_inicio = HoraInicio.ToString();
            this.Hora_fin = HoraFin.ToString();
        }

        public void validarDatos()
        {
            if (this.NombreUsuario == "")
            {
                throw new Exception("Ingrese un usuario valido");
            }
            if (this.Nombre_empleado == "")
            {
                throw new Exception("Ingrese un tecnico valido");
            }
            if (this.Sucursal == "")
            {
                throw new Exception("Ingrese una sucursal");
            }
            if (this.Fecha_Inicio == null)
            {
                throw new Exception("Ingrese una fecha de inicio valida");
            }
            if (this.Fecha_Fin == null)
            {
                throw new Exception("Ingrese una fecha de finalización valida");
            }
            if (this.Hora_inicio == "")
            {
                throw new Exception("Ingrese una hora de inicio valida");
            }
        }
    }

}

