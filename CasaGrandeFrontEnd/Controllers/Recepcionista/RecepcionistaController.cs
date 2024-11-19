using Microsoft.AspNetCore.Mvc;

namespace ClinicaEspacioAbiertoFrontEnd.Controllers.Recepcionista
{
    public class RecepcionistaController : Controller
    {
        // GET: RecepcionistaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RecepcionistaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RecepcionistaController/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: RecepcionistaController/Create
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

        // GET: RecepcionistaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecepcionistaController/Edit/5
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

        // GET: RecepcionistaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecepcionistaController/Delete/5
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


        //FUNCIONALIDADES DE RECEPCIONISTA:

        //VER HISTORIAL CLINICO
        //SUBIR DOCUMENTOS EN SU HISTORIA

        //PASAR ASISTENCIA DE UNA CONSULTA
        //CANCELAR CONSULTA(SOLO LA DE ESE DIA)
        //CANCELAR CONSULTA Y TODAS LAS SIGUIENTES
        //REAGENDAR CONSULTA
        //PAGAR CONSULTA
        //NO SUPERPONER CONSULTAS



    }
}
