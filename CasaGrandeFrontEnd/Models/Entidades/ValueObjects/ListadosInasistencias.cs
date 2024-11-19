namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class ListadosInasistencias
    {
        public IEnumerable<ConsultasModel> inasistenciasUsuarios { get; set; }
        public IEnumerable<ConsultasModel> inasistenciasMedicos { get; set; }

        public ListadosInasistencias() { }
        public ListadosInasistencias(IEnumerable<ConsultasModel> inasistenciasUsuarios, IEnumerable<ConsultasModel> inasistenciasMedicos)
        {
            this.inasistenciasUsuarios = inasistenciasUsuarios;
            this.inasistenciasMedicos = inasistenciasMedicos;
        }
    }
}
