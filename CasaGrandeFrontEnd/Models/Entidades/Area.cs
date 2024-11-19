namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Area
    {
        public string NombreArea;

        public string CodigoArea;

        public List<Tecnico> ListaTecnicos;

        public List<ConsultorioModel> consultorios;

        public Area(string nombre, string codigo)
        {
            this.NombreArea = nombre;
            this.CodigoArea = codigo;
            this.ListaTecnicos = new List<Tecnico>();
            this.consultorios = new List<ConsultorioModel>();
        }
        public Area() { }
    }

}

