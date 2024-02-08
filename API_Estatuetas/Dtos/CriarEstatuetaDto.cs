using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace API_Estatuetas.Dtos
{
    public class CriarEstatuetaDto
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public List<string> Categorias { get; set; } 
        public List<IFormFile> Fotos { get; set; }
    }

}
