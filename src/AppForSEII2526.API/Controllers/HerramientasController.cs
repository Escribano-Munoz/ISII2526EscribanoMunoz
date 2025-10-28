using AppForSEII2526.API.DTOs.HerramientaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientasController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system is running
        private readonly ILogger<HerramientasController> _logger;

        public HerramientasController(ApplicationDbContext context, ILogger<HerramientasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaCrearOfertaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaCrearOfertas(string? nombre, string? fabricante, string? material, float? precio)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Include(h => h.OfertaItems)
                    .ThenInclude(oi => oi.oferta)
                .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                    && ((h.Material.Equals(material)) || (material == null))
                    && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                    && ((h.Precio.Equals(precio)) || (precio == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaCrearOfertaDTO(
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
