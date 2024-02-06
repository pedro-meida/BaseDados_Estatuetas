using API_Estatuetas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Estatuetas.Data.Map
{
    public class EstatuetasMap : IEntityTypeConfiguration<Estatueta>
    {
        public void Configure(EntityTypeBuilder<Estatueta> builder)
        {
            builder.HasKey(x => x.EstatuetaID);

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(70);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Preco)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasMany(e => e.ListaFotos)
                .WithOne()
                .HasForeignKey(f => f.EstatuetaFK)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}