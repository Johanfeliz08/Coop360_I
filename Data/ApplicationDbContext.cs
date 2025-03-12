using Microsoft.EntityFrameworkCore;
using Coop360_I.Models;

namespace Coop360_I.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Definiciones de dbsets
        public DbSet<Usuario> Usuarios { get; set; }

    }
}
