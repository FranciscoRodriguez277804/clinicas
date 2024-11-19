using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinicaEspacioAbiertoFrontEnd.Middleware
{
    public class RequireLoginFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var path = context.HttpContext.Request.Path;

            // Si no hay sesión, redirige al usuario a la página de login
            if (session.GetString("Rol") == null &&
                !path.StartsWithSegments("/EmpleadoSistemas/Autorizar") &&
                !path.StartsWithSegments("/EmpleadoSistemas/Autorizar"))
            {
                context.Result = new RedirectToActionResult("Autorizar", "EmpleadoSistemas", null);
            }
        }
    }
}
