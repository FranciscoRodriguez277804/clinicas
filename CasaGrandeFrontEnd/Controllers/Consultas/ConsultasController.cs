
using ClinicaEspacioAbiertoFrontEnd.Middleware;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;


namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Consultas
{
    public class ConsultasController : Controller
    {
        private readonly IConsultasService _consultasService;
        private readonly HttpClient _httpClient;


        private readonly string _url = "https://sistemaapidegestiondeclinicasv2 affxc4hdbvfjewce.canadacentral 01.azurewebsites.net/api/";


        private readonly JsonSerializerOptions _jsonOptions
            = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ConsultasController(HttpClient httpClient, IConsultasService consultas)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_url);
            _consultasService = consultas;
        }

        // GET: EmpleadoSistemasController
        public ActionResult Index()
        {
            return View();
        }


        //AGREGAR CONSULTAS ENTRE DOS FECHAS
        public ActionResult AgregarConsulta()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AgregarConsulta(AgregarConsultasModel nuevaConsulta)
        {
            try
            {
                if (nuevaConsulta == null)
                {
                    throw new Exception("La consulta no es válida.");
                }

                nuevaConsulta.validarDatos();

                // Cambia a PostAsJsonAsync para enviar los datos en el cuerpo de la solicitud POST
                string url = $"{_url}Consultas/agregar";
                var respuesta = await _httpClient.PostAsJsonAsync(url, nuevaConsulta);

                if (respuesta.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Consulta agregada correctamente";
                    ActualizarConsultas();
                    return View();
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"Ocurrió un error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


        [HttpGet]
        public IActionResult AgregarUnaNuevaConsulta()
        {
            return PartialView("_AgregarNuevaConsulta");
        }

        [HttpGet]
        //AGREGAR CONSULTA PARA UNA FECHA
        public IActionResult AgregarConsultaPartial(DateTime fecha, string hora, string tecnico)
        {

            string sucursal = HttpContext.Session.GetString("Sucursal");
            // Establecer la cultura a español
            CultureInfo culturaEspañol = new CultureInfo("es ES");

            // Obtener el día de la semana
            string diaDeLaSemana = fecha.ToString("dddd", culturaEspañol);
            diaDeLaSemana = char.ToUpper(diaDeLaSemana[0]) + diaDeLaSemana.Substring(1);


            TimeSpan horaInicio;
            if (TimeSpan.TryParse(hora, out horaInicio))
            {
                // Sumar 45 minutos a la hora de inicio
                TimeSpan horaFin = horaInicio.Add(new TimeSpan(0, 45, 0));

                // Convertir la hora fin de nuevo a un string si es necesario (formato "hh:mm")
                string horaFinString = horaFin.ToString(@"hh\:mm");

                var model = new ConsultasModel
                {
                    Fecha_Consulta = fecha,
                    Hora_inicio = hora,      // Mantener la hora de inicio como string
                    Hora_fin = horaFinString, // Asignar la hora fin calculada
                    Dia = diaDeLaSemana,
                    Nombre_empleado = tecnico,
                    Sucursal = sucursal

                };

                // Aquí puedes continuar con el resto del código para procesar el modelo
                return PartialView("PartialViewAgregarUnaConsulta", model); // O la vista que corresponda
            }
            else
            {
                // Manejo de error si la hora no es válida
                Console.WriteLine("Hora de inicio no válida.");
                return BadRequest("La hora de inicio no es válida.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AgregarConsultaPartial(ConsultasModel nuevaConsulta)
        {
            try
            {
                if (nuevaConsulta == null)
                {
                    throw new Exception("La consulta no es válida.");
                }

                if (string.IsNullOrWhiteSpace(nuevaConsulta.NombreUsuario))
                {
                    throw new Exception("El nombre de usuario no puede estar vacío.");
                }
                DateTime horaInicio = ParseHora(nuevaConsulta.Hora_inicio);
                DateTime horaFin = ParseHora(nuevaConsulta.Hora_fin);
                ConsultasModel ConsultaCompleta = new ConsultasModel
                {
                    NombreUsuario = nuevaConsulta.NombreUsuario,
                    Nombre_empleado = nuevaConsulta.Nombre_empleado,
                    Fecha_Consulta = nuevaConsulta.Fecha_Consulta,
                    Hora_inicio = nuevaConsulta.Hora_inicio,
                    Hora_fin = nuevaConsulta.Hora_fin,
                    n_consultorio = nuevaConsulta.n_consultorio,
                    Convenio = nuevaConsulta.Convenio,
                    Sucursal = nuevaConsulta.Sucursal,
                    UserMail = HttpContext.Session.GetString("Email"),
                    Servicio = nuevaConsulta.Servicio,
                    Dia = nuevaConsulta.Dia,
                    Estado = "Activo",
                    Pago = "Pendiente de pago",
                    ID_Usuario = nuevaConsulta.ID_Usuario

                };
                var json = JsonConvert.SerializeObject(ConsultaCompleta, Formatting.Indented);
                Console.WriteLine(json);

                string url = $"{_url}Consultas/AgregarUnaConsulta";
                var respuesta = await _httpClient.PostAsJsonAsync(url, ConsultaCompleta);

                if (respuesta.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Consulta agregada correctamente";
                    await ActualizarConsultas(); // Verifica que este método maneje correctamente la asincronía
                    return RedirectToAction("ListarConsultas");
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"Ocurrió un error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> AgregarNuevaConsultaPartial(ConsultasModel nuevaConsulta)
        {
            try
            {
                if (nuevaConsulta == null)
                {
                    throw new Exception("La consulta no es válida.");
                }

                nuevaConsulta.validarDatos();

                // Cambia a PostAsJsonAsync para enviar los datos en el cuerpo de la solicitud POST
                string url = $"{_url}Consultas/agregar";
                var respuesta = await _httpClient.PostAsJsonAsync(url, nuevaConsulta);

                if (respuesta.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Consulta agregada correctamente";
                    ActualizarConsultas();
                    return View("ListarConsultas");
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"Ocurrió un error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("ListarConsultas");
            }
        }
        public DateTime ParseHora(string horaString)
        {
            // Usamos el formato "HH:mm:ss" para que siempre tenga horas, minutos y segundos
            if (TimeSpan.TryParse(horaString, out TimeSpan tiempo))
            {
                // Si el parseo es exitoso, lo convertimos a DateTime con una fecha base
                return DateTime.Today.Add(tiempo);
            }
            else
            {
                // Manejo de errores
                throw new FormatException("Formato de hora no válido.");
            }
        }

        public async Task<IEnumerable<ConsultasModel>> ObtenerConsultasAsync()
        {
            try
            {
                string url = $"Consultas/lista?fecha={DateTime.Today:yyyy MM dd}";

                // Realizar la solicitud GET a la API
                var respuesta = await _httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(content);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                        return Enumerable.Empty<ConsultasModel>();
                    }

                    return consultas;
                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                    return Enumerable.Empty<ConsultasModel>();
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
                return Enumerable.Empty<ConsultasModel>();
            }

        }
        //FIN DE AGREGADOS

        //                             LISTADOS                           

        public async Task<ActionResult> ListarProximas5Consultas()
        {
            // Obtener las consultas desde la API
            var consultasList = (await ObtenerConsultasAsync2(DateTime.Now.Date)).ToList();

            if (consultasList == null || !consultasList.Any())
            {
                ViewBag.Error = "No se han podido obtener las consultas.";
                return View();
            }
            HttpContext.Session.SetInt32("CantidadSesiones", consultasList.Count());
            // Obtener la hora actual
            var horaActual = DateTime.Now.TimeOfDay;

            // Filtrar consultas que son después de la hora actual
            var proximasConsultas = consultasList
                .Where(c => TimeSpan.TryParse(c.Hora_inicio, out var horaInicio) && horaInicio >= horaActual)
                .OrderBy(c => TimeSpan.Parse(c.Hora_inicio)) // Ordenar por hora de inicio
                .Take(5) // Tomar las primeras 5
                .ToList();

            return PartialView("_cargar5Consultas", proximasConsultas);
        }

        public async Task<ActionResult> ListarProximas5ConsultasParaTecnico()
        {
            var consultasList = (await ObtenerConsultasAsync2(DateTime.Now.Date)).ToList();
            var Tecnico = HttpContext.Session.GetString("Nombre") + " " + HttpContext.Session.GetString("Apellido");


            if (consultasList == null || !consultasList.Any())
            {
                ViewBag.Error = "No se han podido obtener las consultas.";
                return View();
            }
            HttpContext.Session.SetInt32("CantidadSesiones", consultasList.Count());
            // Obtener la hora actual
            var horaActual = DateTime.Now.TimeOfDay;

            // Filtrar consultas que son después de la hora actual
            var proximasConsultas = consultasList
                .Where(c => TimeSpan.TryParse(c.Hora_inicio, out var horaInicio) && horaInicio >= horaActual && c.Nombre_empleado == Tecnico)
                .OrderBy(c => TimeSpan.Parse(c.Hora_inicio)) // Ordenar por hora de inicio
                .Take(5) // Tomar las primeras 5
                .ToList();

            return PartialView("_cargar5ConsultasParaTecnico", proximasConsultas);
        }
        public async Task<ActionResult> ListarConsultasPorDiaParaTecnicoLog()
        {
            try
            {
                // Si no se proporciona una fecha, utilizamos la fecha actual
                var fechaConsultas = DateTime.Now;
                List<string> nombreSucursal = new List<string>();
                CultureInfo culturaEspañol = new CultureInfo("es ES");


                string diaDeLaSemana = fechaConsultas.ToString("dddd", culturaEspañol);
                diaDeLaSemana = char.ToUpper(diaDeLaSemana[0]) + diaDeLaSemana.Substring(1);

                string consultasJson = HttpContext.Session.GetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}");
                var Tecnico = HttpContext.Session.GetString("Nombre") + " " + HttpContext.Session.GetString("Apellido");

                List<ConsultasModel> consultasDelDia;

                // Obtener las consultas desde la API para la fecha específica
                var consultasList = (await ObtenerConsultasAsync2(fechaConsultas)).ToList();

                if (consultasList == null)
                {

                    return PartialView("_vistaTablaxDia", new List<ConsultorioModel>());
                }

                // Filtrar las consultas por la sucursal
                consultasDelDia = consultasList
                    .Where(c => c.Nombre_empleado == Tecnico)
                    .OrderBy(c => c.Hora_inicio).ToList();

                return PartialView("_VistaPorDiaTecnicoView", consultasDelDia);
            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                return PartialView("_VistaPorDiaTecnicoView", new List<ConsultorioModel>());
            }
        }
        public async Task<IActionResult> ConsultasActualesYProximas()
        {
            ConsultasVistaUnica modeloConsultas = await ObtenerProximasConsultas();
            return View(modeloConsultas);
        }

        public async Task<ConsultasVistaUnica> ObtenerProximasConsultas()
        {
            IEnumerable<ConsultasModel> consultasDelDia = await ObtenerConsultasAsync2(DateTime.Today.Date);
            TimeSpan horaActual = DateTime.UtcNow.TimeOfDay;

            // Consultas en ejecución
            IEnumerable<ConsultasModel> consultasActuales = consultasDelDia
                .Where(c => TimeSpan.Parse(c.Hora_inicio) <= horaActual && TimeSpan.Parse(c.Hora_fin) >= horaActual)
                .ToList();

            // Consultas próximas (las primeras 5 que inician después de la hora actual)
            IEnumerable<ConsultasModel> consultasProximas = consultasDelDia
                .Where(c => TimeSpan.Parse(c.Hora_inicio) > horaActual)
                .OrderBy(c => TimeSpan.Parse(c.Hora_inicio))
                .Take(5)
                .ToList();

            // Crear el modelo para pasar a la vista
            return new ConsultasVistaUnica
            {
                ConsultasActuales = consultasActuales,
                ConsultasProximas = consultasProximas
            };
        }


        public IActionResult CantidadDeConsultas()
        {
            var cantidadConsultas = HttpContext.Session.GetInt32("CantidadSesiones");
            ViewBag.ConsultasDelDia = cantidadConsultas;
            return PartialView("_cantidadConsultasPorDia");
        }

        public async Task<ActionResult> ListarConsultas()
        {
            try
            {
                // Obtener la cadena JSON desde la sesión
                string consultasJson = HttpContext.Session.GetString("ConsultasDelDia");

                List<ConsultasModel> consultasDelDia;
                if (string.IsNullOrEmpty(consultasJson))
                {
                    // Obtener las consultas desde la API si no están en la sesión
                    var consultasList = (await ObtenerConsultasAsync2(DateTime.Now.Date)).ToList();

                    if (consultasList == null)
                    {
                        ViewBag.Error = "No se han podido obtener las consultas.";
                        return View();
                    }

                    // Convertir la lista a JSON y guardarla en la sesión
                    consultasJson = JsonConvert.SerializeObject(consultasList);
                    HttpContext.Session.SetString("ConsultasDelDia", consultasJson);

                    consultasDelDia = consultasList;
                }
                else
                {
                    // Deserializar la cadena JSON a una lista de objetos ConsultasModel
                    consultasDelDia = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);
                }

                var sucursal = HttpContext.Session.GetString("Sucursal");

                // Filtrar las consultas por la sucursal
                var consultasFiltradas = consultasDelDia
                    .Where(c => c.Sucursal == sucursal)
                    .ToList();

                // Agrupar las consultas por médico y luego por horario
                var horarios = Enumerable.Range(8, 13)
                    .Select(h => $"{h:D2}:00:00")
                    .ToList();

                var groupedConsultas = consultasFiltradas
                    .GroupBy(c => c.Nombre_empleado)
                    .Select(g => new MedicoModel
                    {
                        NombreMedico = g.Key,
                        Horarios = horarios.ToDictionary(
                            horario => horario,
                            horario => g.Where(c =>
                            {
                                var consultaHora = DateTime.Parse(c.Hora_inicio);
                                var intervaloHora = DateTime.Parse(horario);
                                // Agrupar si la consulta está dentro del intervalo de una hora
                                return consultaHora >= intervaloHora && consultaHora < intervaloHora.AddHours(1);
                            }).ToList()
                        )
                    })
                    .ToList();

                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                return View("ListarConsultas", groupedConsultas); // Cambiado para cargar la vista por médico
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<ActionResult> ListarConsultasPorDia(DateTime? fecha)
        {
            try
            {
                // Si no se proporciona una fecha, utilizamos la fecha actual
                var fechaConsultas = fecha ?? DateTime.Now;
                List<string> nombreSucursal = new List<string>();
                // Obtener la cadena JSON desde la sesión, pero ahora incluirá la fecha
                string consultasJson = HttpContext.Session.GetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}");

                List<ConsultasModel> consultasDelDia;

                // Obtener las consultas desde la API para la fecha específica
                var consultasList = (await ObtenerConsultasAsync2(fechaConsultas)).ToList();

                if (consultasList == null || consultasList.Count == 0)
                {
                    return PartialView("_vistaConsultasPorMedico", new List<ConsultorioModel>());
                }

                // Convertir la lista a JSON y guardarla en la sesión con la fecha
                consultasJson = JsonConvert.SerializeObject(consultasList);
                HttpContext.Session.SetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}", consultasJson);

                consultasDelDia = consultasList;

                var sucursal = HttpContext.Session.GetString("Sucursal");

                // Filtrar las consultas por la sucursal
                var consultasFiltradas = consultasDelDia
                    .Where(c => c.Sucursal == sucursal)
                    .ToList();

                // Agrupar las consultas por médico y luego por horario
                var horarios = Enumerable.Range(8, 13)
                    .Select(h => $"{h:D2}:00:00")
                    .ToList();

                var groupedConsultas = consultasFiltradas
                    .GroupBy(c => c.Nombre_empleado)
                    .Select(g => new MedicoModel
                    {
                        NombreMedico = g.Key,
                        Horarios = horarios.ToDictionary(
                            horario => horario,
                            horario => g.Where(c =>
                            {
                                var consultaHora = DateTime.Parse(c.Hora_inicio);
                                var intervaloHora = DateTime.Parse(horario);
                                // Agrupar si la consulta está dentro del intervalo de una hora
                                return consultaHora >= intervaloHora && consultaHora < intervaloHora.AddHours(1);
                            }).ToList()
                        )
                    })
                    .ToList();


                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                // Imprimir los datos agrupados en la consola (opcional)
                Console.WriteLine("Datos agrupados por consultorio:");


                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                return PartialView("_vistaConsultasPorMedico", groupedConsultas);
            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                return PartialView("_vistaConsultasPorMedico", new List<MedicoModel>());
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListarConsultasPendientes()
        {
            try
            {
                // Obtener la cadena JSON desde la sesión
                string consultasJson = HttpContext.Session.GetString("ConsultasPendientes");

                List<ConsultasModel> consultasPendientes;
                if (string.IsNullOrEmpty(consultasJson))
                {
                    // Obtener las consultas desde la API si no están en la sesión
                    await ObtenerConsultasPendientes();
                    consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
                }

                // Deserializar la cadena JSON a una lista de objetos ConsultasModel
                consultasPendientes = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);

                if (consultasPendientes == null)
                {
                    // Manejo del caso donde no hay consultas cargadas
                    return PartialView("_vistaConsultasPendientes", new List<Usuario>());
                }

                var usuariosConConsultasPendientes = new List<Usuario>();

                // Agrupar consultas por el nombre completo del usuario (primer nombre y primer apellido)
                var consultasPorUsuario = consultasPendientes
                    .Where(c => c.Pago != "Pagado") // Filtrar solo consultas no pagadas
                    .GroupBy(c => new { c.NombreUsuario }) // Agrupar por nombre y apellido
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Recuperar la lista de usuarios desde el caché
                var usuariosEnCache = HttpContext.Session.GetString("Usuarios");
                if (usuariosEnCache == null)
                {
                    await cargarUsuarios();
                    usuariosEnCache = HttpContext.Session.GetString("Usuarios");
                }
                IEnumerable<Usuario> usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(usuariosEnCache);

                // Asignar las consultas agrupadas a los usuarios correspondientes
                foreach (var usuario in usuarios)
                {
                    var key = new { NombreUsuario = usuario.NombreCompleto.PrimerNombre + " " + usuario.NombreCompleto.PrimerApellido };

                    if (consultasPorUsuario.TryGetValue(key, out var consultas))
                    {
                        if (consultas.Any()) // Ya filtramos las no pagadas arriba
                        {
                            usuario.Sesiones = consultas; // Asignar las consultas no pagadas al usuario
                            usuariosConConsultasPendientes.Add(usuario); // Añadir a la lista de usuarios con consultas pendientes
                        }
                    }
                }

                // Devolver la vista parcial con la lista de usuarios y sus consultas no pagadas
                return PartialView("_vistaConsultasPendientes", usuariosConConsultasPendientes);
            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                return PartialView("_vistaConsultasPendientes", new List<Usuario>());
            }
        }


        [HttpPost]
        private async Task ObtenerConsultasPendientes()
        {
            try
            {
                // Construir la URL para la solicitud GET
                string url = $"Consultas/BuscarConsultasPendiente";

                // Realizar la solicitud GET a la API
                var respuesta = await _httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(content);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                    }
                    var consultasJson = JsonConvert.SerializeObject(consultas);
                    HttpContext.Session.SetString("ConsultasPendientes", consultasJson);
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
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
            }

        }
        //                  OBTENER CONSULTAS POR MEDICO                       
        public async Task<ActionResult> ListarConsultasPorMedico()
        {
            try
            {
                // Obtener la cadena JSON desde la sesión
                string consultasJson = HttpContext.Session.GetString("ConsultasDelDia");

                List<ConsultasModel> consultasDelDia;
                if (string.IsNullOrEmpty(consultasJson))
                {
                    // Obtener las consultas desde la API si no están en la sesión
                    var consultasList = (await ObtenerConsultasAsync()).ToList();

                    if (consultasList == null)
                    {
                        return PartialView("_vistaConsultasPorMedico", new List<MedicoModel>());
                    }

                    // Convertir la lista a JSON y guardarla en la sesión
                    consultasJson = JsonConvert.SerializeObject(consultasList);
                    HttpContext.Session.SetString("ConsultasDelDia", consultasJson);

                    consultasDelDia = consultasList;
                }
                else
                {
                    // Deserializar la cadena JSON a una lista de objetos ConsultasModel
                    consultasDelDia = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);
                }

                // Agrupar las consultas por médico y luego por horario
                var horarios = Enumerable.Range(8, 13)
                    .Select(h => $"{h:D2}:00:00")
                    .ToList();

                var groupedConsultas = consultasDelDia
                    .GroupBy(c => c.Nombre_empleado)
                    .Select(g => new MedicoModel
                    {
                        NombreMedico = g.Key,
                        Horarios = horarios.ToDictionary(
                            horario => horario,
                            horario => g.Where(c =>
                            {
                                var consultaHora = DateTime.Parse(c.Hora_inicio);
                                var intervaloHora = DateTime.Parse(horario);
                                // Agrupar si la consulta está dentro del intervalo de una hora
                                return consultaHora >= intervaloHora && consultaHora < intervaloHora.AddHours(1);
                            }).ToList()
                        )
                    })
                    .ToList();

                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                return PartialView("_vistaConsultasPorMedico", groupedConsultas);
            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                return PartialView("_vistaConsultasPorMedico", new List<MedicoModel>());
            }
        }

        [HttpGet]
        public async Task<ActionResult> ListarConsultasMedicosPorDia(DateTime? fecha)
        {
            try
            {
                // Si no se proporciona una fecha, utilizamos la fecha actual
                var fechaConsultas = fecha ?? DateTime.Now;
                List<string> nombreSucursal = new List<string>();
                // Obtener la cadena JSON desde la sesión, pero ahora incluirá la fecha
                string consultasJson = HttpContext.Session.GetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}");

                List<ConsultasModel> consultasDelDia;

                // Obtener las consultas desde la API para la fecha específica


                if (string.IsNullOrEmpty(consultasJson))
                {

                    var consultasList = (await ObtenerConsultasAsync2(fechaConsultas)).ToList();

                    if (consultasList == null || consultasList.Count == 0)
                    {
                        return PartialView("_vistaTablaxDia", new List<ConsultorioModel>());
                    }

                    // Convertir la lista a JSON y guardarla en la sesión
                    consultasJson = JsonConvert.SerializeObject(consultasList);
                    HttpContext.Session.SetString("ConsultasDelDia", consultasJson);

                    consultasDelDia = consultasList;
                }
                else
                {
                    // Deserializar la cadena JSON a una lista de objetos ConsultasModel
                    consultasDelDia = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);
                }

                // Agrupar las consultas por médico y luego por horario
                var horarios = Enumerable.Range(8, 13)
                    .Select(h => $"{h:D2}:00:00")
                    .ToList();

                var groupedConsultas = consultasDelDia
                    .GroupBy(c => c.Nombre_empleado)
                    .Select(g => new MedicoModel
                    {
                        NombreMedico = g.Key,
                        Horarios = horarios.ToDictionary(
                            horario => horario,
                            horario => g.Where(c =>
                            {
                                var consultaHora = DateTime.Parse(c.Hora_inicio);
                                var intervaloHora = DateTime.Parse(horario);
                                // Agrupar si la consulta está dentro del intervalo de una hora
                                return consultaHora >= intervaloHora && consultaHora < intervaloHora.AddHours(1);
                            }).ToList()
                        )
                    })
                    .ToList();

                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                return PartialView("_vistaConsultasPorMedico", groupedConsultas);
            }
            catch (Exception ex)
            {
                // Manejo de errores (opcional)
                return PartialView("_vistaConsultasPorMedico", new List<MedicoModel>());
            }
        }
        //                    OBTENER CONSULTAS POR FECHA                 
        public async Task<IEnumerable<ConsultasModel>> ObtenerConsultasAsync2(DateTime? fecha)
        {
            try
            {
                // Verificar si la fecha tiene valor; de lo contrario, usa la fecha actual
                var fechaConsulta = fecha ?? DateTime.Now;

                // Construir la URL para la solicitud GET, incluyendo la fecha como parámetro de consulta
                string url = $"Consultas/lista?fecha={fechaConsulta:yyyy MM dd}";

                // Realizar la solicitud GET a la API
                var respuesta = await _httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(content);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                        return Enumerable.Empty<ConsultasModel>();
                    }

                    return consultas;
                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                    return Enumerable.Empty<ConsultasModel>();
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
                return Enumerable.Empty<ConsultasModel>();
            }
        }



        //                                    FILTRADO                    
        [HttpPost]
        public async Task<ActionResult> Filtrado(FiltradoModel filtrado)
        {
            try
            {
                if (filtrado.FechaConsulta != null || filtrado.FechaFinal != null || filtrado.NumeroConsultorio != null || filtrado.NombreTecnico != null || filtrado.IdUsuario != null)
                {
                    // Serializar el modelo de filtrado a JSON
                    var jsonContent = JsonConvert.SerializeObject(filtrado);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Realizar la solicitud POST a la API
                    var respuesta = await _httpClient.PostAsync("Consultas/listaPorAtributos", content);

                    // Serializar el modelo de filtrado a JSON
                    if (respuesta.IsSuccessStatusCode)
                    {
                        // Leer y deserializar el contenido de la respuesta
                        var responseContent = await respuesta.Content.ReadAsStringAsync();
                        var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(responseContent)
                 .OrderBy(c => c.Fecha_Consulta)
                 .ToList();

                        if (consultas == null)
                        {
                            ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                            return View();
                        }
                        var consultasJson = JsonConvert.SerializeObject(consultas);
                        HttpContext.Session.SetString("consultasFiltradas", consultasJson);
                        var excelFile = GenerarExcel(consultas);
                        HttpContext.Session.Set("ArchivoExcel", excelFile);
                        return PartialView("Filtrado", consultas);
                    }
                    else
                    {
                        // Leer el contenido del error y mostrarlo
                        var errorContent = await respuesta.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                        return PartialView();
                    }
                }
                else
                {
                    var consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
                    IEnumerable<ConsultasModel> consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                        return View();
                    }
                    HttpContext.Session.SetString("consultasFiltradas", consultasJson);
                    return PartialView("Filtrado", consultas);
                }

            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
                return PartialView();
            }
        }
        private byte[] GenerarExcel(IEnumerable<ConsultasModel> consultas)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Consultas");

                // Agregar encabezados
                worksheet.Cell(1, 1).Value = "Fecha Consulta";
                worksheet.Cell(1, 2).Value = "Hora Inicio";
                worksheet.Cell(1, 3).Value = "Hora Fin";
                worksheet.Cell(1, 4).Value = "Nombre Usuario";
                worksheet.Cell(1, 5).Value = "Nombre Empleado";
                worksheet.Cell(1, 6).Value = "Número Consultorio";

                // Agregar datos
                int fila = 2;
                foreach (var consulta in consultas)
                {
                    worksheet.Cell(fila, 1).Value = consulta.Fecha_Consulta?.ToString("yyyy MM dd") ?? "";  // Fecha de consulta en formato yyyy MM dd
                    worksheet.Cell(fila, 2).Value = consulta.Hora_inicio;   // Hora de inicio
                    worksheet.Cell(fila, 3).Value = consulta.Hora_fin;      // Hora de fin
                    worksheet.Cell(fila, 4).Value = consulta.NombreUsuario; // Nombre del usuario
                    worksheet.Cell(fila, 5).Value = consulta.Nombre_empleado; // Nombre del empleado
                    worksheet.Cell(fila, 6).Value = consulta.n_consultorio?.ToString() ?? ""; // Número de consultorio
                    fila++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        [HttpPost]
        public ActionResult DescargarExcel()
        {
            // Obtener las consultas almacenadas en la sesión
            var consultasJson = HttpContext.Session.GetString("consultasFiltradas");
            var consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);

            // Generar el archivo Excel
            var excelBytes = GenerarExcel(consultas);

            // Devolver el archivo Excel
            return File(excelBytes, "application/vnd.openxmlformats officedocument.spreadsheetml.sheet", "ConsultasFiltradas.xlsx");
        }



        //                RELEVAMIENTO DE ASISTENCIA                   
        public async Task<ActionResult> PasarAsistenciasDelDia()
        {

            var consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
            if (consultasJson == null)
            {
                await ActualizarConsultasPendientes();
                consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
            }
            IEnumerable<ConsultasModel> consultasPendientes = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);
            IEnumerable<ConsultasModel> consultasSinAsistencia = consultasPendientes.Where(c => c.Asistencia_usuario_recepcion == "" || c.Asistencia_usuario_recepcion == null || c.Asistencia_tecnicos_recepcion == "" || c.Asistencia_tecnicos_recepcion == null && c.Fecha_Consulta == DateTime.Today.Date).ToList();

            // Obtener el mensaje de éxito de TempData
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(consultasSinAsistencia);

        }

        [HttpPost]
        public async Task<IActionResult> PasarAsistencia(ConsultasModel model, IFormFile certificadoInasistencia)
        {
            try
            {
                // Procesar archivo si se ha subido
                if (certificadoInasistencia != null && certificadoInasistencia.Length > 0)
                {
                    var filePath = Path.Combine("ruta/donde/guardar", certificadoInasistencia.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await certificadoInasistencia.CopyToAsync(stream);
                    }
                    model.Certificado_recepcion = filePath;
                }

                // Serializar el modelo a JSON
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Realizar la solicitud PUT a la API
                var respuesta = await _httpClient.PutAsync("Consultas/PasarAsistencia", content);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Actualizar la lista de consultas y redirigir a la vista con un mensaje de éxito
                    ActualizarConsultasEditada(model);
                    return Json(new { success = true });
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        //FUNCIONALIDAD DE ACTUALIZACION
        private async Task ActualizarConsultas()
        {
            try
            {
                // Obtener las consultas actuales desde la API
                IEnumerable<ConsultasModel> consultasList = (await ObtenerConsultasAsync()).ToList();
                consultasList = consultasList.Where(C => C.Fecha_Consulta == DateTime.Today.Date);

                if (consultasList == null)
                {
                    throw new Exception("No se han podido obtener las consultas.");
                }

                // Convertir la lista a JSON
                var consultasJson = JsonConvert.SerializeObject(consultasList);
                // Guardar en la sesión
                HttpContext.Session.SetString("ConsultasDelDia", consultasJson);
            }
            catch (Exception ex)
            {
                // Manejar excepciones si es necesario
                Console.WriteLine($"Error al actualizar las consultas: {ex.Message}");
            }
        }
        private async Task ActualizarConsultasPendientes()
        {
            // Llama a la API para obtener la lista de consultas pendientes
            await ObtenerConsultasPendientes();

            // Alternativamente, si tienes un método para obtener las consultas actualizadas directamente, utilízalo aquí
            string consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
            if (string.IsNullOrEmpty(consultasJson))
            {
                throw new Exception("No se han podido obtener las consultas pendientes.");
            }
        }
        private void ActualizarConsultasEditada(ConsultasModel model)
        {
            // Obtén la lista de consultas pendientes de la sesión
            string consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
            IEnumerable<ConsultasModel> listaDeConsultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);

            // Encuentra la consulta correspondiente y actualiza sus valores
            ConsultasModel consulta = listaDeConsultas.FirstOrDefault(c => c.ID == model.ID);
            if (consulta != null)
            {
                // Si se proporciona información sobre el técnico, actualiza solo la asistencia del técnico y los motivos relacionados
                if (!string.IsNullOrEmpty(model.Asistencia_tecnicos_recepcion) || !string.IsNullOrEmpty(model.Motivo_inasistencia_tecnico_recepcion))
                {
                    consulta.Asistencia_tecnicos_recepcion = model.Asistencia_tecnicos_recepcion;
                    consulta.Motivo_inasistencia_tecnico_recepcion = model.Motivo_inasistencia_tecnico_recepcion;
                }

                // Si se proporciona información sobre el usuario, actualiza solo la asistencia del usuario y los motivos relacionados
                if (!string.IsNullOrEmpty(model.Asistencia_usuario_recepcion) || !string.IsNullOrEmpty(model.Motivo_inasistencia))
                {
                    consulta.Asistencia_usuario_recepcion = model.Asistencia_usuario_recepcion;
                    consulta.Motivo_inasistencia = model.Motivo_inasistencia;
                    consulta.Certificado_recepcion = model.Certificado_recepcion;
                }
            }

            // Vuelve a serializar la lista de consultas a JSON
            string consultasActualizadasJson = JsonConvert.SerializeObject(listaDeConsultas);

            // Guarda la lista actualizada en la sesión
            HttpContext.Session.SetString("ConsultasPendientes", consultasActualizadasJson);
        }

        //                           EDITAR CONSULTAS        

        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                ConsultasModel consultaCache = ObtenerConsultaDesdeCache(id);
                if (consultaCache == null)
                {
                    consultaCache = await ObtenerConsultaEditable(id);
                }

                return PartialView("_Edit", consultaCache);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en Edit: {ex.Message}");
                throw; // Mantener la pila de excepciones
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditForm(int id, ConsultasModel consulta)
        {
            try
            {
                EditarFechaModel nuevaConsulta = new EditarFechaModel()
                {
                    Id = consulta.ID,
                    NuevaFecha = consulta.Fecha_Consulta,
                    Certificado_recepcion = consulta.Certificado_recepcion,
                    Comentarios_tecnicos_recepcion = consulta.Comentarios_tecnicos_recepcion,
                    Comentarios_recepcion = consulta.Comentarios_recepcion,
                    Asistencia_tecnicos_recepcion = consulta.Asistencia_tecnicos_recepcion,
                    Asistencia_usuario_recepcion = consulta.Asistencia_usuario_recepcion,
                    Hora_inicio = consulta.Hora_inicio,
                    Hora_fin = consulta.Hora_fin

                };

                var jsonData = JsonConvert.SerializeObject(nuevaConsulta);
                var contenido = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = $"{_url}Consultas/editar fecha";

                var respuesta = await _httpClient.PutAsync(url, contenido);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "La consulta fue editada correctamente.";
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al autorizar la solicitud: {errorContent}";
                }

                // Actualizar las consultas después de la edición
                await ActualizarConsultas();

                // Redirigir a la vista parcial actualizada
                return PartialView("_vistaTablaxDia", await ListarConsultasPorDia(DateTime.Now.Date));
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return PartialView("_EditForm", consulta); // Volver a mostrar el formulario en caso de error
            }
        }

        public async Task<ActionResult> AgregarComentarioAConsulta(string idSesion)
        {
            // Asegúrate de que 'ObtenerConsultaEditable' devuelve un objeto no nulo
            ConsultasModel consulta = await ObtenerConsultaEditable(idSesion);

            if (consulta == null)
            {
                return NotFound(); // Maneja el caso en que no se encuentra la consulta
            }

            return PartialView("_AgregarComentario", consulta); // Asegúrate de que el modelo se llena correctamente
        }
        [HttpPost]
        public async Task<JsonResult> AgregarComentarioAConsulta(string idSesion, string comentarioNuevo)
        {
            ComentarioRequest comentario = new ComentarioRequest(idSesion, comentarioNuevo);
            var contenido = new StringContent(JsonConvert.SerializeObject(comentario), Encoding.UTF8, "application/json");
            string url = $"{_url}Consultas/AgregarComentarioASesion";

            var respuesta = await _httpClient.PutAsync(url, contenido);

            if (respuesta.IsSuccessStatusCode)
            {
                // Retornar éxito y la URL de redirección
                return Json(new { success = true, message = "Comentario agregado correctamente.", redirectUrl = Url.Action("Detalles", "Consultas", new { id = idSesion }) });
            }
            else
            {
                var errorContent = await respuesta.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Error: {errorContent}" });
            }
        }




        public async Task<ConsultasModel> ObtenerConsultaEditable(string id)
        {
            ConsultasModel consultaRetorno = null;
            // Obtener las consultas del día
            var consultorios = await ObtenerConsultas();

            // Buscar la consulta específica por ID
            foreach (var consultorio in consultorios)
            {
                foreach (var horario in consultorio.Horarios)
                {
                    foreach (ConsultasModel consulta in horario.Value)
                    {
                        Console.WriteLine($"Buscando consulta: ID={consulta.ID}, Horario={horario.Key}");
                        if (consulta.ID == id)
                        {
                            consultaRetorno = consulta;
                            return consulta;
                        }
                    }
                }
            }
            if (consultaRetorno != null)
            {
                return consultaRetorno;
            }
            if (consultaRetorno == null)
            {
                string url = $"Consultas/ObtenerConsultaPorId/{id}";

                // Realizar la solicitud GET a la API
                var respuesta = await _httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var consultas = JsonConvert.DeserializeObject<ConsultasModel>(content);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el contenido de las consultas.";
                    }
                    consultaRetorno = consultas;

                }
            }
            return consultaRetorno;
        }



        // GET: ConsultasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ConsultasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConsultasController/Create
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






        public async Task<List<ConsultorioModel>> ObtenerConsultas()
        {

            try
            {


                var consultasDelDia = new List<ConsultasModel>();
                // Obtener las consultas desde la API si no están en la sesión
                var consultasList = await ObtenerConsultasAsync();

                if (consultasList == null)
                {
                    ViewBag.Error = "No se han podido obtener las consultas.";
                    return new List<ConsultorioModel>();
                }

                // Convertir la lista a JSON y guardarla en la sesión
                var consultasJson = JsonConvert.SerializeObject(consultasList);
                HttpContext.Session.SetString("ConsultasDelDia", consultasJson);
                consultasDelDia = JsonConvert.DeserializeObject<List<ConsultasModel>>(consultasJson);
                consultasDelDia = (List<ConsultasModel>)consultasList;

                // Agrupar las consultas por consultorio
                var groupedConsultas = consultasDelDia
                    .GroupBy(c => c.n_consultorio)
                    .Select(g => new ConsultorioModel
                    {
                        NroConsultorio = g.Key,
                        Horarios = g
                            .GroupBy(c => c.Hora_inicio)
                            .ToDictionary(hg => hg.Key, hg => hg.ToList())
                    })
                    .ToList();

                // Ordenar las consultas si es necesario
                groupedConsultas.Sort();

                return groupedConsultas;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public ConsultasModel ObtenerConsultaDesdeCache(string id)
        {
            var Consultas = HttpContext.Session.GetString("ConsultasDelDia");
            IEnumerable<ConsultasModel> consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(Consultas);
            var consultaEditable = consultas.Where(c => c.ID == id).First();
            return consultaEditable;
        }


        // GET: ConsultasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConsultasController/Delete/5
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


        //LISTAR CONSULTAS

        public ActionResult MostrarCalendario()
        {
            try
            {
                List<ConsultasModel> consultas = new List<ConsultasModel>();

                consultas.Sort((a, b) => (a.Hora_inicio).CompareTo(b.Hora_inicio));
                ViewBag.Consultas = consultas;

                return View();
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<JsonResult> BuscarTecnico(string term)
        {

            var jsonTecnico = HttpContext.Session.GetString("Empleados");
            if (jsonTecnico == null)
            {
                await cargarMedicos();
                jsonTecnico = HttpContext.Session.GetString("Empleados");
            }
            var Empleados = JsonConvert.DeserializeObject<List<Empleados>>(jsonTecnico);

            // Crea una lista de objetos anónimos con la estructura deseada
            var coincidencias = Empleados.Where(u =>
            $"{u.NombreCompleto.PrimerNombre} {u.NombreCompleto.PrimerApellido}".Contains(term, StringComparison.OrdinalIgnoreCase)
        ).Select(u => new
        {
            PrimerNombre = u.NombreCompleto.PrimerNombre,
            PrimerApellido = u.NombreCompleto.PrimerApellido,
            ID = u.IdEmpleado.ToString(),
        }).ToList();

            return Json(coincidencias);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarUsuario(string term)
        {
            var jsonTecnico = HttpContext.Session.GetString("Pacientes");
            if (jsonTecnico == null)
            {
                await cargarUsuarios();
                jsonTecnico = HttpContext.Session.GetString("Usuarios");
            }
            var Usuarios = JsonConvert.DeserializeObject<List<Usuario>>(jsonTecnico);

            var coincidencias = Usuarios
                .Where(u => $"{u.NombreCompleto.PrimerNombre} {u.NombreCompleto.PrimerApellido}"
                .Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(u => new
                {
                    PrimerNombre = u.NombreCompleto.PrimerNombre,
                    PrimerApellido = u.NombreCompleto.PrimerApellido,
                    ID = u.Id // Asegúrate de que este campo exista y tenga valor
                })
                .ToList();

            return Json(coincidencias);
        }

        public async Task cargarMedicos()
        {
            try
            {

                string url = $"{_url}Empleados/ObtenerTodosLosEmpleados";



                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var medicos = JsonConvert.DeserializeObject<List<Empleados>>(content);

                    if (medicos == null)
                    {
                        ViewBag.Error = "No se han podido obtener los médicos.";
                        return; // Salir si no se pueden deserializar los médicos
                    }

                    var empleadosJson = JsonConvert.SerializeObject(medicos);
                    HttpContext.Session.SetString("Empleados", empleadosJson);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("La operación fue cancelada o tomó demasiado tiempo: " + ex.Message);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error en la solicitud HTTP: " + ex.Message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error: " + ex.Message);
            }

        }
        public async Task cargarUsuarios()
        {
            try
            {

                string url = $"{_url}Usuario/ObtenerUsuariosActivos";



                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(content);

                    if (usuarios == null)
                    {
                        ViewBag.Error = "No se han podido obtener los médicos.";
                        return; // Salir si no se pueden deserializar los médicos
                    }

                    var empleadosJson = JsonConvert.SerializeObject(usuarios);
                    HttpContext.Session.SetString("Usuarios", empleadosJson);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("La operación fue cancelada o tomó demasiado tiempo: " + ex.Message);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error en la solicitud HTTP: " + ex.Message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de red: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error: " + ex.Message);
            }

        }


        //PASAR ASISTENCIA DE UNA CONSULTA
        //CANCELAR CONSULTA(SOLO LA DE ESE DIA)

        [HttpGet]
        public async Task<IActionResult> cancelarConsulta()
        {
            var consultasOrdenadas = await ObtenerConsultasPorDia(DateTime.Today.Date);
            return View("cancelarConsulta", consultasOrdenadas);
        }

        public async Task<IActionResult> cancelarConsultaXfecha(DateTime fecha)
        {
            // Asegúrate de que si la fecha es mínima, la reemplazas con la fecha actual.
            if (fecha == DateTime.MinValue)
            {
                fecha = DateTime.Today.Date;
            }

            // Obtiene las consultas para la fecha dada.
            var consultasOrdenadas = await ObtenerConsultasPorDia(fecha);

            // Redirige a la vista "cancelarConsulta" pasando las consultas ordenadas.
            return View("cancelarConsulta", consultasOrdenadas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> cancelarConsulta(int id, ConsultasModel consulta)
        {
            try
            {
                EditarFechaModel nuevaConsulta = new EditarFechaModel()
                {
                    Id = consulta.ID,
                    NuevaFecha = consulta.Fecha_Consulta,
                    Certificado_recepcion = consulta.Certificado_recepcion,
                    Comentarios_tecnicos_recepcion = consulta.Comentarios_tecnicos_recepcion,
                    Comentarios_recepcion = consulta.Comentarios_recepcion,
                    Asistencia_tecnicos_recepcion = consulta.Asistencia_tecnicos_recepcion,
                    Asistencia_usuario_recepcion = consulta.Asistencia_usuario_recepcion,
                    Hora_inicio = consulta.Hora_inicio,
                    Hora_fin = consulta.Hora_fin
                };

                var jsonData = JsonConvert.SerializeObject(nuevaConsulta);
                var contenido = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = $"{_url}Consultas/CancelarConsulta";

                var respuesta = await _httpClient.PutAsync(url, contenido);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "La consulta fue editada correctamente.";
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al autorizar la solicitud: {errorContent}";
                }

                // Actualizar las consultas después de la edición
                await ActualizarConsultas();

                // Redirigir a la vista "cancelarConsulta"
                return RedirectToAction("cancelarConsulta");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return RedirectToAction("cancelarConsulta"); // Redirigir a cancelarConsulta en caso de error
            }
        }
        [HttpPost]
        public async Task<IActionResult> cancelarConsulta2(string id, DateTime fecha, string sucursal, string motivo)
        {
            try
            {
                EditarFechaModel nuevaConsulta = new EditarFechaModel()
                {
                    Id = id,
                    sucursal = sucursal,
                    NuevaFecha = fecha,
                    Comentarios_recepcion = motivo,
                    Asistencia_tecnicos_recepcion = "Cancelada",
                    Asistencia_usuario_recepcion = "Cancelada"
                };

                var jsonData = JsonConvert.SerializeObject(nuevaConsulta);
                var contenido = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = $"{_url}Consultas/cancelar consulta";

                var respuesta = await _httpClient.PutAsync(url, contenido);
                if (respuesta.IsSuccessStatusCode)
                {
                    // Recuperar las consultas pendientes de la sesión
                    var listaConsultasJson = await ObtenerConsultasAsync();
                    // Buscar la consulta por ID
                    var consultaAEditar = listaConsultasJson.FirstOrDefault(c => c.ID == id);
                    if (consultaAEditar != null)
                    {
                        // Modificar los atributos de la consulta
                        consultaAEditar.Fecha_Consulta = fecha;
                        consultaAEditar.Sucursal = sucursal;
                        consultaAEditar.Comentarios_recepcion = motivo;
                        consultaAEditar.Asistencia_tecnicos_recepcion = "Cancelada";
                        consultaAEditar.Asistencia_usuario_recepcion = "Cancelada";

                        TempData["Mensaje"] = "La consulta fue editada correctamente.";
                    }
                    ActualizarConsultasEditada(consultaAEditar);
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al autorizar la solicitud: {errorContent}";
                }

                // Redirigir a la vista "cancelarConsulta"
                return RedirectToAction("cancelarConsulta");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return RedirectToAction("cancelarConsulta"); // Redirigir a cancelarConsulta en caso de error
            }
        }


        [HttpPost]
        public async Task<IActionResult> CancelarTodasLasConsultasDelDia(DateTime fechaConsulta, string motivo)
        {
            try
            {
                EditarFechaModel nuevaConsulta = new EditarFechaModel()
                {
                    fecha_Inicio = fechaConsulta,
                    Motivo_inasistencia = motivo,
                    sucursal = HttpContext.Session.GetString("Sucursal")
                };

                var jsonData = JsonConvert.SerializeObject(nuevaConsulta);
                var contenido = new StringContent(jsonData, Encoding.UTF8, "application/json");

                string url = $"{_url}Consultas/cancelar consulta deDia";

                var respuesta = await _httpClient.PutAsync(url, contenido);

                if (respuesta.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "La consulta fue editada correctamente.";
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al autorizar la solicitud: {errorContent}";
                }

                // Actualizar las consultas después de la edición
                await ActualizarConsultas();

                // Redirigir a la vista "cancelarConsulta"
                return RedirectToAction("cancelarConsulta");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return RedirectToAction("cancelarConsulta"); // Redirigir a cancelarConsulta en caso de error
            }
        }


        [HttpGet]
        public async Task<IEnumerable<ConsultasModel>> ObtenerConsultasPorDia(DateTime? fecha)
        {
            // Si no se proporciona una fecha, utilizamos la fecha actual
            var fechaConsultas = fecha ?? DateTime.Now;

            // Obtener la cadena JSON desde la sesión, pero ahora incluirá la fecha
            string consultasJson = HttpContext.Session.GetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}");

            List<ConsultasModel> consultasDelDia;

            // Obtener las consultas desde la API para la fecha específica
            var consultasList = (await ObtenerConsultasAsync2(fechaConsultas)).ToList();

            // Convertir la lista a JSON y guardarla en la sesión con la fecha
            consultasJson = JsonConvert.SerializeObject(consultasList);
            HttpContext.Session.SetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}", consultasJson);

            consultasDelDia = consultasList;

            var sucursal = HttpContext.Session.GetString("Sucursal");

            // Filtrar las consultas por la sucursal
            var consultasFiltradas = consultasDelDia
                .Where(c => c.Sucursal == sucursal).OrderBy(c => c.Hora_inicio)
                .ToList();

            return consultasFiltradas;
        }

        //Obtener consultas devuelve json 
        [HttpGet]
        public async Task<JsonResult> ObtenerConsultasPorDiaFran(DateTime? fecha)
        {
            try
            {
                // Si no se proporciona una fecha, utilizamos la fecha actual
                var fechaConsultas = fecha ?? DateTime.Now;
                string consultasJson = HttpContext.Session.GetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}");
                List<ConsultasModel> consultasDelDia;

                // Obtener las consultas desde la API para la fecha específica
                var consultasList = (await ObtenerConsultasAsync2(fechaConsultas)).ToList();

                if (consultasList == null || !consultasList.Any())
                {
                    return Json(new { success = false, message = "No se encontraron consultas para la fecha especificada" });
                }

                // Convertir la lista a JSON y guardarla en la sesión con la fecha
                consultasJson = JsonConvert.SerializeObject(consultasList);
                HttpContext.Session.SetString($"ConsultasDelDia_{fechaConsultas:yyyyMMdd}", consultasJson);

                consultasDelDia = consultasList;
                var sucursal = HttpContext.Session.GetString("Sucursal");

                // Filtrar las consultas por la sucursal
                var consultasFiltradas = consultasDelDia
                    .Where(c => c.Sucursal == sucursal)
                    .OrderBy(c => c.Hora_inicio)
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = consultasFiltradas,
                    message = "Consultas obtenidas exitosamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error al obtener las consultas: {ex.Message}"
                });
            }
        }


        //Cambiar estado de consulta a asiste o no asiste 
        [HttpPost]
        public async Task<IActionResult> ActualizarAsistenciaPost(string id, string asistenciaUsuarioRecepcion, string motivoInasistencia)
        {
            try
            {
                // Crear el modelo de actualización
                var actualizacion = new ConsultasModel
                {
                    ID = id,
                    Asistencia_usuario_recepcion = asistenciaUsuarioRecepcion != null ? asistenciaUsuarioRecepcion : "",
                    Motivo_inasistencia = asistenciaUsuarioRecepcion != null ? motivoInasistencia : "",
                    Sucursal = HttpContext.Session.GetString("Sucursal")
                };

                var jsonData = JsonConvert.SerializeObject(actualizacion);
                var contenido = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // URL del endpoint
                string url = $"{_url}Consultas/PasarAsistencia";

                // Realizar la petición PUT
                var respuesta = await _httpClient.PutAsync(url, contenido);
                var responseContent = await respuesta.Content.ReadAsStringAsync();

                if (respuesta.IsSuccessStatusCode)
                {
                    // Actualizar la consulta en la sesión
                    await ActualizarConsultas();

                    return Json(new
                    {
                        success = true,
                        message = "Asistencia actualizada correctamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error al actualizar la asistencia: {responseContent}"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}"
                });
            }
        }





    }





}

