using ClinicaEspacioAbiertoFrontEnd.Controllers.Empleado;
using ClinicaEspacioAbiertoFrontEnd.Middleware;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades;
using ClinicaEspacioAbiertoFrontEnd.Models.Entidades.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers
{
    public class PagosController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IConsultasService _consultasService;
        private readonly EmpleadoSistemasController sistemasController;
        private readonly string _url = "https://sistemaapidegestiondeclinicasv2-affxc4hdbvfjewce.canadacentral-01.azurewebsites.net/api/";
        private readonly JsonSerializerOptions _jsonOptions
            = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


        public PagosController(HttpClient httpClient, IConsultasService consultasService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_url);
            _httpClient.Timeout = TimeSpan.FromSeconds(100);
            _consultasService = consultasService;

        }

        public async Task<IActionResult> DetailsPagos()
        {
            var pagosJson = HttpContext.Session.GetString("Pagos");
            IEnumerable<Pagos> pagos = JsonConvert.DeserializeObject<IEnumerable<Pagos>>(pagosJson);
            // Suponiendo que la propiedad que representa la fecha de pago se llama `FechaPago`
            IEnumerable<Pagos> pagosOrdenados = pagos.OrderByDescending(p => p.fecha_pago);

            return View(pagosOrdenados); ;


        }

        public async Task<ActionResult> AgregarPagos(string idConsulta)
        {
            try
            {
                string consultasJson = HttpContext.Session.GetString("ConsultasDelDia");
                IEnumerable<ConsultasModel> listaDeConsultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);
                ConsultasModel consultaEditable = listaDeConsultas.FirstOrDefault(c => c.ID == idConsulta);
                if (consultaEditable == null)
                {
                    consultasJson = HttpContext.Session.GetString("ConsultasPendientes");
                    listaDeConsultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);
                    consultaEditable = listaDeConsultas.FirstOrDefault(c => c.ID == idConsulta);
                    if (consultaEditable == null)
                    {
                        throw new Exception("No se encontró la consulta entre las pendientes y editables.");
                    }
                    Pagos pago = new Pagos()
                    {
                        fecha_consulta = consultaEditable.Fecha_Consulta.Value,
                        nombre_Empleado = consultaEditable.Nombre_empleado,
                        nombre_Usuario = consultaEditable.NombreUsuario,
                        Hora_Fin = consultaEditable.Hora_fin,
                        Hora_inicio = consultaEditable.Hora_inicio,
                        sucursalSesion = consultaEditable.Sucursal,
                        id_consulta = consultaEditable.ID,
                        convenio = consultaEditable.Convenio,
                        estado = consultaEditable.Estado,
                        insertdate = DateTime.Now,
                        idUsuario = consultaEditable.ID_Usuario,
                        Tipo = new TipoPago()
                    };
                    return PartialView("_AgregarPagos", pago);

                }
                else
                {
                    Pagos pago = new Pagos()
                    {
                        fecha_consulta = consultaEditable.Fecha_Consulta.Value,
                        nombre_Empleado = consultaEditable.Nombre_empleado,
                        nombre_Usuario = consultaEditable.NombreUsuario,
                        Hora_Fin = consultaEditable.Hora_fin,
                        Hora_inicio = consultaEditable.Hora_inicio,
                        sucursalSesion = consultaEditable.Sucursal,
                        id_consulta = consultaEditable.ID,
                        convenio = consultaEditable.Convenio,
                        estado = consultaEditable.Estado,
                        insertdate = DateTime.Now,
                        idUsuario = consultaEditable.ID_Usuario,
                        Tipo = new TipoPago()
                    };
                    return PartialView("_AgregarPagos", pago);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<IActionResult> AgregarPagos(Pagos pagos)
        {
            try
            {
                if (pagos == null)
                {
                    throw new Exception("La consulta no es válida.");
                }

                pagos.userEmail = HttpContext.Session.GetString("Email");
                pagos.estado_conciliado = "Pagado";

                string url = $"{_url}Cobros/AgregarCobro";
                var respuesta = await _httpClient.PostAsJsonAsync(url, pagos);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Generar el PDF utilizando QuestPDF o IronPDF
                    var pdfBytes = GenerarPdfFactura(pagos);

                    // Crear un nombre único para el archivo
                    var fileName = $"FacturaPago_{Guid.NewGuid()}.pdf";

                    // Convertir el PDF a base64
                    var pdfBase64 = Convert.ToBase64String(pdfBytes);
                    var filePath = $"/Temp/{fileName}";

                    if (pagos.id_consulta != "" && pagos.id_consulta != null)
                    {
                        EliminarConsultaPendiente(pagos.id_consulta);
                    }
                    // Eliminar la consulta pendiente de la lista y actualizar la sesión


                    // *** Nueva llamada para actualizar las consultas pendientes ***
                    await _httpClient.GetAsync("Consultas/ActualizarConsultasPendientes");
                    await ActualizarPagos();

                    ViewBag.FilePath = filePath;
                    ViewBag.Mensaje = "Pago agregado correctamente.";

                    return PartialView("_ConfirmacionPago", new ConfirmacionPagoViewModel
                    {
                        Mensaje = ViewBag.Mensaje,
                        FilePath = filePath,
                        PdfBase64 = pdfBase64,
                        pago = pagos
                    });
                }
                else
                {
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"Ocurrió un error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // Manejar el error
                ViewBag.Error = ex.Message;
                return PartialView("_Error");
            }
        }

        private byte[] GenerarPdfFactura(Pagos pagos)
        {
            // Ruta de la imagen en wwwroot/assets
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "Logo-Header-1.jpg");
            byte[] imageBytes;

            // Leer la imagen como un array de bytes
            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }
            }

            // Crear el documento PDF usando QuestPDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A6);

                    page.Header().Text("Factura de Pago").FontSize(20).Bold().AlignCenter();

                    page.Content().Column(column =>
                    {
                        // Ajustar el tamaño de la imagen a 1 pulgada de ancho
                        column.Item().AlignLeft().Width(1, Unit.Inch).Image(imageBytes);

                        // Contenido del pago
                        column.Item().Text($"Nombre del Usuario: {pagos.nombre_Usuario}");
                        column.Item().Text($"Nombre del Empleado: {pagos.nombre_Empleado}");
                        column.Item().Text($"Fecha de Pago: {pagos.fecha_pago.ToString("dd/MM/yyyy")}");
                        column.Item().Text($"Convenio: {pagos.convenio}");
                        column.Item().Text($"Monto de Pago: {pagos.Importe:C}");
                    });
                });
            });

            // Convertir el documento PDF a un array de bytes
            return document.GeneratePdf();
        }


        public void EliminarConsultaPendiente(string idConsulta)
        {
            string consultasJson = HttpContext.Session.GetString("ConsultasDelDia");
            IEnumerable<ConsultasModel> listaDeConsultas = JsonConvert.DeserializeObject<IEnumerable<ConsultasModel>>(consultasJson);

            if (listaDeConsultas != null)
            {
                // Encuentra la consulta que deseas eliminar
                var consultaAActualizar = listaDeConsultas.FirstOrDefault(c => c.ID == idConsulta);

                if (consultaAActualizar != null)
                {
                    // Actualiza el campo de pago
                    consultaAActualizar.Pago = "Pagado";

                    // Vuelve a serializar la lista actualizada a JSON
                    string consultasActualizadasJson = JsonConvert.SerializeObject(listaDeConsultas);

                    // Actualiza el JSON en la sesión
                    HttpContext.Session.SetString("ConsultasPendientes", consultasActualizadasJson);
                }
            }
        }
        public JsonResult BuscarPagos(string term)
        {
            var jsonPagos = HttpContext.Session.GetString("Pagos");
            var pagos = JsonConvert.DeserializeObject<IEnumerable<Pagos>>(jsonPagos).Where(p => p.nombre_Usuario.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                            p.nombre_Empleado.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Json(pagos);
        }
        public async Task<IActionResult> pagosPorMesPartial()
        {
            var jsonPagos = HttpContext.Session.GetString("Pagos");
            IEnumerable<Pagos> pagos = JsonConvert.DeserializeObject<IEnumerable<Pagos>>(jsonPagos);

            var pagosPorMes = pagos
                .GroupBy(p => new { p.fecha_pago.Year, p.fecha_pago.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Mes = $"{g.Key.Month}/{g.Key.Year}",
                    TotalMes = g.Sum(p => p.Importe)
                }).ToList();

            // Verificar los datos
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                labels = pagosPorMes.Select(p => p.Mes).ToArray(),
                data = pagosPorMes.Select(p => (decimal)p.TotalMes).ToArray()
            }));

            return Json(new
            {
                labels = pagosPorMes.Select(p => p.Mes).ToArray(),
                data = pagosPorMes.Select(p => (decimal)p.TotalMes).ToArray()
            });
        }


        public async Task<ActionResult> UltimosPagosIngresados()
        {
            var jsonPagos = HttpContext.Session.GetString("Pagos");
            var sucursal = HttpContext.Session.GetString("Sucursal");
            IEnumerable<Pagos> pagos = JsonConvert.DeserializeObject<IEnumerable<Pagos>>(jsonPagos).OrderByDescending(c => c.fecha_pago).Take(7)
                .ToList();
            if (pagos == null)
            {
                await ActualizarPagos();
                jsonPagos = HttpContext.Session.GetString("Pagos");
            }

            if (string.IsNullOrEmpty(jsonPagos))
            {
                return PartialView("_ListaUltimoPagosDelDia", new List<Pagos>());
            }
            return PartialView("_ListaUltimoPagosDelDia", pagos);
        }

        public async Task ActualizarPagos()
        {
            try
            {
                HttpContext.Session.Remove("Pagos");

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


        // GET: PagosController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagosController1/Create
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

        // GET: PagosController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PagosController1/Edit/5
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

        // GET: PagosController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PagosController1/Delete/5
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
