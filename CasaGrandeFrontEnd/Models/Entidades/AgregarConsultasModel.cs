
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class AgregarConsultasModel
    {
        public string NombreUsuario { get; set; }

        public string Nombre_empleado { get; set; }

        public string Sucursal { get; set; }

        public DateTime? Fecha_Inicio { get; set; }

        public DateTime? Fecha_Fin { get; set; }

        public string Hora_inicio { get; set; }

        public string Hora_fin { get; set; }

        public string Dia { get; set; }

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
