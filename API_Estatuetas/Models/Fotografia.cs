using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Estatuetas.Models
{
    public class Fotografia
    {
        public int Id { get; set; }

        ///<summary>
        ///Nome do documento com a fotografia da pessoa
        /// </summary> 
        public string NomeFicheiro { get; set; }

        ///<summary>
        ///Data em que a fotografia foi tirada
        /// </summary>

        public DateTime Data { get; set; }

        /**********************************************************/

        ///<summary>
        ///FK para identificar a Fotografia da Estatueta que pertence
        /// </summary>
        [ForeignKey(nameof(Estatueta))]

        public int? EstatuetaFK { get; set; }

        public Estatueta Estatueta { get; set; }

    }
}
