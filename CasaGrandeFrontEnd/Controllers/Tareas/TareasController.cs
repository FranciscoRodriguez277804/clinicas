using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Tareas
{
    public class TareasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _url = "https://sistemaapidegestiondeclinicasv2-affxc4hdbvfjewce.canadacentral-01.azurewebsites.net/api/";
        private readonly JsonSerializerOptions _jsonOptions
            = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public TareasController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ActionResult> ListarTareasDelUsuario()
        {
            try
            {
                string idJson = HttpContext.Session.GetString("id");

                if (string.IsNullOrEmpty(idJson))
                {
                    ViewBag.Error = "No se pudo obtener el ID del usuario de la sesión.";
                    return PartialView("_tareasPartialView");
                }

                var EmpleadoJson = HttpContext.Session.GetString("Empleado");
                Empleados empleado = JsonConvert.DeserializeObject<Empleados>(EmpleadoJson);


                // Verificar si la respuesta fue exitosa
                if (empleado != null)
                {
                    IEnumerable<TareasModel> tareas = empleado.Tareas;
                    var tareasFiltradas = tareas.Where(t => (t.Estado == "Pendiente" || t.Estado == "Solicitada")
                && t.Fecha_Inicio <= DateTime.Today.Date).ToList();

                    if (tareasFiltradas == null || !tareasFiltradas.Any())
                    {
                        ViewBag.Error = "No hay tareas pendientes.";
                        return PartialView("_tareasPartialView");
                    }

                    return PartialView("_tareasPartialView", tareasFiltradas);
                }
                else
                {
                    ViewBag.Error = $"Error al obtener tareas:";
                    return PartialView("_tareasPartialView");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ocurrió un error al listar las tareas: {ex.Message}";
            }

            return PartialView("_tareasPartialView");
        }
        [HttpPost]
        public async Task<ActionResult> AgregarTarea(TareasModel tareas)
        {
            try
            {
                if (tareas.asignarTarea == "mi")
                {
                    tareas.ID_EmpleadoSolicitante = HttpContext.Session.GetString("id");
                    tareas.ID_EmpleadoSolicitado = HttpContext.Session.GetString("id");
                    tareas.Estado = "Solicitada";
                }
                else
                {
                    tareas.ID_EmpleadoSolicitante = HttpContext.Session.GetString("id");
                    tareas.ID_EmpleadoSolicitado = tareas.ID_EmpleadoSolicitado;
                    tareas.Estado = "Solicitada";
                }

                var tareaContent = new StringContent(JsonConvert.SerializeObject(tareas), Encoding.UTF8, "application/json");
                var url = $"{_url}Tareas/AgregarTarea";
                HttpResponseMessage respuesta = await _httpClient.PostAsync(url, tareaContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Obtiene el ID de la tarea creada
                    var contenido = await respuesta.Content.ReadAsStringAsync();

                    // Serializa el objeto empleado y actualiza la sesión
                    var EmpleadoJson = HttpContext.Session.GetString("Empleado");
                    Empleados empleado = JsonConvert.DeserializeObject<Empleados>(EmpleadoJson);
                    tareas.ID_Tarea = contenido; // Asigna el ID a la tarea
                    empleado.Tareas.Add(tareas);
                    EmpleadoJson = JsonConvert.SerializeObject(empleado);
                    HttpContext.Session.SetString("Empleado", EmpleadoJson);

                    // Devuelve la vista parcial directamente
                    return PartialView("_tareaConfirmada", tareas);
                }
                else
                {
                    ViewBag.Error = "No se pudo cargar la tarea";
                    return PartialView("_tareaConfirmada");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo cargar la tarea";
                return PartialView("_tareaConfirmada");
            }
        }


        public JsonResult BuscarEmpleados(string query)
        {
            var jsonTecnico = HttpContext.Session.GetString("Empleados");
            IEnumerable<Empleados> empleados = JsonConvert.DeserializeObject<List<Empleados>>(jsonTecnico);

            var coincidencias = empleados.Where(u =>
                $"{u.NombreCompleto.PrimerNombre} {u.NombreCompleto.PrimerApellido}".Contains(query, StringComparison.OrdinalIgnoreCase)
            ).Select(u => new
            {
                PrimerNombre = u.NombreCompleto.PrimerNombre,
                PrimerApellido = u.NombreCompleto.PrimerApellido,
                id = u.IdEmpleado,
            }).ToList();

            return Json(coincidencias);
        }


        public async Task<ActionResult> TableroTareas()
        {
            IEnumerable<TareasModel> listadoTareas = await ObtenerTareas();
            return View(listadoTareas);
        }


        [HttpPost]
        public async Task<ActionResult> ActualizarEstadoTarea([FromBody] ActualizarEstadoModel modelo)
        {
            // Construir la URL de la API donde se realizará la actualización del estado
            string url = $"{_url}Tareas/ActualizarEstado";

            try
            {
                using (var client = new HttpClient())
                {
                    // Convertir el objeto en JSON
                    var jsonData = JsonConvert.SerializeObject(modelo);

                    // Crear el contenido de la solicitud (tipo JSON)
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Hacer la solicitud PUT a la API
                    var response = await client.PutAsync(url, content);

                    // Leer el contenido de la respuesta como texto
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Devolver una respuesta JSON indicando éxito
                        var empleadoJson = HttpContext.Session.GetString("Empleado");
                        var empleado = JsonConvert.DeserializeObject<Empleados>(empleadoJson);
                        var tarea = empleado.Tareas.FirstOrDefault(t => t.ID_Tarea == modelo.id_Tarea);
                        if (tarea != null)
                        {
                            tarea.Estado = modelo.estado; // Cambia el estado de la tarea
                        }
                        empleado.Tareas = empleado.Tareas; // Asegúrate de que la lista de tareas se mantenga actualizada
                        HttpContext.Session.SetString("Empleado", JsonConvert.SerializeObject(empleado));
                        return Json(new { success = true });
                    }
                    else
                    {
                        // Devolver una respuesta JSON indicando error
                        return Json(new { success = false, message = responseContent });
                    }
                }
            }
            catch (Exception ex)
            {
                // Devolver una respuesta JSON indicando excepción
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IEnumerable<TareasModel>> ObtenerTareas()
        {
            try
            {
                // Construir la URL para la solicitud GET
                string url = $"{_url}Tareas/ObtenerTareas";

                // Realizar la solicitud GET a la API
                var respuesta = await _httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var tareas = JsonConvert.DeserializeObject<IEnumerable<TareasModel>>(content);
                    return tareas;
                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
                return null;
            }


        }

        [HttpPost]
        public async Task<ActionResult> FinalizarTarea(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Id no válido");
                }

                string url = $"{_url}Tareas/FinalizarTarea?id={id}";
                var content = new StringContent(id, Encoding.UTF8, "text/plain");

                // Hacer la solicitud PUT a la API
                var response = await _httpClient.PutAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    // Devolver una respuesta JSON indicando éxito
                    var empleadoJson = HttpContext.Session.GetString("Empleado");
                    var empleado = JsonConvert.DeserializeObject<Empleados>(empleadoJson);
                    var tarea = empleado.Tareas.FirstOrDefault(t => t.ID_Tarea == id);
                    if (tarea != null)
                    {
                        tarea.Estado = "Finalizada"; // Cambia el estado de la tarea
                    }
                    empleado.Tareas = empleado.Tareas; // Asegúrate de que la lista de tareas se mantenga actualizada
                    HttpContext.Session.SetString("Empleado", JsonConvert.SerializeObject(empleado));
                    return new JsonResult(new { success = true }) { ContentType = "application/json" };

                }
                return new JsonResult(new { success = true }) { ContentType = "application/json" };

            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false }) { ContentType = "application/json" };
            }


        }
        [HttpPost]
        public async Task<ActionResult> EliminarTarea(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Id no válido" });
                }

                string url = $"{_url}Tareas/EliminarTarea?id={id}";

                // Hacer la solicitud PUT a la API
                var response = await _httpClient.PutAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    // Actualiza la sesión con la tarea eliminada
                    var empleadoJson = HttpContext.Session.GetString("Empleado");
                    var empleado = JsonConvert.DeserializeObject<Empleados>(empleadoJson);
                    var tarea = empleado.Tareas.FirstOrDefault(t => t.ID_Tarea == id);
                    if (tarea != null)
                    {
                        tarea.Estado = "Eliminada";
                    }
                    HttpContext.Session.SetString("Empleado", JsonConvert.SerializeObject(empleado));
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "No se pudo eliminar la tarea en la API" });
            }
            catch (Exception ex)
            {
                // Log de la excepción para revisión
                Console.WriteLine("Error al eliminar la tarea:", ex);
                return Json(new { success = false, message = "Ocurrió un error al eliminar la tarea." });
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: TareasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TareasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TareasController/Create
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

        // GET: TareasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TareasController/Edit/5
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

        // GET: TareasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TareasController/Delete/5
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
