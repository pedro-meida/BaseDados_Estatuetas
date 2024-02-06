using API_Estatuetas.Models;
using API_Estatuetas.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Estatuetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstatuetaController : ControllerBase
    {
        private readonly IEstatuetaRepository _estatuetaRepository;
        public EstatuetaController(IEstatuetaRepository estatuetaRepository)
        {
            _estatuetaRepository = estatuetaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Estatueta>>> GetAllEstatuetas()
        {
            List<Estatueta> estatuetas = await _estatuetaRepository.GetAllEstatuetas();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Estatueta>>> GetEstatuetaById(int id)
        {
            Estatueta estatueta = await _estatuetaRepository.GetEstatuetaById(id);
            return Ok(estatueta);
        }

        [HttpPost]
        public async Task<ActionResult<Estatueta>> Register([FromBody] Estatueta estatueta, IFormFile imagemEstatueta)
        {
            Estatueta Estatueta = await _estatuetaRepository.Add(estatueta, imagemEstatueta);
            return Ok(Estatueta);
        }
    }
}
