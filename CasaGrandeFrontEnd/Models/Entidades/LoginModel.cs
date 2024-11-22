using System.ComponentModel.DataAnnotations;

namespace ClinicaEspacioAbiertoFrontEnd.Models.Entidades
{
    public class LoginModel
    {
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }


    }
}
