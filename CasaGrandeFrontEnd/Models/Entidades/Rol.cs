

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public record Rol
    {
        public string NombreRol;

        public Rol(string nombreRol)
        {
            this.NombreRol = nombreRol;
        }
        public Rol() { }
    }

}

