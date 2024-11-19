using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Convenios
{
    public class ConvenioController : Controller
        {
            private readonly HttpClient _httpClient;
            private readonly string _url = "https://sistemaapidegestiondeclinicasv2-affxc4hdbvfjewce.canadacentral-01.azurewebsites.net/api/";
            private readonly string _url2 = "https://localhost:7231/api/";
            private readonly JsonSerializerOptions _jsonOptions
                = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };



            public ConvenioController(HttpClient httpClient)
            {
                _httpClient = httpClient;
                _httpClient.BaseAddress = new Uri(_url);
                _httpClient.Timeout = TimeSpan.FromSeconds(100); // Ajusta según tus necesidades

            }


            public ActionResult AgregarConvenio()
            {
                return View();
            }
            [HttpPost]
            public async Task<ActionResult> AgregarConvenio(Convenio convenio)
            {
                try
                {
                    if (convenio == null)
                    {
                        throw new Exception("El convenio no puede ser nulo");
                    }
                    string url = $"{_url}Convenio/AgregarConvenio";
                    var respuesta = _httpClient.PostAsJsonAsync(url, convenio);

                    if (respuesta.IsCompletedSuccessfully)
                    {
                        return PartialView("_convenioAgregado");
                    }
                    else
                    {
                        throw new Exception("No se pudo cargar el convenio");
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex;
                    return View();
                }


            }



            public async Task<IEnumerable<Convenio>> ObtenerListaDeConvenios()
            {
                try
                {
                    string url = $"{_url}Convenios/ObtenerConvenios";

                    // Realizar la solicitud GET a la API
                    var respuesta = await _httpClient.GetAsync(url);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        // Leer y deserializar el contenido de la respuesta
                        var content = await respuesta.Content.ReadAsStringAsync();
                        var convenios = JsonConvert.DeserializeObject<IEnumerable<Convenio>>(content);
                        return convenios;


                    }
                    return Enumerable.Empty<Convenio>();

                }
                catch (Exception ex)
                {
                    // Manejar excepciones y mostrar errores
                    ViewBag.Error = ex.Message;
                    return Enumerable.Empty<Convenio>();
                }
            }


        public async Task<ActionResult> DetallesConvenio(string nombreConvenio, int page = 1)
        {

            IEnumerable<Convenio> convenios = await ObtenerListaDeConvenios();
            int pageSize = 10;

            Convenio convenioEncontrado = null;

            foreach (var convenio in convenios)
            {
                if (convenio.nombre_convenio == nombreConvenio)
                {
                    convenioEncontrado = convenio;
                    break;
                }
            }

            if (convenioEncontrado == null)
            {
                // Handle the case where the convenio is not found
                return NotFound();
            }
            if (convenioEncontrado.usuarios.Count > 0)
            {
                var paginatedConvenios = convenioEncontrado.usuarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                var totalItems = convenioEncontrado.usuarios.Count;
                convenioEncontrado.usuarios = paginatedConvenios; // This modifies the original list
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                ViewBag.Titulo = nombreConvenio;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
            }
            

            return View("_DetallesConvenioPorNombre", convenioEncontrado); // Pass the single convenio
        }

            public async Task<ActionResult> DetallesConvenioActualizado(string nombreConvenio, int page = 1)
            {
                IEnumerable<Convenio> convenios = await ObtenerListaDeConvenios();
                int pageSize = 10;

                Convenio convenioEncontrado = null;

                foreach (var convenio in convenios)
                {
                    if (convenio.nombre_convenio == nombreConvenio)
                    {
                        convenioEncontrado = convenio;
                        break;
                    }
                }

                if (convenioEncontrado == null)
                {
                    // Handle the case where the convenio is not found
                    return NotFound();
                }

            if (convenioEncontrado.usuarios!=null)
            {
                var paginatedConvenios = convenioEncontrado.usuarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                var totalItems = convenioEncontrado.usuarios.Count;
                convenioEncontrado.usuarios = paginatedConvenios; // This modifies the original list
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                ViewBag.Titulo = nombreConvenio;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
            }

            return View("_DetallesConvenioPorNombre", convenioEncontrado); // Pass the single convenio
            }

            private IEnumerable<UsuarioConvenio> ObtenerListaDeUsuariosConvenios(string nombreConvenio)
            {
                var convenios = HttpContext.Session.GetString("UsuariosConvenios");
                IEnumerable<UsuarioConvenio> listaDeConvenios = JsonConvert.DeserializeObject<IEnumerable<UsuarioConvenio>>(convenios);
                var listaConveniosPorNombre = listaDeConvenios.Where(c => c.convenio == nombreConvenio).ToList();
                return listaConveniosPorNombre;
            }




    // GET: ConvenioController
    public ActionResult Index()
        {
            return View();
        }

        // GET: ConvenioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ConvenioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConvenioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConvenioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ConvenioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConvenioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConvenioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
