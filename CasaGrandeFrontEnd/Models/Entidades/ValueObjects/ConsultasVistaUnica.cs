namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class ConsultasVistaUnica
    {
        public IEnumerable<ConsultasModel> ConsultasActuales { get; set; }
        public IEnumerable<ConsultasModel> ConsultasProximas { get; set; }
    }
}
