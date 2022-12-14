using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DiaSymReader;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Rol_Admin)]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Bodega bodega = new Bodega();
            if (id == null)
            {
                // Creamos el registro
                return View(bodega);
            }

            // Actualizamos el registro
            bodega = _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            
            if (bodega == null)
            {
                return NotFound();
            }

            return View(bodega);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  // Evita que entren e nuestra aplicación y envíen datos
        public IActionResult Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    _unidadTrabajo.Bodega.Agregar(bodega);
                } else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            } else
            {
                return View(bodega);
            }
        }


        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var bodegaDB = _unidadTrabajo.Bodega.Obtener(id);
            if (bodegaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDB);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }

        #endregion
    }
}
