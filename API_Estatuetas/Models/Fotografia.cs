using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_Estatuetas.Models
{
    public class Fotografia
    {
        public int Id { get; set; }

        ///<summary>
        /// Nome do documento com a fotografia da pessoa
        /// </summary> 
        public string NomeFicheiro { get; set; }

        ///<summary>
        /// Data em que a fotografia foi tirada
        /// </summary>
        public DateTime Data { get; set; }

        /**********************************************************/

        ///<summary>
        /// Navegação para identificar a Estatueta a que a fotografia pertence
        /// </summary>
        [JsonIgnore]
        public Estatueta Estatueta { get; set; }
    }
}
