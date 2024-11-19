

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class Servicio
    {
        public string NombreServicio { get; set; }
        public List<Empleados> empleados { get; set; }

        public Servicio(List<Empleados> empleados, string NombreServicio)
        {
            this.empleados = empleados;
            this.NombreServicio = NombreServicio;
        }
        public Servicio() { }

    }

}

