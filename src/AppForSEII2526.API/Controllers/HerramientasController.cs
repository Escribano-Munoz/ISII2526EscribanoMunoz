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
