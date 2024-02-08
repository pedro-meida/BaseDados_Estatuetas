using API_Estatuetas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Estatuetas.Data.Map
{
    public class EstatuetasMap : IEntityTypeConfiguration<Estatueta>
    {
        public void Configure(EntityTypeBuilder<Estatueta> builder)
        {
            // Configuração da chave primária
            builder.HasKey(x => x.EstatuetaID);

            // Configuração do título
            builder.Property(x => x.Titulo)
                .HasMaxLength(100)
                .IsRequired();

            // Configuração da descrição
            builder.Property(x => x.Descricao)
                .HasMaxLength(1000)
                .IsRequired();

            // Configuração do preço
            builder.Property(x => x.Preco)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Configuração da relação muitos-para-muitos com Categoria
            builder
                .HasMany(e => e.Categorias)
                .WithMany(c => c.Estatuetas);

            // Configuração da relação com ListaFotos
            builder.HasMany(e => e.ListaFotos)
                .WithOne(f => f.Estatueta);
        }
    }
}
