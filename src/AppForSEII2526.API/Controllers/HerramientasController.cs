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

        public HerramientasController(ApplicationDbContext context, ILogger<HerramientasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaComprar(string? nombre, string? material, string?fabricante, float? precio)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Include(h => h.CompraItems)
                    .ThenInclude(ci => ci.Compra)
                .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                    && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                    && ((h.Material.Equals(material)) || (fabricante == null))
                    && ((h.Precio.Equals(precio)) || (precio == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaComprarDTO(
                    h.Fabricante,
                    h.Material,
                    h.Nombre,
                    h.Precio
                ))
                .ToListAsync();

            return Ok(herramientas);
        }
    }
}
