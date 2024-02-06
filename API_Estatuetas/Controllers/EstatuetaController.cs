using API_Estatuetas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Estatuetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstatuetaController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Estatueta>> GetAllEstatuetas()
        {
            return Ok();
        }
    }
}
