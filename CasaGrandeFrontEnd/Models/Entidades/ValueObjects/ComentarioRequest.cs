namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class ComentarioRequest
    {

        public string id { get; set; }
        public string comentario { get; set; }

        public ComentarioRequest(string id, string comentario)
        {
            this.id = id;
            this.comentario = comentario;
        }
    }
}
