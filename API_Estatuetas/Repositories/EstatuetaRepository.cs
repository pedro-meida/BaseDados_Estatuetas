﻿using API_Estatuetas.Data;
using API_Estatuetas.Models;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API_Estatuetas.Repositories
{
    public class EstatuetaRepository : IEstatuetaRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EstatuetaRepository(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // Obtém todas as estatuetas
        public async Task<List<Estatueta>> GetAllEstatuetas()
        {
            return await _dbContext.Estatuetas
                         .Include(e => e.ListaFotos)
                         .Include(e => e.Categorias)  // Inclui as categorias associadas
                         .ToListAsync();
        }

        // Obtém uma estatueta por ID
        public async Task<Estatueta> GetEstatuetaById(int id)
        {
            return await _dbContext.Estatuetas
                        .Include(e => e.ListaFotos)
                        .Include(e => e.Categorias)  // Inclui as categorias associadas
                        .FirstOrDefaultAsync(e => e.EstatuetaID == id);
        }


        // Adiciona uma nova estatueta
        public async Task<Estatueta> Add(Estatueta estatueta)
        {
            _dbContext.Estatuetas.Add(estatueta);
            await _dbContext.SaveChangesAsync();

            return estatueta;
        }

        // Atualiza os dados de uma estatueta existente
        public async Task<Estatueta> UpdateDadosEstatueta(int id, string titulo, string descricao, decimal preco)
        {
            Estatueta estatuetabyId = await GetEstatuetaById(id);

            if (estatuetabyId == null)
            {
                throw new Exception($"Estatueta para o Id: {id} não foi encontrado na base de dados.");
            }

            estatuetabyId.Titulo = titulo;
            estatuetabyId.Descricao = descricao;
            estatuetabyId.Preco = preco;

            _dbContext.Estatuetas.Update(estatuetabyId);
            await _dbContext.SaveChangesAsync();

            return estatuetabyId;
        }

        // Atualiza a categoria de uma estatueta existente
        public async Task<Estatueta> UpdateEstatuetaCategoria(int idEstatueta, int categoriaIdARemover)
        {
            Estatueta estatuetabyId = await GetEstatuetaById(idEstatueta);

            if (estatuetabyId == null)
            {
                throw new Exception($"Estatueta para o Id: {idEstatueta} não foi encontrado na base de dados.");
            }

            // Remove a categoria especificada
            Categoria categoriaToRemove = estatuetabyId.Categorias.FirstOrDefault(c => c.CategoriaId == categoriaIdARemover);
            if (categoriaToRemove != null)
            {
                estatuetabyId.Categorias.Remove(categoriaToRemove);
            }

            // Atualiza a estatueta no banco de dados
            _dbContext.Estatuetas.Update(estatuetabyId);
            await _dbContext.SaveChangesAsync();

            // Verifica se a categoria não está mais associada a nenhuma estatueta
            Categoria categoria = await _dbContext.Categorias.Include(c => c.Estatuetas).FirstOrDefaultAsync(c => c.CategoriaId == categoriaIdARemover);
            if (categoria != null && categoria.Estatuetas.Count == 0)
            {
                // Remove a categoria do banco de dados se não estiver mais associada a nenhuma estatueta
                _dbContext.Categorias.Remove(categoria);
                await _dbContext.SaveChangesAsync();
            }

            return estatuetabyId;
        }

        // Adiciona uma nova foto a uma estatueta existente
        public async Task<Estatueta> AdicionarFoto(int idEstatueta, IFormFile foto)
        {
            // Obtém a estatueta pelo ID fornecido
            Estatueta estatueta = await _dbContext.Estatuetas.Include(e => e.ListaFotos)
                                                             .FirstOrDefaultAsync(e => e.EstatuetaID == idEstatueta);

            if (estatueta == null)
            {
                // Retorna nulo se a estatueta não for encontrada
                return null;
            }

            // Adiciona a nova foto à estatueta
            string nomeFoto = await SalvarImagemLocal(foto);

            estatueta.ListaFotos.Add(new Fotografia
            {
                NomeFicheiro = nomeFoto,
                Data = DateTime.Now
            });

            // Salva as alterações no banco de dados
            await _dbContext.SaveChangesAsync();

            return estatueta;
        }

        // Exclui uma foto de uma estatueta
        public async Task<bool> ExcluirFoto(int idEstatueta, int idFoto)
        {
            // Obtém a estatueta pelo ID fornecido
            Estatueta estatueta = await _dbContext.Estatuetas.Include(e => e.ListaFotos)
                                                             .FirstOrDefaultAsync(e => e.EstatuetaID == idEstatueta);

            if (estatueta == null)
            {
                // Retorna falso se a estatueta não for encontrada
                return false;
            }

            // Obtém a foto pelo ID fornecido
            Fotografia foto = estatueta.ListaFotos.FirstOrDefault(f => f.Id == idFoto);

            if (foto == null)
            {
                // Retorna falso se a foto não for encontrada
                return false;
            }

            // Remove a foto da lista de fotos da estatueta
            estatueta.ListaFotos.Remove(foto);

            // Remove a foto do sistema de arquivos local
            RemoverImagemLocal(foto.NomeFicheiro);

            // Salva as alterações no banco de dados
            await _dbContext.SaveChangesAsync();

            return true;
        }

        // Deleta uma estatueta, suas fotos associadas e a categoria se não estiver associada a mais nenhuma estatueta
        public async Task<bool> DeleteEstatueta(int id)
        {
            Estatueta estatuetabyId = await GetEstatuetaById(id);

            if (estatuetabyId == null)
            {
                throw new Exception($"Estatueta para o Id: {id} não foi encontrado na base de dados.");
            }

            // Remover imagens associadas à estatueta do sistema de arquivos local
            RemoverImagensLocais(estatuetabyId);

            _dbContext.Estatuetas.Remove(estatuetabyId);

            // Salva as categorias associadas à estatueta antes de removê-las
            var categoriasParaRemover = estatuetabyId.Categorias.ToList();

            await _dbContext.SaveChangesAsync();

            // Verifica se cada categoria está associada a mais estatuetas além da que foi excluída
            foreach (var categoria in categoriasParaRemover)
            {
                bool categoriaTemOutrasEstatuetas = _dbContext.Estatuetas.Any(e => e.Categorias.Any(c => c.CategoriaId == categoria.CategoriaId && e.EstatuetaID != id));
                if (!categoriaTemOutrasEstatuetas)
                {
                    // Se a categoria não estiver associada a mais nenhuma estatueta, exclua-a
                    _dbContext.Categorias.Remove(categoria);
                }
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }


        // Remove imagens associadas à estatueta do sistema de arquivos local
        private void RemoverImagensLocais(Estatueta estatueta)
        {
            if (estatueta.ListaFotos != null && estatueta.ListaFotos.Any())
            {
                foreach (var foto in estatueta.ListaFotos)
                {
                    string nomeLocalizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                    string nomeDoFicheiro = Path.Combine(nomeLocalizacaoImagem, foto.NomeFicheiro);

                    if (File.Exists(nomeDoFicheiro))
                    {
                        File.Delete(nomeDoFicheiro);
                    }
                }

                estatueta.ListaFotos.Clear();
            }
        }

        // Salva uma imagem no sistema de arquivos local e retorna o nome do arquivo
        public async Task<string> SalvarImagemLocal(IFormFile foto)
        {
            string nomeArquivo = $"{Guid.NewGuid()}_{foto.FileName}";
            string pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
            string caminhoArquivo = Path.Combine(pastaImagens, nomeArquivo);

            // Verifica se o diretório "imagens" existe e, se não, cria-o
            if (!Directory.Exists(pastaImagens))
            {
                Directory.CreateDirectory(pastaImagens);
            }

            // Salva a imagem no diretório
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            return nomeArquivo;
        }


        // Remove uma imagem do sistema de arquivos local
        private void RemoverImagemLocal(string nomeArquivo)
        {
            string caminhoArquivo = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", nomeArquivo);

            if (File.Exists(caminhoArquivo))
            {
                File.Delete(caminhoArquivo);
            }
        }
    }
}
