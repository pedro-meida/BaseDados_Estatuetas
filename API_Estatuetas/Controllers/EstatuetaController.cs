using API_Estatuetas.Dtos;
using API_Estatuetas.Models;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EstatuetaController(IEstatuetaRepository estatuetaRepository, IWebHostEnvironment webHostEnvironment)
        {
            _estatuetaRepository = estatuetaRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Obtém todas as estatuetas
        [HttpGet]
        public async Task<ActionResult<List<Estatueta>>> GetAllEstatuetas()
        {
            try
            {
                List<Estatueta> estatuetas = await _estatuetaRepository.GetAllEstatuetas();
                return Ok(estatuetas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter estatuetas: {ex.Message}");
            }
        }

        // Obtém uma estatueta por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Estatueta>> GetEstatuetaById(int id)
        {
            try
            {
                Estatueta estatueta = await _estatuetaRepository.GetEstatuetaById(id);
                if (estatueta == null)
                    return NotFound("Estatueta não encontrada");
                return Ok(estatueta);
            }
            catch (Exception ex)
            {
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir foto: {ex.Message}");
            }
        }

        // Atualiza os dados de uma estatueta existente
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarEstatueta(int id, string titulo, string descricao , decimal preco)
        {
            try
            {
                Estatueta estatuetaAtualizada = await _estatuetaRepository.UpdateDadosEstatueta(id , titulo, descricao, preco);
                if (estatuetaAtualizada == null)
                {
                    return NotFound("Estatueta não encontrada");
                }
                return Ok(estatuetaAtualizada);
            }
            catch (Exception ex)
            {
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
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir estatueta: {ex.Message}");
            }
        }
    }
}
