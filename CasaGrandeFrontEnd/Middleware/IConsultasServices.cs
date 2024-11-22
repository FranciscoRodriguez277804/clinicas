using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;

namespace ClinicaEspacioAbiertoFrontEnd.Middleware
{
    public interface IConsultasService
    {
        Task<ConsultasModel> ObtenerConsultaEditable(string consultasJson, string idConsulta);
    }
}
