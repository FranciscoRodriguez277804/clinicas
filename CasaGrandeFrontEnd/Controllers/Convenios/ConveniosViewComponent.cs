using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Convenios
{
    public class ConveniosViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var conveniosJson = HttpContext.Session.GetString("ListaConvenios");
            var convenios = !string.IsNullOrEmpty(conveniosJson)
                ? JsonConvert.DeserializeObject<IEnumerable<Convenio>>(conveniosJson)
                : Enumerable.Empty<Convenio>();

            return View(convenios);
        }
    }

}

