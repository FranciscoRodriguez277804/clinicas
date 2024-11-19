using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Empleado
{
    public class EmpleadoSistemasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _url = "https://sistemaapidegestiondeclinicasv2-affxc4hdbvfjewce.canadacentral-01.azurewebsites.net/api/";
        private readonly string _url2 = "https://localhost:7231/api/";
        private readonly JsonSerializerOptions _jsonOptions
            = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };



        public EmpleadoSistemasController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_url);
            _httpClient.Timeout = TimeSpan.FromSeconds(100); // Ajusta según tus necesidades

        }

        // GET: EmpleadoSistemasController
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Autorizar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AutorizarAsync(LoginModel loginModel)
        {

            try
            {

                // Construir la URL para la solicitud GET
                string url = $"{_url}Empleados/login";

                var requestData = new
                {
                    cedula = loginModel.Cedula,
                    contraseña = loginModel.Password
                };
                if (loginModel.Cedula == null || loginModel.Cedula == "")
                {
                    throw new Exception("Ingrese una cedula correcta");
                }
                if (loginModel.Password == "" || loginModel.Password == null)
                {
                    throw new Exception("Ingrese una contraseña valida");
                }
                string jsonContent = JsonConvert.SerializeObject(requestData);

                StringContent contentApi = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Enviar la solicitud POST a la API
                HttpResponseMessage respuesta = await _httpClient.PostAsync(url, contentApi);
                // Realizar la solicitud GET a la API

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer y deserializar el contenido de la respuesta
                    var content = await respuesta.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<Empleados>(content);

                    if (token == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar el token";
                        return View(loginModel);
                    }

                    // Guardar datos en la sesión
                    HttpContext.Session.SetString("Rol", token.area_empleado);
                    HttpContext.Session.SetString("Nombre", token.NombreCompleto.PrimerNombre);
                    HttpContext.Session.SetString("Apellido", token.NombreCompleto.PrimerApellido);
                    HttpContext.Session.SetString("id", token.IdEmpleado);
                    HttpContext.Session.SetString("Email", token.UserEmail);
                    HttpContext.Session.SetString("Token", token.Token);
                    HttpContext.Session.SetString("Empleado", content);
                    await cargarPagos();
                    await ObtenerListaDeConvenios();
                    // Redirigir a otra acción
                    return RedirectToAction("SeleccionarSucursal", "EmpleadoSistemas");

                }
                else
                {
                    // Leer el contenido del error y mostrarlo
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error al autorizar la solicitud: {errorContent}";
                    return View(loginModel);
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
                return View(loginModel);
            }
        }


        public async Task<ActionResult> VerDocumentos()
        {
            string carpeta = "1_BndPKCSa8ZJm5DzVqMDpkiRqQwHXqk7"; // ID de la carpeta
            var baseUrl = new Uri(_url);
            var uriBuilder = new UriBuilder(baseUrl)
            {
                Path = "/api/ArchivosDrive/listar",
                Query = $"folderId={carpeta}"
            };

            string apiUrl = uriBuilder.ToString();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var archivos = JsonConvert.DeserializeObject<List<FileMetadata>>(content);
                    return View(archivos);  // Devuelve los archivos a la vista
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public async Task<ActionResult> VerDocumentosEnCarpeta(string folderID)
        {
            var baseUrl = new Uri(_url);
            var uriBuilder = new UriBuilder(baseUrl)
            {
                Path = "/api/ArchivosDrive/listar",
                Query = $"folderId={folderID}"
            };

            string apiUrl = uriBuilder.ToString();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var archivos = JsonConvert.DeserializeObject<List<FileMetadata>>(content);
                    return View("VerDocumentos",archivos);  // Devuelve los archivos a la vista
                }
                else
                {
                    return View("Error");
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> DescargarArchivo(string fileId)
        {
            // Define la URL de tu API backend para descargar el archivo
            string apiUrl = $"{_url}ArchivosDrive/descargar/{fileId}";

            using (var httpClient = new HttpClient())
            {
                // Realiza la solicitud GET a la API para descargar el archivo
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido del archivo descargado en bytes
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentDisposition = response.Content.Headers.ContentDisposition;

                    // Extraer el nombre del archivo del encabezado si existe
                    var fileName = contentDisposition != null ? contentDisposition.FileName.Trim('"').Trim() : "archivo_descargado";

                    // Devolver el archivo como un FileResult para que el navegador lo descargue
                    return File(fileBytes, "application/octet-stream", fileName.Trim());
                }
                else
                {
                    // Si hay un error, puedes redirigir al usuario a una página de error o mostrar un mensaje
                    return View("Error");
                }
            }
        }

            public ActionResult SeleccionarSucursal()
        {

            return View();
        }

        [HttpPost]
        public IActionResult SeleccionarSucursal(SeleccionarSucursalModel nombreSucursal)
        {
            // Guarda la sucursal en la sesión
            HttpContext.Session.SetString("Sucursal", nombreSucursal.Sucursal);


            return RedirectToAction("Inicio", "EmpleadoSistemas");

        }


        public ActionResult Inicio()
        {
            var conveniosJson = HttpContext.Session.GetString("ListaConvenios");

            IEnumerable<Convenio> convenios = new List<Convenio>();

            if (conveniosJson != null)
            {
                convenios = JsonConvert.DeserializeObject<IEnumerable<Convenio>>(conveniosJson);
            }

            return View(convenios); // Pasa el modelo directamente
        }


        public ActionResult TableroEmpleado()
        {
            return View("VistaInicioTecnico");
        }

        public async Task<IEnumerable<ConsultasModel>> cargarProximas5Consultas()
        {
            try
            {
                string url = $"{_url}Consultas/ListarProximas5ConsultasParaTecnico";
                var response = await _httpClient.GetAsync(url);
                IEnumerable<ConsultasModel> consultas = new List<ConsultasModel>();
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    consultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(content);

                    if (consultas == null)
                    {
                        ViewBag.Error = "No se ha podido deserializar la lista de consultas.";
                    }
                }
                return consultas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public async Task cargarPagos()
        {
            try
            {

                string url = $"{_url}Cobros/ObtenerCobros";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var pagos = JsonConvert.DeserializeObject<IEnumerable<Pagos>>(content);

                    if (pagos == null)
                    {
                        ViewBag.Error = "No se han podido obtener los pagos.";
                        return; // Salir si no se pueden deserializar los médicos
                    }

                    var pagosjson = JsonConvert.SerializeObject(pagos);
                    HttpContext.Session.SetString("Pagos", pagosjson);
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

        public async Task cargarConvenios()
        {
            try
            {


                string url = $"{_url}UsuariosConvenios/ObtenerConvenios";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var convenios = JsonConvert.DeserializeObject<IEnumerable<UsuarioConvenio>>(content);

                    if (convenios == null)
                    {
                        ViewBag.Error = "No se han podido obtener los convenios.";
                        return; // Salir si no se pueden deserializar los médicos
                    }
                    var pagosjson = JsonConvert.SerializeObject(convenios);
                    HttpContext.Session.SetString("UsuariosConvenios", pagosjson);
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

        public async Task ObtenerListaDeConvenios()
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

                    // Agregar un registro para verificar el contenido
                    Console.WriteLine(content); // Imprimir el contenido de la respuesta

                    var convenios = JsonConvert.DeserializeObject<IEnumerable<Convenio>>(content);

                    if (convenios != null)
                    {
                        var conveniosJson = JsonConvert.SerializeObject(convenios);
                        HttpContext.Session.SetString("ListaConvenios", conveniosJson);
                    }
                    else
                    {
                        ViewBag.Error = "No se pudo deserializar la lista de convenios.";
                    }
                }
                else
                {
                    ViewBag.Error = "Error al obtener convenios: " + respuesta.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y mostrar errores
                ViewBag.Error = ex.Message;
            }
        }


        public async Task<ActionResult> ListarEmpleados(string area = "")
        {

            string url = $"{_url}Empleados/ObtenerTodosLosEmpleados";



            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var medicos = JsonConvert.DeserializeObject<IEnumerable<Empleados>>(content);

            // Obtener la lista de áreas para el filtro en el formulario
            ViewBag.ListaAreas = medicos.Select(e => e.area_empleado).Distinct().ToList();

            // Si se seleccionó un área, filtrar los empleados por el área seleccionada
            if (!string.IsNullOrEmpty(area))
            {
                medicos = medicos.Where(e => e.area_empleado == area);
            }



            return View(medicos);
        }


        public async Task<ActionResult> MostrarPerfil(string id)
        {
            // Construye la URL con el parámetro id
            string url = $"{_url}Empleados/ObtenerEmpleadoPorId/{id}";

            // Realiza la solicitud GET
            var response = await _httpClient.GetAsync(url);

            // Verifica si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                // Lee el contenido de la respuesta
                var content = await response.Content.ReadAsStringAsync();

                // Deserializa el contenido de la respuesta si es necesario
                var empleado = JsonConvert.DeserializeObject<Empleados>(content);

                // Devuelve la vista con el modelo empleado
                return View(empleado);
            }
            else
            {
                // Maneja el caso de error (puedes redirigir o mostrar un mensaje de error)
                return View("Error");
            }
        }

        // GET: EmpleadoSistemasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmpleadoSistemasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpleadoSistemasController/Create
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

        // GET: EmpleadoSistemasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmpleadoSistemasController/Edit/5
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

        // GET: EmpleadoSistemasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmpleadoSistemasController/Delete/5
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

        public ActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return View("Autorizar");
        }
    }
}
