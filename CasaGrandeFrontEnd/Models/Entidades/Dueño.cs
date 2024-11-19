using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Dueño : Empleados
    {
        public Dueño(string ID, NombreCompleto nombre, DateTime fechaAlta, string contraseña, string Email, DateTime fechaNac, string cedula, string celular, Rol rol, Servicio servicio, string Sexo, DateTime vencimiento, string fotoEmpleado, string UserMail, string Estado, Direccion direccion, List<Documentos> documentos) : base(ID, nombre, fechaAlta, contraseña, Email, fechaNac, cedula, celular, rol, servicio, Sexo, vencimiento, fotoEmpleado, UserMail, Estado, direccion, documentos)
        {
            this.IdEmpleado = ID;
            this.NombreCompleto = nombre;
            this.FechaAlta = fechaAlta;
            this.Contraseña = contraseña;
            this.Email = Email;
            this.Cedula = cedula;
            this.FechaNacimiento = fechaNac;
            this.Celular = celular;
            this.rol = rol;
        }
        public Dueño() : base() { }
    }

}

