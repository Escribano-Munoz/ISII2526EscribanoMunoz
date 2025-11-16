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
        [ProducesResponseType(typeof(IList<HerramientaParaCrearOfertaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaCrearOfertas(string? fabricante, float? precio)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Where(h => ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
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

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaRepararDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaReparar(string? nombre, string? fabricante, string? material, float? precio, int? maxDiasReparar)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Include(h => h.ReparacionItems)
                    .ThenInclude(ri => ri.Reparacion)
                .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                    && ((h.Material.Equals(material)) || (material == null))
                    && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                    && ((h.Precio.Equals(precio)) || (precio == null))
                    && ((h.TiempoReparacion <= maxDiasReparar) || (maxDiasReparar == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaRepararDTO(
                    h.Fabricante,
                    h.Material,
                    h.Nombre,
                    h.Precio,
                    h.TiempoReparacion
                    ))
                .ToListAsync();

            return Ok(herramientas);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaComprar(string? material, float? precio)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Where(h => ((h.Material.Contains(material)) || (material == null))
                    && ((h.Precio.Equals(precio)) || (precio == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaComprarDTO(
                    h.Fabricante.nombre,
                    h.Material,
                    h.Nombre,
                    h.Precio
                ))
                .ToListAsync();

            return Ok(herramientas);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaAlquilar(string? nombre, string? material)
        {
            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                    && ((h.Material.Contains(material)) || (material == null)))
                .OrderBy(h => h.Nombre)
                .Select(h => new HerramientaParaAlquilarDTO(
                    h.Fabricante.nombre,
                    h.Material,
                    h.Nombre,
                    h.Precio
                ))
                .ToListAsync();

            return Ok(herramientas);
        }
    }
}
