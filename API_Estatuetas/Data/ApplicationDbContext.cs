using API_Estatuetas.Data.Map;
using API_Estatuetas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Estatuetas.Data
{
    /// <summary>
    /// esta classe representa a Base de Dados do nosso projeto
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /* *********************************************
         * Criação das Tabelas
         * ********************************************* */
        public DbSet<Estatueta> Estatuetas { get; set; }
        public DbSet<Fotografia> Fotografia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EstatuetasMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
