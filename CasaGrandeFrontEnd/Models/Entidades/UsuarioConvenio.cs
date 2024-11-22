namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class UsuarioConvenio
    {
        public string id_usuario { get; set; }
        public string id_usuario_convenio { get; set; }
        public DateTime fecha_alta { get; set; }
        public string nombre_usuario { get; set; }
        public string convenio { get; set; }
        public string cedula { get; set; }
        public string presupuesto { get; set; }
        public string estado { get; set; }
        public DateTime fecha_baja { get; set; }
        public string solicitante { get; set; }
        public string motivo { get; set; }
        public string useremail { get; set; }
        public DateTime insertdate { get; set; }
        public string status { get; set; }

        public UsuarioConvenio(string id_usuario, string id_)
        {

        }
        public UsuarioConvenio() { }

        public UsuarioConvenio(string id_usuario, string id_usuario_convenio, DateTime fecha_alta, string nombre_usuario, string convenio, string cedula, string presupuesto, string estado, DateTime fecha_baja,
            string solicitante, string motivo, string useremail, DateTime insertdate, string status) : this(id_usuario, id_usuario_convenio)
        {
            this.fecha_alta = fecha_alta;
            this.nombre_usuario = nombre_usuario;
            this.convenio = convenio;
            this.cedula = cedula;
            this.presupuesto = presupuesto;
            this.estado = estado;
            this.fecha_baja = fecha_baja;
            this.solicitante = solicitante;
            this.motivo = motivo;
            this.useremail = useremail;
            this.insertdate = insertdate;
            this.status = status;
        }
    }
}
