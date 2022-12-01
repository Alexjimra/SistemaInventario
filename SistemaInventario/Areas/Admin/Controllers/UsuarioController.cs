using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Data;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                usuario.Rol = roles.FirstOrDefault(u => u.Id == usuario.Id).Name;
            }

            return Json(new { data = usuarioLista });
        }

        #endregion
    }
}
