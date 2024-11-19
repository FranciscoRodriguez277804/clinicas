namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public record NombreCompleto
    {
        public string PrimerNombre;

        public string SegundoNombre;

        public string PrimerApellido;

        public string SegundoApellido;


        public NombreCompleto(string PrimerNombre, string PrimerApellido)
        {
            this.PrimerNombre = PrimerNombre;
            this.SegundoNombre = null;
            this.PrimerApellido = PrimerApellido;
            this.SegundoApellido = null;
        }
        public NombreCompleto(string PrimerNombre, string segundoNombre, string PrimerApellido, string SegundoApellido)
        {
            this.PrimerNombre = PrimerNombre;
            this.SegundoNombre = segundoNombre;
            this.PrimerApellido = PrimerApellido;
            this.SegundoApellido = SegundoApellido;
            validarDatos();
        }
        public NombreCompleto() { }


        public void validarDatos()
        {
            try
            {
                if (this.PrimerNombre.Length < 2 || this.PrimerNombre == "")
                {
                    throw new Exception("Ingrese un nombre valido");
                }
                if (this.SegundoNombre != "" && this.SegundoNombre.Length < 2)
                {
                    throw new Exception("Ingrese un segundo nombre valido");
                }
                if (this.PrimerApellido.Length < 2 || this.PrimerApellido == "")
                {
                    throw new Exception("Ingrese un apellido valido");
                }
                if (this.SegundoNombre != "" && this.SegundoApellido.Length < 2)
                {
                    throw new Exception("Ingrese un segundo apellido valido");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}

