using API_Estatuetas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Estatuetas.Data.Map
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(x => x.CategoriaId);

            builder.Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasMany(c => c.Estatuetas)
                .WithMany(e => e.Categorias);
        }
    }
}
