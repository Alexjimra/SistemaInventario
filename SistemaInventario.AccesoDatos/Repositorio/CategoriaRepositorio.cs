using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        private readonly SistemaInventarioContext _db;

        public CategoriaRepositorio(SistemaInventarioContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Categoria categoria)
        {
            var CategoriaDB = _db.Categoria.FirstOrDefault(b => b.Id == categoria.Id);
            if (CategoriaDB != null)
            {
                CategoriaDB.Nombre = categoria.Nombre;
                CategoriaDB.Estado = categoria.Estado;

            }
        }
    }
}
