namespace API_Estatuetas.Models
{
    public class Estatueta
    {
        public int EstatuetaID { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataCriacao { get; set; }
        public ICollection<Fotografia>? ListaFotos { get; set; }
    }
}
