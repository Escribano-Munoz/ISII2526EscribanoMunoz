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
                    o.paraSocio.HasValue ? (tiposDirigidaOferta?)o.paraSocio.Value : null, 
                    o.OfertaItems
                        .Select(oi => new OfertaItemDTO(
                            oi.herramienta.Nombre,
                            oi.herramienta.Material,
                            oi.herramienta.Fabricante.nombre,
                            oi.precioOriginal,
                            oi.precioFinal
                        )).ToList<OfertaItemDTO>()))
                .FirstOrDefaultAsync();

            if (oferta == null)
            {
                _logger.LogError($"Error: Oferta with id {id} does not exist");
                return NotFound();
            }

            return Ok(oferta);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CrearOfertasDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateOferta(CrearOfertasCreateDTO ofertaForCreate)
        {
            if (_context.Oferta == null)
            {
                _logger.LogError("Error: Ofertas table does not exist");
                return StatusCode(500, "Error al configurar la base de datos.");
            }

            // Validaciones de fechas (Flujo Alternativo 1)
            if (ofertaForCreate.FechaInicio <= DateTime.Today)
                ModelState.AddModelError("FechaInicio", "Error! La fecha de inicio debe ser posterior a hoy");

            if (ofertaForCreate.FechaInicio >= ofertaForCreate.FechaFinal)
                ModelState.AddModelError("FechaInicio&FechaFin", "Error! La oferta debe terminar después de que comience");

            // Validación de items (Flujo Alternativo 2)
            if (ofertaForCreate.OfertaItems.Count == 0)
                ModelState.AddModelError("OfertaItems", "Error! Debe incluir al menos una herramienta para la oferta");

            // Validación de método de pago
            if (!Enum.IsDefined(typeof(tiposMetodoPago), ofertaForCreate.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error! El método de pago seleccionado no es válido");

            // Validación de porcentajes (Flujo Alternativo 3)
            foreach (var item in ofertaForCreate.OfertaItems)
            {
                if (item.PorcentajeDescuento <= 0 || item.PorcentajeDescuento > 100)
                    ModelState.AddModelError("PorcentajeDescuento", $"Error! El porcentaje de rebaja debe estar entre 0% y 100%");

               
                if (string.IsNullOrEmpty(item.Nombre))
                    ModelState.AddModelError("Nombre", "Error! El nombre de la herramienta es obligatorio");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            
            var herramientaNombres = ofertaForCreate.OfertaItems.Select(oi => oi.Nombre).Distinct().ToList();

            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Where(h => herramientaNombres.Contains(h.Nombre))
                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.Material,
                    FabricanteNombre = h.Fabricante.nombre,
                    Precio = h.Precio
                })
                .ToListAsync();

          
            Oferta oferta = new Oferta(
                ofertaForCreate.FechaInicio,
                ofertaForCreate.FechaFinal,
                (AppForSEII2526.API.Models.tiposMetodoPago)ofertaForCreate.MetodoPago,
                (AppForSEII2526.API.Models.tiposDirigidaOferta)ofertaForCreate.DirigidoA,
                DateTime.Now,
                new List<OfertaItem>()
            );

            oferta.PrecioTotal = 0;

            
            foreach (var item in ofertaForCreate.OfertaItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == item.Nombre);

                // Validar que la herramienta existe
                if (herramienta == null)
                {
                    ModelState.AddModelError("OfertaItems", $"Error! La herramienta '{item.Nombre}' no existe");
                }
                else
                {
                    
                    decimal precioOriginal = (decimal)herramienta.Precio;
                    decimal precioConDescuento = precioOriginal * (1 - (item.PorcentajeDescuento / 100m));

                    
                    var herramientaCompleta = await _context.Herramienta.FindAsync(herramienta.Id);

                    
                    oferta.OfertaItems.Add(new OfertaItem(
                        herramienta: herramientaCompleta,
                        oferta: oferta,
                        porcentaje: item.PorcentajeDescuento,
                        precioFinal: precioConDescuento
                    ));

                    
                    item.PrecioFinal = precioConDescuento;
                }
            }

            
            oferta.PrecioTotal = oferta.OfertaItems.Sum(oi => oi.precioFinal);

            
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Oferta.Add(oferta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Oferta", "Error! Hubo un problema al guardar la oferta, por favor intente más tarde");
                return Conflict("Error: " + ex.Message);
            }

           
            var ofertaDetail = new CrearOfertasDetailDTO(
                oferta.Id,
                oferta.fechaCreacion,
                oferta.fechaInicio,
                oferta.fechaFinal,
                ofertaForCreate.MetodoPago,
                ofertaForCreate.DirigidoA,
                ofertaForCreate.OfertaItems
            );

            return CreatedAtAction("GetOfertaDetail", new { id = oferta.Id }, ofertaDetail);
        }

    }
}
