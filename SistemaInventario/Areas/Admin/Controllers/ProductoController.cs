using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.DiaSymReader;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Packaging.Signing;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Rol_Admin + "," + DS.Rol_Inventario)]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostHenvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostHenvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostHenvironment = hostHenvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                }),

                MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                }),

                PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select(p => new SelectListItem
                {
                    Text = p.Descripcion,
                    Value = p.Id.ToString()
                })
            };
               

            if (id == null)
            {
                // Creamos el registro
                return View(productoVM);
            }

            // Actualizamos el registro
            productoVM.Producto = _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
            
            if (productoVM.Producto == null)
            {
                return NotFound();
            }

            return View(productoVM);
         }

        [HttpPost]
        [ValidateAntiForgeryToken]  // Evita que entren e nuestra aplicación y envíen datos
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                // Cargar imagen
                string webRootPath = _hostHenvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\productos");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (productoVM.Producto.ImagenUrl != null)
                    {
                        // Esto es para editar, necesitamos borrar la imagen anterior
                        var imagenPath = Path.Combine(webRootPath, productoVM.Producto.ImagenUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }       
                   
                    productoVM.Producto.ImagenUrl = @"\imagenes\productos\" + fileName + extension;

                } else {
                    // Si es Update, no cambia la imagen
                    if (productoVM.Producto.Id != 0)
                    {
                        Producto productoDB = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                        productoVM.Producto.ImagenUrl = productoDB.ImagenUrl;
                    }
                }


                if (productoVM.Producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                } else
                {
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));

            } else {
                productoVM.CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });

                productoVM.MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                });

                productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select(p => new SelectListItem
                {
                    Text = p.Descripcion,
                    Value = p.Id.ToString()
                });

                if (productoVM.Producto.Id != 0)
                {
                    productoVM.Producto = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                }
            }

            return View(productoVM.Producto);
        }
        
        
        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            // "Categoria,Marca" - - Sin espacios entre las comas
            var todos = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productoDB = _unidadTrabajo.Producto.Obtener(id);
            if (productoDB == null)
            {
                return Json(new { success = false, message = "Error al borrar" });
            }

            // Eliminamos la imágen
            string webRootPath = _hostHenvironment.WebRootPath;
            var imagenPath = Path.Combine(webRootPath, productoDB.ImagenUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagenPath))
            {
                System.IO.File.Delete(imagenPath);
            }


            _unidadTrabajo.Producto.Remover(productoDB); 
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }

        #endregion
    }
}
