using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Clinica
    {
        public string NombreClinica;

        public Direccion Direccion;

        public List<Empleados> Empleados;

        public List<Area> Areas;

        public List<UsuarioConvenio> Usuarios;

        public List<ConsultorioModel> Consultorios;

        public List<ConsultasModel> Consultas;

        public List<UsuarioConvenio> Convenios;

        public Clinica(string NombreClinica, Direccion direccion)
        {
            this.NombreClinica = NombreClinica;
            this.Direccion = direccion;
            validarDatos();
        }
        public Clinica() { }

        public void validarDatos()
        {
            try
            {
                if (this.NombreClinica.Length < 3)
                {
                    throw new Exception("El nombre de clinica debe contener mas de 3 caracteres");
                }

                if (this.Direccion.Calle == "")
                {
                    throw new Exception("Ingrese una calle correcta");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AgregarAreas(string NombreArea, string CodigoArea)
        {
            Area nuevaArea = new Area(NombreArea, CodigoArea);
            Areas.Add(nuevaArea);
        }

        public void AgregarConsultorios(ConsultorioModel consultorios)
        {
            ConsultorioModel nuevoConsultorio = consultorios;
            Consultorios.Add(nuevoConsultorio);
        }

        public void AgregarUsuarios(UsuarioConvenio usuario)
        {
            UsuarioConvenio nuevoUsuario = usuario;
            Usuarios.Add(nuevoUsuario);
        }

        public void AgregarConsultas(ConsultasModel consultas)
        {
            ConsultasModel nuevoConsulta = consultas;
            Consultas.Add(nuevoConsulta);
        }

        public void AgregarConvenios(UsuarioConvenio Conveni)
        {
            UsuarioConvenio nuevoConvenio = Conveni;
            Convenios.Add(nuevoConvenio);
        }

        public void AgregarEmpleados(Empleados empleados)
        {
            Empleados nuevoEmpleado = empleados;
            Empleados.Add(nuevoEmpleado);
        }

    }


}

