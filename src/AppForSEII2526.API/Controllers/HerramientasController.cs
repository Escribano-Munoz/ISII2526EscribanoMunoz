using AppForSEII2526.API.DTOs.HerramientaDTOs;
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

        public HerramientasController(ApplicationDbContext context,
            ILogger<HerramientasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaAlquilar(string? nombre, string? fabricante, string? material, float? precio)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Include(h => h.AlquilarItems)
                    .ThenInclude(ai => ai.Alquiler)
                .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                    && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                    && ((h.Material.Equals(material)) || (material == null))
                    && ((h.Precio.Equals(precio) ) || (precio == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaAlquilarDTO(
                    h.Fabricante,
                    h.Nombre,
                    h.Material,
                    h.Precio
                ))
                .ToListAsync();

            return Ok(herramientas);
        }

    }
}
