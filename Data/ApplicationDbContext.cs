using Microsoft.EntityFrameworkCore;
using Coop360_I.Models;

namespace Coop360_I.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Definiciones de dbsets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Codeudor> Codeudores { get; set; }
        public DbSet<Region> Regiones { get; set; }
        public DbSet<Provincia> Provincias { get; set; }  
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Sector> Sectores { get; set; }
        public DbSet<Puesto> Puestos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<NivelAprobacion> NivelesAprobacion { get; set; }
        public DbSet<EntidadBancaria> EntidadesBancarias { get; set; }
        public DbSet<CategoriaPrestamo> CategoriaPrestamo { get; set; }

    }
}
