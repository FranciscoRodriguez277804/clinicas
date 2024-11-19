using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Recepcionista : Empleados
    {
        public Recepcionista(string ID, NombreCompleto nombre, DateTime fechaAlta, string contraseña, string Email, DateTime fechaNac, string cedula, string celular, Rol rol, Servicio servicio, string Sexo, DateTime vencimiento, string fotoEmpleado, string UserMail, string Estado, Direccion direccion, List<Documentos> documentos) : base(ID, nombre, fechaAlta, contraseña, Email, fechaNac, cedula, celular, rol, servicio, Sexo, vencimiento, fotoEmpleado, UserMail, Estado, direccion, documentos)
        {
            this.NombreCompleto = nombre;
            this.FechaAlta = fechaAlta;
            this.Contraseña = contraseña;
            this.Email = Email;
            this.FechaNacimiento = fechaNac;
            this.Cedula = cedula;
            this.Celular = celular;
            this.rol = rol;
            this.servicio = servicio;
            this.IdEmpleado = ID;
            this.Documentos = documentos;
            this.Sexo = Sexo;
            this.Estado = Estado;
            this.VencimientoCarnet = vencimiento;
            this.FotoEmpleado = fotoEmpleado;

        }
        public Recepcionista() { }

    }

}

