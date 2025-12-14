using AppForSEII2526.API.DTOs.HerramientaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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


            _logger.LogInformation("HerramientasController inicializado");
            _logger.LogInformation("LOG DE PRUEBA: Constructor HerramientasController");
            _logger.LogError(new Exception("Excepción de prueba"), "ERROR DE PRUEBA");
            _logger.LogWarning("WARNING DE PRUEBA");
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaCrearOfertaDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaCrearOfertas(string? nombre, string? fabricante, string? material, float? precio)
        {
            _logger.LogInformation("Iniciando GetHerramientasParaCrearOfertas");

            try
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


                _logger.LogInformation("GetHerramientasParaCrearOfertas completado. Encontradas: {Cantidad} herramientas", herramientas.Count);

                return Ok(herramientas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en GetHerramientasParaCrearOfertas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaRepararDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaReparar(string? nombre, string? fabricante, string? material, float? precio, int? maxDiasReparar)
        {
            _logger.LogInformation("Iniciando GetHerramientasParaReparar");
            _logger.LogDebug("Parámetros - MaxDiasReparar: {MaxDias}", maxDiasReparar?.ToString() ?? "null");

            try
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

                _logger.LogInformation("GetHerramientasParaReparar completado. Encontradas: {Cantidad} herramientas", herramientas.Count);

                return Ok(herramientas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en GetHerramientasParaReparar");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaComprar(string? nombre, string? material, string? fabricante, float? precio)
        {
            _logger.LogInformation("Iniciando GetHerramientasParaComprar");

            try
            {
                var herramientas = await _context.Herramienta
                    .Include(h => h.Fabricante)
                    .Include(h => h.CompraItems)
                        .ThenInclude(ci => ci.Compra)
                    .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                        && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                        && ((h.Material.Equals(material)) || (material == null))
                        && ((h.Precio.Equals(precio)) || (precio == null)))
                    .OrderBy(h => h.Nombre)
                    .Select(h => new HerramientaParaComprarDTO(
                        h.Fabricante,
                        h.Material,
                        h.Nombre,
                        h.Precio
                    ))
                    .ToListAsync();

                _logger.LogInformation("GetHerramientasParaComprar completado. Encontradas: {Cantidad} herramientas", herramientas.Count);

                return Ok(herramientas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en GetHerramientasParaComprar");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<HerramientaParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientasParaAlquilar(string? nombre, string? fabricante, string? material, float? precio)
        {
            _logger.LogInformation("Iniciando GetHerramientasParaAlquilar");

            try
            {
                var herramientas = await _context.Herramienta
                    .Include(h => h.Fabricante)
                    .Include(h => h.AlquilarItems)
                        .ThenInclude(ai => ai.Alquiler)
                    .Where(h => ((h.Nombre.Contains(nombre)) || (nombre == null))
                        && ((h.Fabricante.nombre.Equals(fabricante)) || (fabricante == null))
                        && ((h.Material.Equals(material)) || (material == null))
                        && ((h.Precio.Equals(precio)) || (precio == null)))
                    .OrderBy(h => h.Nombre)
                    .Select(h => new HerramientaParaAlquilarDTO(
                        h.Fabricante,
                        h.Nombre,
                        h.Material,
                        h.Precio
                    ))
                    .ToListAsync();

                _logger.LogInformation("GetHerramientasParaAlquilar completado. Encontradas: {Cantidad} herramientas", herramientas.Count);

                return Ok(herramientas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR en GetHerramientasParaAlquilar");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}