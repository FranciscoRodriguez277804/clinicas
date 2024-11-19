using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Usuarios
{
    public class UsuariosController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly string _url = "https://sistemaapidegestiondeclinicasv2-affxc4hdbvfjewce.canadacentral-01.azurewebsites.net/api/";
        private readonly JsonSerializerOptions _jsonOptions
            = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly string idCarpetaIMG = "19a513xXTOlhRCD-m0vqEBACCLZA7I3gP";

        public UsuariosController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_url);
        }




        public async Task<ActionResult> ListadoDeUsuarios(int page = 1, int pageSize = 20)
        {
            var usuariosJson = HttpContext.Session.GetString("Pacientes");

            if (string.IsNullOrEmpty(usuariosJson))
            {
                // Cargar los usuarios si no están en la sesión
                IEnumerable<Usuario> usuarios = await cargarPacientes();
                usuariosJson = JsonConvert.SerializeObject(usuarios);
                HttpContext.Session.SetString("Pacientes", usuariosJson);
            }

            var allUsuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(usuariosJson);

            // Calcular total de páginas
            int totalUsuarios = allUsuarios.Count();
            int totalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);

            // Aplicar la paginación
            var pagedUsuarios = allUsuarios.Skip((page - 1) * pageSize).Take(pageSize);

            // Crear el modelo de vista de paginación
            var model = new PaginacionViewModel<Usuario>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = pagedUsuarios
            };

            return View(model);
        }


        public async Task<ActionResult> ListadoDeUsuariosPartial(int page = 1, int pageSize = 20)
        {
            var usuariosJson = HttpContext.Session.GetString("Pacientes");

            if (string.IsNullOrEmpty(usuariosJson))
            {
                // Cargar los usuarios si no están en la sesión
                IEnumerable<Usuario> usuarios = await cargarPacientes();
                usuariosJson = JsonConvert.SerializeObject(usuarios);
                HttpContext.Session.SetString("Pacientes", usuariosJson);
            }

            var allUsuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(usuariosJson);

            // Calcular total de páginas
            int totalUsuarios = allUsuarios.Count();
            int totalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);

            // Aplicar la paginación
            var pagedUsuarios = allUsuarios.Skip((page - 1) * pageSize).Take(pageSize);

            // Crear el modelo de vista de paginación
            var model = new PaginacionViewModel<Usuario>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = pagedUsuarios
            };

            return PartialView("_ListadoUsuarios", model);
        }
        




        [HttpGet]
        public async Task<IActionResult> DescargarImagen(string fileId)


        {
            // Define la URL de tu API para descargar la imagen
            string apiUrl = $"{_url}ArchivosDrive/descargarImagenConNombre/{fileId}";

            using (var httpClient = new HttpClient())
            {
                // Realiza la solicitud GET a la API para descargar la imagen
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la imagen en bytes
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentDisposition = response.Content.Headers.ContentDisposition;

                    // Extraer el nombre del archivo del encabezado si existe
                    var fileName = contentDisposition != null ? contentDisposition.FileName.Trim('"').Trim() : "imagen_descargada.jpg";

                    // Devolver la imagen como un FileResult para que se muestre en el navegador
                    return File(imageBytes, "image/jpeg", fileName);
                }
                else
                {
                    // Si hay un error, puedes devolver una imagen por defecto o un error
                    return NotFound();
                }
            }
        }

        public IActionResult Detalles(string id)
        {
            var usuario = BuscarPaciente(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return PartialView("_DetallesUsuario", usuario); // Retorna una vista parcial con el modelo del usuario
        }

        public Usuario BuscarPaciente(string id)
        {
            Usuario user = null;
            try
            {
                var usuariosJson = HttpContext.Session.GetString("Pacientes");
                
                IEnumerable<Usuario> usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(usuariosJson);

                foreach (var usuario in usuarios)
                {
                    if (usuario.Id == id)
                    {
                        user = usuario;
                        return usuario;
                    }
                }
                if (user == null)
                {
                    throw new Exception("No se encontró el usuario");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return user;

        }


        public async Task<IEnumerable<Usuario>> cargarPacientes()
        {
            try
            {
                string url = $"{_url}Usuario/ObtenerUsuariosActivos";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pacientes = JsonConvert.DeserializeObject<List<Usuario>>(content);

                    if (pacientes == null)
                    {
                        throw new Exception("No se ha podido deserializar la lista de pacientes.");
                    }

                    var pacientesJson = JsonConvert.SerializeObject(pacientes);
                    HttpContext.Session.SetString("Pacientes", pacientesJson);
                    return pacientes;
                }
                else
                {
                    throw new Exception("Error en la solicitud");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        // GET: UsuariosController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            Usuario paciente = BuscarPaciente(id);

            // Espera el resultado de la tarea async
            IEnumerable<ConsultasModel> consultasDeUsuario = await BuscarConsultasPorIDPaciente(id);

            paciente.Sesiones = consultasDeUsuario;
            return View(paciente);
        }

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }


        public async Task<IActionResult> Buscar(string query, int page = 1, int pageSize = 20)
        {
            var usuariosJson = HttpContext.Session.GetString("Pacientes");
            IEnumerable<Usuario> usuarios = string.IsNullOrEmpty(usuariosJson)
                ? await cargarPacientes()
                : JsonConvert.DeserializeObject<IEnumerable<Usuario>>(usuariosJson);

            // Si el campo está vacío, simplemente muestra la primera página de usuarios
            if (string.IsNullOrWhiteSpace(query))
            {
                query = ""; // Asegurar que la consulta esté vacía
            }

            // Filtrar por el nombre o apellido, y aplicar paginación
            var resultados = usuarios.Where(u =>
                    u.NombreCompleto.PrimerNombre.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    u.NombreCompleto.PrimerApellido.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Crear modelo de vista con los resultados paginados
            var model = new PaginacionViewModel<Usuario>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)usuarios.Count() / pageSize),
                Items = resultados
            };

            return PartialView("_ListadoUsuarios", model);
        }


        // POST: UsuariosController/Create
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

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuariosController/Edit/5
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

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuariosController/Delete/5
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


        public async Task<IEnumerable<ConsultasModel>> BuscarConsultasPorPaciente(string nombreCompleto)
        {
            IEnumerable<ConsultasModel> consultasDePaciente = null;
            try
            {
                var url = $"{_url}Consultas/BuscarConsultaPorPaciente?nombreCompleto={Uri.EscapeDataString(nombreCompleto)}";
                var respuesta = await _httpClient.GetAsync(url);

                // Serializar el modelo de filtrado a JSON
                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var responseContent = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(responseContent);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                    }
                    consultasDePaciente = consultas;
                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return consultasDePaciente;
        }
        public async Task<IEnumerable<ConsultasModel>> BuscarConsultasPorIDPaciente(string id)
        {
            IEnumerable<ConsultasModel> consultasDePaciente = null;
            try
            {
                var url = $"{_url}Consultas/BuscarConsultaPorPacienteID/{id}";
                var respuesta = await _httpClient.GetAsync(url);

                // Serializar el modelo de filtrado a JSON
                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var responseContent = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(responseContent);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                    }
                    consultasDePaciente = consultas;
                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return consultasDePaciente;
        }
    }
}
