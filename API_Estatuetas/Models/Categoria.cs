namespace API_Estatuetas.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; }

        public ICollection<Estatueta> Estatuetas { get; set; }

        public Categoria()
        {
            Estatuetas = new List<Estatueta>();
        }
    }
}
