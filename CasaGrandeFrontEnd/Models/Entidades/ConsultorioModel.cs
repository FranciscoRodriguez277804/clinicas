namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class ConsultorioModel : IComparable<ConsultorioModel>
    {
        public int? NroConsultorio;

        public Area Area;
        public DateTime fecha;

        public Dictionary<string, List<ConsultasModel>> Horarios { get; set; } = new();

        public ConsultorioModel(int nroConusltorio, Area area)
        {
            this.NroConsultorio = nroConusltorio;
            this.Area = area;
        }
        public ConsultorioModel() { }


        public int CompareTo(ConsultorioModel? other)
        {
            if (other == null) return 1; // Si `other` es null, se considera menor que el objeto actual

            // Si ambos NroConsultorio tienen valor, comparar normalmente
            if (NroConsultorio.HasValue && other.NroConsultorio.HasValue)
            {
                return NroConsultorio.Value.CompareTo(other.NroConsultorio.Value);
            }
            // Si solo el objeto actual tiene valor, se considera mayor
            if (NroConsultorio.HasValue)
            {
                return 1;
            }
            // Si solo el objeto `other` tiene valor, el objeto actual se considera menor
            if (other.NroConsultorio.HasValue)
            {
                return -1;
            }
            // Ambos valores son null, se consideran iguales
            return 0;
        }
    }

}

