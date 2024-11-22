using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Empleados
    {
        public string Token { get; set; }
        public string IdEmpleado { get; set; }
        public NombreCompleto NombreCompleto { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? Sexo { get; set; }
        public string Contraseña { get; set; }
        public string? area_empleado { get; set; }
        public Direccion? Direccion { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }

        public DateTime? VencimientoCarnet { get; set; }
        public string? FotoEmpleado { get; set; }
        public string? Estado { get; set; }
        public string? UserEmail { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public string? Cedula { get; set; }


        public List<Documentos> Documentos { get; set; }

        public Rol? rol { get; set; }


        public Servicio? servicio { get; set; }
        public List<TareasModel> Tareas { get; set; }


        public Empleados(string ID, NombreCompleto nombre, DateTime fechaAlta, string contraseña, string Email, DateTime fechaNac, string cedula, string celular, Rol rol, Servicio servicio, string Sexo, DateTime vencimiento, string fotoEmpleado, string UserMail, string Estado, Direccion direccion, List<Documentos> documentos)
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
            Tareas = new List<TareasModel>();

        }
        public Empleados(NombreCompleto nombre, DateTime fechaAlta, string contraseña, string Email, DateTime fechaNac, string cedula, string celular)
        {
            NombreCompleto = nombre;
            FechaAlta = fechaAlta;
            Contraseña = contraseña;
            this.Email = Email;
            FechaNacimiento = fechaNac;
            Cedula = cedula;
            Celular = cedula;
            Tareas = new List<TareasModel>();
        }

        public Empleados()
        {

        }

        public Empleados(string idEmpleado, NombreCompleto nombre, DateTime fechaAlta, DateTime fechaNac, string contraseña, string cedula, string estado, List<Documentos> documentos)
        {
            IdEmpleado = idEmpleado;
            NombreCompleto = nombre;
            FechaAlta = fechaAlta;
            FechaNacimiento = fechaNac;
            Contraseña = contraseña;
            Cedula = cedula;
            Estado = estado;
            Documentos = documentos;
        }/*
        public void validarDatos()
        {
            try
            {
                if (NombreCompleto == null)
                {
                    throw new Exception("El nombre completo no puede ser nulo");
                }
                if (NombreCompleto.PrimerNombre == "" || NombreCompleto.PrimerNombre.Length < 3)
                {
                    throw new Exception("El primer nombre debe tener mas de 3 caracteres.");
                }
                if (NombreCompleto.PrimerApellido == "" || NombreCompleto.PrimerApellido.Length < 3)
                {
                    throw new Exception("El primer apellido debe tener mas de 3 caracteres");
                }
                if (Contraseña == "" || Contraseña.Length > 7)
                {
                    throw new Exception("La contraseña debe tener mas de 7 caracteres");
                }
                validarCedula();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void validarCedula()
        {
            int[] Pesos = { 2, 9, 8, 7, 6, 3, 4 };
            try
            {
                string cedula = Cedula;
                // Verificamos que la cédula tenga exactamente 7 dígitos
                if (cedula.Length != 7 || !int.TryParse(cedula, out _))
                {
                    throw new Exception("La cédula debe tener exactamente 7 dígitos.");
                }
                int suma = 0;
                // Multiplicamos cada dígito por su peso correspondiente y sumamos los resultados
                for (int i = 0; i < 7; i++)
                {
                    int digito = int.Parse(cedula[i].ToString());
                    suma += digito * Pesos[i] % 10;
                }
                // Calculamos el dígito verificador
                int digitoVerificador = (10 - suma % 10) % 10;
                if (digitoVerificador != cedula[6])
                {
                    throw new Exception("Cedula invalida, valide los campos");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AgregarDocumento(Documentos documento)
        {
            try
            {
                if (documento == null)
                {
                    throw new Exception("Ingrese una documentacion valida");
                }
                Documentos.Add(documento);
            }
            catch (Exception ex)
            {

            }
        }


        */
    }

}

