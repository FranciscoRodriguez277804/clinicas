namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class PacientesYUsuariosModel
    {
        public IEnumerable<UsuarioConvenio> Pacientes { get; set; }
        public IEnumerable<Empleados> Empleados { get; set; }
    }
}
