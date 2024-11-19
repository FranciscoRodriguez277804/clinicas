namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Representante
    {
        public string NombreCompleto;

        public string CIRepresentante;

        public Representante(string nombre, string CIRepresentante)
        {
            this.CIRepresentante = CIRepresentante;
            this.NombreCompleto = nombre;
        }
        public Representante() { }
    }

}

