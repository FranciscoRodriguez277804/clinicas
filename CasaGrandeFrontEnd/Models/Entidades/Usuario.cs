using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Usuario
    {
        public string Id { get; set; }
        public NombreCompleto NombreCompleto { get; set; }
        public string CI { get; set; }
        public string Sexo { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Celular { get; set; }
        public string Celular2 { get; set; }
        public string Telefono { get; set; }

        public Representante Representante { get; set; }
        public Direccion Direccion { get; set; }
        public string Foto { get; set; }
        public string Estado { get; set; }
        /*public Area Area { get; set; }*/
        public DateTime? FechaBaja { get; set; }
        public string Motivo { get; set; }
        public string email { get; set; }
        public DateTime insertdate { get; set; }
        public string disponibilidad_usuarios { get; set; }
        public List<Documentos> Documentos { get; set; }
        public List<Observaciones> Observaciones { get; set; }
        public List<Pagos> Pagos { get; set; }
        public IEnumerable<ConsultasModel> Sesiones { get; set; }
        public UsuarioConvenio Convenio { get; set; }
        public FileMetadata FotoFile { get; set; }
        public Usuario(NombreCompleto NombreCompleto, string CI, string Telefono, DateTime FechaNacimiento, Representante Representante, Direccion Direccion, string Foto, Area Area)
        {
            this.NombreCompleto = NombreCompleto;
            this.CI = CI;
            this.Telefono = Telefono;
            this.FechaNacimiento = FechaNacimiento;
            this.Representante = Representante;
            this.Direccion = Direccion;
            this.Foto = Foto;
            this.Documentos = new List<Documentos>();
            this.Observaciones = new List<Observaciones>();
            this.Pagos = new List<Pagos>();
            this.Sesiones = new List<ConsultasModel>();
            this.FotoFile = new FileMetadata();
        }
        public Usuario(string Id, NombreCompleto NombreCompleto, string CI, string Telefono, DateTime FechaNacimiento, Representante Representante, Direccion Direccion, string Foto, string Email, string estado, DateTime fechaAlta)
        {
            this.Id = Id;
            this.NombreCompleto = NombreCompleto;
            this.CI = CI;
            this.Telefono = Telefono;
            this.FechaNacimiento = FechaNacimiento;
            this.Representante = Representante;
            this.Direccion = Direccion;
            this.Foto = Foto;
            Documentos = new List<Documentos>();
            Observaciones = new List<Observaciones>();
            Pagos = new List<Pagos>();
            Sesiones = new List<ConsultasModel>();
            email = Email;
            this.Estado = estado;
            this.FechaAlta = fechaAlta;
            this.FotoFile = new FileMetadata();
            validarDatos();
        }
        public Usuario() { }
        public void validarDatos()
        {
            try
            {
                if (this.NombreCompleto == null)
                {
                    throw new Exception("El nombre completo no puede ser nulo");
                }
                if (this.NombreCompleto.PrimerNombre == "" || this.NombreCompleto.PrimerNombre.Length < 3)
                {
                    throw new Exception("El primer nombre debe tener mas de 3 caracteres.");
                }
                if (this.NombreCompleto.PrimerApellido == "" || this.NombreCompleto.PrimerApellido.Length < 3)
                {
                    throw new Exception("El primer apellido debe tener mas de 3 caracteres");
                }
                if (this.FechaNacimiento.Date > DateTime.Today.Date)
                {
                    throw new Exception("Ingrese una fecha de nacimiento valida");
                }
                if (this.Representante == null)
                {
                    throw new Exception("Ingrese un representante valido");
                }
                if (this.Direccion == null)
                {
                    throw new Exception("Ingrese una direccion valida");
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
                string cedula = this.CI;
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
                    suma += (digito * Pesos[i]) % 10;
                }
                // Calculamos el dígito verificador
                int digitoVerificador = (10 - (suma % 10)) % 10;
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
                    throw new Exception("Ingrese un documento valido");
                }
                Documentos.Add(documento);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AgregarPago(Pagos pago)
        {
            try
            {
                if (pago == null)
                {
                    throw new Exception("El pago no puede ser nulo");
                }
                this.Pagos.Add(pago);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}

