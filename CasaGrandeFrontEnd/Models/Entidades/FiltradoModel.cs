namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class FiltradoModel
    {
        public DateTime? FechaConsulta { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string? NombreTecnico { get; set; }
        public string? IdUsuario { get; set; }
        public int? NumeroConsultorio { get; set; }
    }
}
