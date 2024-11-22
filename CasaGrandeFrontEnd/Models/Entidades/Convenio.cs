using Newtonsoft.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Convenio
    {
        public string id_convenio { get; set; }
        public string nombre_convenio { get; set; }
        public string area { get; set; }
        public string estado { get; set; }
        public DateTime insertdate { get; set; }
        public string useremail { get; set; }

        [JsonProperty("usuariosConvenio")]
        public List<UsuarioConvenio> usuarios { get; set; }

        public Convenio()
        {
            this.usuarios = new List<UsuarioConvenio>();
        }

        public Convenio(string id_convenio, string nombre_convenio, string area, string estado, DateTime insertdate, string useremail, List<UsuarioConvenio> usuarios)
        {
            this.id_convenio = id_convenio;
            this.nombre_convenio = nombre_convenio;
            this.area = area;
            this.estado = estado;
            this.insertdate = insertdate;
            this.useremail = useremail;
            this.usuarios = usuarios;
        }
    }
}
