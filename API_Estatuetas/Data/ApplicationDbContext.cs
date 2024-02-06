using API_Estatuetas.Data.Map;
using API_Estatuetas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_Estatuetas.Data
{
    /// <summary>
    /// Representa a Base de Dados do projeto.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Construtor da classe ApplicationDbContext.
        /// </summary>
        /// <param name="options">Opções de configuração do DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /* *********************************************
         * Criação das Tabelas
         * ********************************************* */

        /// <summary>
        /// Conjunto de dados para a entidade Estatueta.
        /// </summary>
        public DbSet<Estatueta> Estatuetas { get; set; }

        /// <summary>
        /// Conjunto de dados para a entidade Fotografia.
        /// </summary>
        public DbSet<Fotografia> Fotografias { get; set; }

        /// <summary>
        /// Configurações adicionais do modelo de dados.
        /// </summary>
        /// <param name="modelBuilder">Construtor do modelo de dados.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica as configurações específicas do mapeamento da entidade Estatueta
            modelBuilder.ApplyConfiguration(new EstatuetasMap());
            // Chama a implementação base do método OnModelCreating
            base.OnModelCreating(modelBuilder);
        }
    }
}
