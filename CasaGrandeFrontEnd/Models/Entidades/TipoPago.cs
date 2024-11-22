
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class TipoPago
    {
        public string NombreTipo { get; set; }
        public string Descripcion { get; set; }

        public TipoPago(string nombre)
        {
            NombreTipo = nombre;

        }
        public TipoPago() { }
    }

}

