using API_Estatuetas.Dtos;
using API_Estatuetas.Models;
using API_Estatuetas.Repositories;
using API_Estatuetas.Repositories.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Estatuetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstatuetaController : ControllerBase
    {
        private readonly IEstatuetaRepository _estatuetaRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EstatuetaController> _logger;

        public EstatuetaController(IEstatuetaRepository estatuetaRepository, ICategoriaRepository categoriaRepository, IWebHostEnvironment webHostEnvironment, ILogger<EstatuetaController> logger)
        {
            _estatuetaRepository = estatuetaRepository;
            _categoriaRepository = categoriaRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // Obtém todas as estatuetas
        [HttpGet]
        public async Task<ActionResult<string>> GetAllEstatuetas()
        {
            try
            {
                List<Estatueta> estatuetas = await _estatuetaRepository.GetAllEstatuetas();
                // Serializa os objetos sem incluir referências circulares
                string json = SerializeObjectWithoutReferenceLoopHandling(estatuetas);
                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatuetas");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter estatuetas: {ex.Message}");
            }
        }

        // Obtém uma estatueta por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetEstatuetaById(int id)
        {
            try
            {
                Estatueta estatueta = await _estatuetaRepository.GetEstatuetaById(id);
                if (estatueta == null)
                    return NotFound("Estatueta não encontrada");
                // Serializa o objeto sem incluir referências circulares
                string json = SerializeObjectWithoutReferenceLoopHandling(estatueta);
                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatueta");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter estatueta: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarEstatuetaComFotos([FromForm] CriarEstatuetaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaEstatueta = new Estatueta
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Preco = dto.Preco
            };

            try
            {
                foreach (var categoriaNome in dto.Categorias)
                {
                    Categoria categoria;
                    // Verifica se a categoria já existe no banco de dados pelo nome
                    categoria = await _categoriaRepository.GetCategoriaByNome(categoriaNome);
                    if (categoria == null)
                    {
                        // Se a categoria não existir, cria uma nova categoria com o nome fornecido
                        categoria = new Categoria { Nome = categoriaNome };
                        await _categoriaRepository.AddCategoria(categoria);
                    }

                    // Associa a estatueta à categoria
                    novaEstatueta.Categorias.Add(categoria);
                }

                foreach (var foto in dto.Fotos)
                {
                    // Adiciona a nova foto à estatueta
                    string nomeFoto = await _estatuetaRepository.SalvarImagemLocal(foto);

                    novaEstatueta.ListaFotos.Add(new Fotografia
                    {
                        NomeFicheiro = nomeFoto,
                        Data = DateTime.Now
                    });
                }

                await _estatuetaRepository.Add(novaEstatueta);

                return Ok(novaEstatueta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar estatueta");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao adicionar estatueta: {ex.Message}");
            }
        }


        // Adiciona uma nova foto a uma estatueta existente
        [HttpPost("{idEstatueta}/fotos")]
        public async Task<IActionResult> AdicionarFoto(int idEstatueta, IFormFile foto)
        {
            try
            {
                Estatueta estatueta = await _estatuetaRepository.AdicionarFoto(idEstatueta, foto);

                if (estatueta == null)
                {
                    return NotFound("Estatueta não encontrada");
                }

                return Ok(estatueta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar foto");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao adicionar foto: {ex.Message}");
            }
        }

        // Exclui uma foto de uma estatueta
        [HttpDelete("{idEstatueta}/fotos/{idFoto}")]
        public async Task<IActionResult> ExcluirFoto(int idEstatueta, int idFoto)
        {
            try
            {
                bool fotoExcluida = await _estatuetaRepository.ExcluirFoto(idEstatueta, idFoto);

                if (!fotoExcluida)
                {
                    return NotFound("Foto não encontrada");
                }

                return Ok("Foto excluída com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir foto");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir foto: {ex.Message}");
            }
        }

        // Atualiza os dados de uma estatueta existente
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarEstatueta(int id, string titulo, string descricao, decimal preco)
        {
            try
            {
                Estatueta estatuetaAtualizada = await _estatuetaRepository.UpdateDadosEstatueta(id, titulo, descricao, preco);
                if (estatuetaAtualizada == null)
                {
                    return NotFound("Estatueta não encontrada");
                }
                return Ok(estatuetaAtualizada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar estatueta");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar estatueta: {ex.Message}");
            }
        }

        // Exclui uma estatueta por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirEstatueta(int id)
        {
            try
            {
                bool estatuetaExcluida = await _estatuetaRepository.DeleteEstatueta(id);

                if (!estatuetaExcluida)
                {
                    return NotFound("Estatueta não encontrada");
                }

                return Ok("Estatueta excluída com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir estatueta");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir estatueta: {ex.Message}");
            }
        }

        // Remove a associação entre uma categoria e uma estatueta
        [HttpDelete("{idEstatueta}/categorias/{idCategoria}")]
        public async Task<IActionResult> RemoverCategoriaDaEstatueta(int idEstatueta, int idCategoria)
        {
            try
            {
                // Verifica se a estatueta existe
                Estatueta estatueta = await _estatuetaRepository.GetEstatuetaById(idEstatueta);
                if (estatueta == null)
                {
                    return NotFound("Estatueta não encontrada");
                }

                // Verifica se a categoria existe
                Categoria categoria = await _categoriaRepository.GetCategoriaById(idCategoria);
                if (categoria == null)
                {
                    return NotFound("Categoria não encontrada");
                }

                // Verifica se a estatueta está associada à categoria
                if (!estatueta.Categorias.Any(c => c.CategoriaId == idCategoria))
                {
                    return BadRequest("A estatueta não está associada a esta categoria");
                }

                // Remove a associação entre a categoria e a estatueta
                estatueta.Categorias.Remove(categoria);

                // Atualiza a estatueta no banco de dados
                await _estatuetaRepository.UpdateEstatuetaCategoria(estatueta.EstatuetaID, categoria.CategoriaId);

                return Ok("Categoria removida da estatueta com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover categoria da estatueta");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao remover categoria da estatueta: {ex.Message}");
            }
        }

        // Método auxiliar para serialização com configurações personalizadas
        private string SerializeObjectWithoutReferenceLoopHandling<T>(T obj)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
