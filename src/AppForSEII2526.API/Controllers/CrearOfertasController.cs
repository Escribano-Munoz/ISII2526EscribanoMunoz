using AppForSEII2526.API.DTOs.CrearOfertasDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrearOfertasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CrearOfertasController> _logger;

        public CrearOfertasController(ApplicationDbContext context, ILogger<CrearOfertasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CrearOfertasDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOfertaDetail(int id)
        {
            if (_context.Oferta == null)
            {
                _logger.LogError("Error: Ofertas table does not exist");
                return NotFound();
            }

            var oferta = await _context.Oferta
                .Where(o => o.Id == id)
                .Include(o => o.OfertaItems) 
                    .ThenInclude(oi => oi.herramienta) 
                        .ThenInclude(herramienta => herramienta.Fabricante) 
                .Select(o => new CrearOfertasDetailDTO(
                    o.Id,
                    o.fechaCreacion,
                    o.fechaInicio,  
                    o.fechaFinal,     
                    o.metodoPago,
                    (tiposDirigidaOferta)o.paraSocio, 
                    o.OfertaItems
                        .Select(oi => new OfertaItemDTO(
                        
                            oi.herramienta.Id,
                            oi.herramienta.Nombre,
                            oi.herramienta.Material,
                            oi.herramienta.Fabricante.nombre,
                            oi.precioOriginal,
                            oi.precioFinal,
                            oi.porcentaje
                        )).ToList<OfertaItemDTO>()))
                .FirstOrDefaultAsync();

            if (oferta == null)
            {
                _logger.LogError($"Error: Oferta with id {id} does not exist");
                return NotFound();
            }

            return Ok(oferta);
        }

    }
}
