using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Utilidades;
using System.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Rol_Admin)]
    public class UsuarioController : Controller
    {
        private readonly SistemaInventarioContext _db;

        public UsuarioController(SistemaInventarioContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var usuarioLista = _db.UsuarioAplicacion.ToList();
            var userRol = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var usuario in usuarioLista)
            {
                var rolId = userRol.FirstOrDefault(u => u.UserId == usuario.Id).RoleId;
                usuario.Rol = roles.FirstOrDefault(u => u.Id == rolId).Name;
            }

            return Json(new { data = usuarioLista });
        }

        [HttpPost]
        public IActionResult BloquearDesbloquear([FromBody] string id)
        {
            var usuario = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Error de usuario" });
            }

            if (usuario.LockoutEnd != null && usuario.LockoutEnd > DateTime.Now)
            {
                // Ususrio bloqueado
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);

            }
            _db.SaveChanges();
            return Json(new { success = true, Message = "Operación exitosa" });
        }
        #endregion
    }
}
