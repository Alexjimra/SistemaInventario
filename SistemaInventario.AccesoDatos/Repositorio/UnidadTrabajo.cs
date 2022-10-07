using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly SistemaInventarioContext _db;
        public IBodegaRepositorio Bodega { get; private set; }

        public UnidadTrabajo(SistemaInventarioContext db)
        {
            _db = db;
            Bodega = new BodegaRepositorio(_db); // Inicializamos

        }

        public void Guardar()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
