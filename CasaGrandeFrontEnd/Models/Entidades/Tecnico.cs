using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Tecnico : Empleados
    {
        public Area Area;

        public List<ConsultasModel> Consultas;

        public Tecnico(string ID, NombreCompleto nombre, DateTime fechaAlta, string contraseña, string Email, DateTime fechaNac, string cedula, string celular, Rol rol, Servicio servicio, string Sexo, DateTime vencimiento, Area area, string fotoEmpleado, string UserMail, string Estado, Direccion direccion, List<Documentos> documentos) : base(ID, nombre, fechaAlta, contraseña, Email, fechaNac, cedula, celular, rol, servicio, Sexo, vencimiento, fotoEmpleado, UserMail, Estado, direccion, documentos)
        {
            this.IdEmpleado = ID;
            this.NombreCompleto = nombre;
            this.FechaAlta = fechaAlta;
            this.Contraseña = contraseña;
            this.Email = Email;
            this.FechaNacimiento = fechaNac;
            this.Cedula = cedula;
            this.Celular = celular;
            this.rol = rol;
            this.Consultas = new List<ConsultasModel>();
            this.Area = area;

        }
        public Tecnico() : base() { }
    }

}

