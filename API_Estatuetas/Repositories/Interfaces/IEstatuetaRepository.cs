using API_Estatuetas.Models;

namespace API_Estatuetas.Repositories.Interfaces
{
    public interface IEstatuetaRepository
    {
        Task<List<Estatueta>> GetAllEstatuetas();
        Task<Estatueta> GetEstatuetaById(int id);
        Task<Estatueta> Add(Estatueta estatueta,IFormFile imagemEstatueta);
        Task<Estatueta> Update(Estatueta estatueta,int id);
        Task<bool> Delete(int id);
    }
}
