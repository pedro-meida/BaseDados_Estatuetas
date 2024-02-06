using API_Estatuetas.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Estatuetas.Repositories.Interfaces
{
    /// <summary>
    /// Interface para manipulação de estatuetas.
    /// </summary>
    public interface IEstatuetaRepository
    {
        /// <summary>
        /// Obtém todas as estatuetas.
        /// </summary>
        Task<List<Estatueta>> GetAllEstatuetas();

        /// <summary>
        /// Obtém uma estatueta por ID.
        /// </summary>
        /// <param name="id">ID da estatueta.</param>
        Task<Estatueta> GetEstatuetaById(int id);

        /// <summary>
        /// Adiciona uma nova estatueta.
        /// </summary>
        /// <param name="estatueta">Objeto Estatueta a ser adicionado.</param>
        /// <param name="imagemEstatueta">Imagem associada à estatueta.</param>
        Task<Estatueta> Add(Estatueta estatueta, IFormFile imagemEstatueta);

        /// <summary>
        /// Atualiza uma estatueta existente.
        /// </summary>
        /// <param name="estatueta">Objeto Estatueta atualizado.</param>
        /// <param name="id">ID da estatueta a ser atualizada.</param>
        /// <param name="novaImagem">Nova imagem associada à estatueta (opcional).</param>
        Task<Estatueta> Update(Estatueta estatueta, int id, IFormFile? novaImagem = null);

        /// <summary>
        /// Exclui uma estatueta por ID.
        /// </summary>
        /// <param name="id">ID da estatueta a ser excluída.</param>
        Task<bool> Delete(int id);
    }
}
