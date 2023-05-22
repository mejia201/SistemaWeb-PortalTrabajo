using Microsoft.EntityFrameworkCore;
using SistemaWeb_PortalTrabajo.Models;

namespace SistemaWeb_PortalTrabajo.Context
{
    public class portalTrabajoDbContext: DbContext
    {

        public portalTrabajoDbContext(DbContextOptions options) : base(options) { 
        
        }


        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Trabajo> Trabajo { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Curriculum> Curriculum { get; set; }
        public DbSet<Candidato> Candidato { get; set; }


    }
}
