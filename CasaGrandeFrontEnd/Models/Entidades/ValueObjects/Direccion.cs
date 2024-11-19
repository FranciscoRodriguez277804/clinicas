
namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public record Direccion
    {
        public string Ciudad { get; set; }

        public string Calle { get; set; }

        public int Numero { get; set; }

        public Direccion(string ciudad, string calle, int numero)
        {
            this.Ciudad = ciudad;
            this.Calle = calle;
            this.Numero = numero;
        }

    }

}

