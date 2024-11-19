namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class TareasModel
    {
        public string ID_Tarea { get; set; }
        public string ID_EmpleadoSolicitante { get; set; }
        public string ID_EmpleadoSolicitado { get; set; }
        public string asignarTarea { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public string TextoDesarrollo { get; set; }

        public TareasModel() { }
    }
}
