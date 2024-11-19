using ClinicaEspacioAbiertoFrontEnd.Middleware;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using Newtonsoft.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Models
{
    public class ConsultasService : IConsultasService
    {

        private readonly HttpClient _httpClient;


        public ConsultasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConsultasModel> ObtenerConsultaEditable(string consultasJson, string idConsulta)
        {
            if (string.IsNullOrEmpty(consultasJson))
            {
                throw new Exception("No hay consultas disponibles en la sesión.");
            }

            var consultas = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);
            return consultas?.FirstOrDefault(c => c.ID == idConsulta);
        }
    }
}
