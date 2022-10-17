using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Data
{
    public partial class SistemaInventarioContext : DbContext
    {
        //public SistemaInventarioContext()
        //{
        //}

        public SistemaInventarioContext(DbContextOptions<SistemaInventarioContext> options)
            : base(options)
        {
        }

        public DbSet<Bodega> Bodega { get; set; } = null!;
        public DbSet<Categoria> Categoria { get; set; } 
        public DbSet<Marca> Marca { get; set; }
     
    }
}
