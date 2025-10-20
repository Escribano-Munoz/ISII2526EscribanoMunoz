using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HerramientasController> _logger;

        public HerramientasController(ApplicationDbContext context, ILogger<HerramientasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Herramienta>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetHerramientasParaReparar()
        {
            IList<Herramienta> herramientas = await _context.Herramienta
                .ToListAsync();
            return Ok(herramientas);
        }


    }
}
