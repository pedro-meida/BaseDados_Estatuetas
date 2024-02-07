using System.ComponentModel.DataAnnotations;

namespace API_Estatuetas.Models
{
    public class Estatueta
    {
        public int EstatuetaID { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "O campo Preco deve ser um valor numérico válido.")]
        public decimal Preco { get; set; }
        public ICollection<Fotografia>? ListaFotos { get; set; }

        public Estatueta()
        {
            ListaFotos = new List<Fotografia>();
        }

    }
}
