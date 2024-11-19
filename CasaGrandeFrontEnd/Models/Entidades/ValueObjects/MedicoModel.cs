namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects
{
    public class MedicoModel : IComparable<MedicoModel>
    {
        public string NombreMedico { get; set; }
        public Dictionary<string, List<ConsultasModel>> Horarios { get; set; } = new();

        // Implementación de IComparable<MedicoModel>
        public int CompareTo(MedicoModel other)
        {
            if (other == null)
                return 1;

            // Ordenar alfabéticamente por NombreMedico
            return string.Compare(NombreMedico, other.NombreMedico, StringComparison.OrdinalIgnoreCase);
        }
    }
}
