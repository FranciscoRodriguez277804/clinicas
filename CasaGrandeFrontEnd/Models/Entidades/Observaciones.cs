namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Observaciones
    {
        public DateTime FechaObservacion;

        public string TituloObservacion;

        public string TextoObservacion;

        public Empleados Medico;

        public UsuarioConvenio Usuario;

        public Observaciones(DateTime FechaObservacion, string Titulo, string Texto, Tecnico medico, UsuarioConvenio usuario)
        {
            this.TextoObservacion = Texto;
            this.FechaObservacion = FechaObservacion;
            this.TituloObservacion = Titulo;
            this.Medico = medico;
            this.Usuario = usuario;
            validarDatos();
        }
        public Observaciones() { }
        public void validarDatos()
        {
            try
            {
                if (this.FechaObservacion.Date > DateTime.Today.Date)
                {
                    throw new Exception("La fecha no puede ser mayor a hoy");
                }
                if (this.Usuario == null)
                {
                    throw new Exception("Ingrese un usuario valido");
                }
                if (this.Medico == null)
                {
                    throw new Exception("Ingrese un medico valido");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}

