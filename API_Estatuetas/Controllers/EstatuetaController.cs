using API_Estatuetas.Models;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Injeta o repositório de estatuetas no controlador
        public EstatuetaController(IEstatuetaRepository estatuetaRepository)
        {
            _estatuetaRepository = estatuetaRepository;
        }

        // Obtém todas as estatuetas
        [HttpGet]
        public async Task<ActionResult<List<Estatueta>>> GetAllEstatuetas()
        {
            try
            {
                // Tenta obter todas as estatuetas do repositório
                List<Estatueta> estatuetas = await _estatuetaRepository.GetAllEstatuetas();
                // Retorna as estatuetas obtidas com sucesso
                return Ok(estatuetas);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um status de erro interno do servidor junto com uma mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter estatuetas: {ex.Message}");
            }
        }

        // Obtém uma estatueta por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Estatueta>> GetEstatuetaById(int id)
        {
            try
            {
                // Tenta obter a estatueta com o ID fornecido do repositório
                Estatueta estatueta = await _estatuetaRepository.GetEstatuetaById(id);
                // Se a estatueta não for encontrada, retorna um status de não encontrado
                if (estatueta == null)
                    return NotFound("Estatueta não encontrada");

                // Retorna a estatueta obtida com sucesso
                return Ok(estatueta);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um status de erro interno do servidor junto com uma mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter estatueta: {ex.Message}");
            }
        }

        // Registra uma nova estatueta
        [HttpPost]
        public async Task<ActionResult<Estatueta>> Register([FromForm] Estatueta estatueta, IFormFile imagemEstatueta)
        {
            try
            {
                // Verifica se a estatueta ou a imagem não foram fornecidas
                if (estatueta == null || imagemEstatueta == null)
                    return BadRequest("Estatueta ou imagem não fornecida");

                // Adiciona a estatueta com a imagem fornecida ao repositório
                Estatueta novaEstatueta = await _estatuetaRepository.Add(estatueta, imagemEstatueta);
                // Retorna um status de criação bem-sucedida juntamente com a nova estatueta criada
                return CreatedAtAction(nameof(GetEstatuetaById), new { id = novaEstatueta.EstatuetaID }, novaEstatueta);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um status de erro interno do servidor junto com uma mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao registrar estatueta: {ex.Message}");
            }
        }
    }
}
