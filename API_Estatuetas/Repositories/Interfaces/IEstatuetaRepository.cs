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
        Task<Estatueta> Add(Estatueta estatueta);

        /// <summary>
        /// Atualiza o título, descrição e preço de uma estatueta existente.
        /// </summary>
        /// <param name="id">ID da estatueta a ser atualizada.</param>
        /// <param name="titulo">Titulo da estatueta a ser atualizada</param>
        /// <param name="descricao">Descrição da estatueta a ser atualizada</param>
        /// <param name="preco">Preço da estatueta a ser atualizada</param>
        Task<Estatueta> UpdateDadosEstatueta(int id, string titulo, string descricao, decimal preco);

        /// <summary>
        /// Atualiza uma estatueta depois de se desassociar uma categoria.
        /// </summary>
        /// <param name="idEstatueta">ID da estatueta a ser atualizada.</param>
        /// <param name="categoriaIdARemover">ID da categoria a ser desassociada à estatueta.</param>
        Task<Estatueta> UpdateEstatuetaCategoria(int idEstatueta, int categoriaIdARemover);

        /// <summary>
        /// Exclui uma estatueta por ID.
        /// </summary>
        /// <param name="id">ID da estatueta a ser excluída.</param>
        Task<bool> DeleteEstatueta(int id);

        /// <summary>
        /// Adiciona uma nova foto à estatueta especificada.
        /// </summary>
        /// <param name="id">ID da estatueta.</param>
        /// <param name="foto">Foto a ser adicionada.</param>
        Task<Estatueta> AdicionarFoto(int id, IFormFile foto);

        Task<string> SalvarImagemLocal(IFormFile foto);

        /// <summary>
        /// Exclui uma foto da estatueta especificada.
        /// </summary>
        /// <param name="id">ID da estatueta.</param>
        /// <param name="fotoId">ID da foto a ser excluída.</param>
        Task<bool> ExcluirFoto(int id, int fotoId);
    }
}
