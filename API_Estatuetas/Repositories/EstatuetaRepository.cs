using API_Estatuetas.Data;
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

        public async Task<List<Estatueta>> GetAllEstatuetas()
        {
            return await _dbContext.Estatuetas.ToListAsync();
        }

        public async Task<Estatueta> GetEstatuetaById(int id)
        {
            return await _dbContext.Estatuetas.FirstOrDefaultAsync(x => x.EstatuetaID == id);
        }
        public async Task<Estatueta> Add(Estatueta estatueta, IFormFile imagemEstatueta)
        {
            //variáveis auxiliares
            string nomeFoto = "";
            bool existeFoto = false;

            if (imagemEstatueta.ContentType != "image/jpg" &&
                    imagemEstatueta.ContentType != "image/png" && imagemEstatueta.ContentType != "image/jpeg")
            {
                estatueta.ListaFotos
                    .Add(new Fotografia
                    {
                        Data = DateTime.Now,
                        NomeFicheiro = "ImagemDefaultEstatueta.jpeg"
                    });
            }
            else
            {
                Guid g = Guid.NewGuid();
                nomeFoto = g.ToString();
                string extensaoNomeFoto = Path.GetExtension(imagemEstatueta.FileName).ToLower();
                nomeFoto += extensaoNomeFoto;

                estatueta.ListaFotos
                        .Add(new Fotografia
                        {
                            Data = DateTime.Now,
                            NomeFicheiro = nomeFoto
                        });
                existeFoto = true;
            }

            _dbContext.Estatuetas.Add(estatueta);
            await _dbContext.SaveChangesAsync();
                if (existeFoto)
                {
                    string nomeLocalizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

                    if (!Directory.Exists(nomeLocalizacaoImagem))
                    {
                        Directory.CreateDirectory(nomeLocalizacaoImagem);
                    }

                    string nomeDoFicheiro = Path.Combine(nomeLocalizacaoImagem, nomeFoto);

                    using var stream = new FileStream(nomeDoFicheiro, FileMode.Create);
                    await imagemEstatueta.CopyToAsync(stream);
                }

            return estatueta;
        }
        public async Task<Estatueta> Update(Estatueta estatueta, int id, List<IFormFile> novasImagens = null)
        {
            Estatueta estatuetabyId = await GetEstatuetaById(id);

            if (estatuetabyId == null)
            {
                throw new Exception($"Estatueta para o Id: {id} não foi encontrado na base de dados.");
            }

            estatuetabyId.Titulo = estatueta.Titulo;
            estatuetabyId.Descricao = estatueta.Descricao;
            estatuetabyId.Preco = estatueta.Preco;

            // Atualiza as imagens
            AtualizarImagens(estatuetabyId, novasImagens);

            _dbContext.Estatuetas.Update(estatuetabyId);
            await _dbContext.SaveChangesAsync();

            return estatuetabyId;
        }

        private void AtualizarImagens(Estatueta estatueta, List<IFormFile> novasImagens)
        {
            if (novasImagens != null && novasImagens.Any())
            {
                // Remove imagens existentes associadas à estatueta
                RemoverImagensLocais(estatueta);

                estatueta.ListaFotos = new List<Fotografia>();

                foreach (var novaImagem in novasImagens)
                {
                    Guid g = Guid.NewGuid();
                    string nomeFoto = g.ToString();
                    string extensaoNomeFoto = Path.GetExtension(novaImagem.FileName).ToLower();
                    nomeFoto += extensaoNomeFoto;

                    estatueta.ListaFotos.Add(new Fotografia
                    {
                        Data = DateTime.Now,
                        NomeFicheiro = nomeFoto
                    });

                    // Salva a nova imagem no sistema de arquivos local
                    SalvarImagemLocal(nomeFoto, novaImagem);
                }
            }
        }

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

        private void SalvarImagemLocal(string nomeFoto, IFormFile novaImagem)
        {
            string nomeLocalizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

            if (!Directory.Exists(nomeLocalizacaoImagem))
            {
                Directory.CreateDirectory(nomeLocalizacaoImagem);
            }

            string nomeDoFicheiro = Path.Combine(nomeLocalizacaoImagem, nomeFoto);

            using var stream = new FileStream(nomeDoFicheiro, FileMode.Create);
            novaImagem.CopyTo(stream);
        }

        public async Task<bool> Delete(int id)
        {
            Estatueta estatuetabyId = await GetEstatuetaById(id);

            if (estatuetabyId == null)
            {
                throw new Exception($"Estatueta para o Id: {id} não foi encontrado na base de dados.");
            }

            // Remova a imagem associada à estatueta do sistema de arquivos local
            RemoverImagemLocal(estatuetabyId);

            _dbContext.Estatuetas.Remove(estatuetabyId);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private void RemoverImagemLocal(Estatueta estatueta)
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
            }
        }
    }
}