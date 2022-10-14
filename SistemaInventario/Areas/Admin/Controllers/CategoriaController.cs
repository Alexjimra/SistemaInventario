using Microsoft.AspNetCore.Mvc;
using Microsoft.DiaSymReader;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Categoria categoria = new Categoria();
            if (id == null)
            {
                // Creamos el registro
                return View(categoria);
            }

            // Actualizamos el registro
            categoria = _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  // Evita que entren e nuestra aplicación y envíen datos
        public IActionResult Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)
                {
                    _unidadTrabajo.Categoria.Agregar(categoria);
                } else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            } else
            {
                return View(categoria);
            }
        }


        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoriaDB = _unidadTrabajo.Categoria.Obtener(id);
            if (categoriaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDB); 
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        #endregion
    }
}
