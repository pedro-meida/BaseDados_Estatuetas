using API_Estatuetas.Models;
using API_Estatuetas.Repositories;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_Estatuetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly IEstatuetaRepository _estatuetaRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(IEstatuetaRepository estatuetaRepository,ICategoriaRepository categoriaRepository, ILogger<CategoriaController> logger)
        {
            _estatuetaRepository = estatuetaRepository;
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        // Obtém todas as categorias
        [HttpGet]
        public async Task<ActionResult<string>> GetAllCategorias()
        {
            try
            {
                List<Categoria> categorias = await _categoriaRepository.GetAllCategorias();
                // Serializa os objetos sem incluir referências circulares
                string json = SerializeObjectWithoutReferenceLoopHandling(categorias);
                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter categorias");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter categorias: {ex.Message}");
            }
        }

        // Obtém uma categoria por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetCategoriaById(int id)
        {
            try
            {
                Categoria categoria = await _categoriaRepository.GetCategoriaById(id);
                if (categoria == null)
                    return NotFound("Categoria não encontrada");
                // Serializa o objeto sem incluir referências circulares
                string json = SerializeObjectWithoutReferenceLoopHandling(categoria);
                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter categoria");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter categoria: {ex.Message}");
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
