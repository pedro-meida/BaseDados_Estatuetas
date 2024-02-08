using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Estatuetas.Data;
using API_Estatuetas.Models;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Estatuetas.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetAllCategorias()
        {
            return await _context.Categorias
                         .Include(c => c.Estatuetas) // Inclui as estatuetas associadas
                         .ToListAsync();
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            return await _context.Categorias
                        .Include(c => c.Estatuetas) // Inclui as estatuetas associadas
                        .FirstOrDefaultAsync(c => c.CategoriaId == id);
        }

        public async Task<Categoria> GetCategoriaByNome(string nome)
        {
            return await _context.Categorias
                        .Include(c => c.Estatuetas) // Inclui as estatuetas associadas
                        .FirstOrDefaultAsync(c => c.Nome == nome);
        }

        public async Task<Categoria> AddCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria> UpdateCategoria(int id, string nome)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return null;
            }

            categoria.Nome = nome;
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return false;
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
