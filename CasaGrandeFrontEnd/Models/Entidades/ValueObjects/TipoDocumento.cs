namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public record TipoDocumento
    {
        public string Id { get; set; }
        public string Nombre { get; set; }

        public TipoDocumento(string id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }
        public TipoDocumento() { }
    }
}
