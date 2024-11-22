using ClinicaEspacioAbiertoFrontEnd.Models;
using ClinicaEspacioAbiertoFrontEnd.Models.Funcionalidades;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private static readonly List<Funciones> Functions = new List<Funciones>
    {
        new Funciones { Name = "Función 1", Url = "/Funcion1" },
        new Funciones { Name = "Función 2", Url = "/Funcion2" },
        // Agrega más funciones aquí
    };

        [HttpGet]
        public JsonResult Search(string query)
        {
            var results = Functions
                .Where(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Json(results);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Cedula, string contraseña)
        {
            if (Cedula != "" && contraseña != "")
            {
                HttpContext.Session.SetString("Username", Cedula);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Error = "Ingrese datos validos";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session?.Clear();
            return RedirectToAction("Login");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
