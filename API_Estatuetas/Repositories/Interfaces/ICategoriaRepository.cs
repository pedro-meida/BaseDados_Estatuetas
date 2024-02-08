using System.Collections.Generic;
using System.Threading.Tasks;
using API_Estatuetas.Models;

namespace API_Estatuetas.Repositories.Interfaces
{
    /// <summary>
    /// Interface para o repositório de categorias.
    /// </summary>
    public interface ICategoriaRepository
    {
        /// <summary>
        /// Obtém todas as categorias.
        /// </summary>
        /// <returns>Uma lista de categorias.</returns>
        Task<List<Categoria>> GetAllCategorias();

        /// <summary>
        /// Obtém uma categoria pelo ID.
        /// </summary>
        /// <param name="id">O ID da categoria.</param>
        /// <returns>A categoria encontrada.</returns>
        Task<Categoria> GetCategoriaById(int id);

        /// <summary>
        /// Obtém uma categoria pelo ID.
        /// </summary>
        /// <param name="nome">O nome da categoria.</param>
        /// <returns>A categoria encontrada.</returns>
        Task<Categoria> GetCategoriaByNome(string nome);

        /// <summary>
        /// Adiciona uma nova categoria.
        /// </summary>
        /// <param name="categoria">A categoria a ser adicionada.</param>
        Task<Categoria> AddCategoria(Categoria categoria);

        /// <summary>
        /// Atualiza uma categoria existente.
        /// </summary>
        /// <param name="categoria">A categoria a ser atualizada.</param>
        Task<Categoria> UpdateCategoria(int id, string nome);

        /// <summary>
        /// Exclui uma categoria pelo ID.
        /// </summary>
        /// <param name="id">O ID da categoria a ser excluída.</param>
        Task<bool> DeleteCategoria(int id);
    }
}
