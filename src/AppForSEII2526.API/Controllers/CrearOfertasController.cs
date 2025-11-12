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
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CrearOfertasDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateOferta(CrearOfertasCreateDTO ofertaForCreate)
        {
            
            if (ofertaForCreate.FechaInicio <= DateTime.Today)
                ModelState.AddModelError("FechaInicio", "Error! La fecha de inicio debe ser posterior a hoy");

            if (ofertaForCreate.FechaInicio >= ofertaForCreate.FechaFinal)
                ModelState.AddModelError("FechaInicio&FechaFin", "Error! La oferta debe terminar después de que comience");

            if (ofertaForCreate.OfertaItems.Count == 0)
                ModelState.AddModelError("OfertaItems", "Error! Debe incluir al menos una herramienta para la oferta");

            
            if (!Enum.IsDefined(typeof(tiposMetodoPago), ofertaForCreate.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error! El método de pago seleccionado no es válido");

           
            foreach (var item in ofertaForCreate.OfertaItems)
            {
                if (item.PorcentajeDescuento <= 0 || item.PorcentajeDescuento > 100)
                    ModelState.AddModelError("PorcentajeDescuento", $"Error! El porcentaje de rebaja debe estar entre 0% y 100%");

                if (item.HerramientaID <= 0)
                    ModelState.AddModelError("HerramientaID", "Error! El ID de la herramienta es obligatorio");

                if (string.IsNullOrEmpty(item.Nombre))
                    ModelState.AddModelError("Nombre", "Error! El nombre de la herramienta es obligatorio");

                if (string.IsNullOrEmpty(item.Material))
                    ModelState.AddModelError("Material", "Error! El material de la herramienta es obligatorio");

                if (item.PrecioOriginal <= 0)
                    ModelState.AddModelError("PrecioOriginal", "Error! El precio original debe ser mayor a 0");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var herramientaIds = ofertaForCreate.OfertaItems.Select(oi => oi.HerramientaID).ToList();

            var herramientas = await _context.Herramienta
                .Include(h => h.Fabricante)
                .Where(h => herramientaIds.Contains(h.Id))
                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.Material,
                    Fabricante = h.Fabricante.nombre,
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
                var herramienta = herramientas.FirstOrDefault(h => h.Id == item.HerramientaID);

                
                if (herramienta == null)
                {
                    ModelState.AddModelError("OfertaItems", $"Error! La herramienta con ID {item.HerramientaID} no existe");
                }
                else
                {
                    decimal precioOriginal = (decimal)herramienta.Precio;
                    decimal precioConDescuento = precioOriginal * (1 - (item.PorcentajeDescuento / 100m));

                    oferta.OfertaItems.Add(new OfertaItem(
                    idHerramienta: herramienta.Id,
                    idOferta: 0,
                    porcentaje: item.PorcentajeDescuento,
                    precioFinal: precioConDescuento,
                    oferta: oferta,
                    herramienta: await _context.Herramienta.FindAsync(herramienta.Id)
                    ));

                    item.PrecioFinal = precioConDescuento;

                }
                oferta.PrecioTotal = oferta.OfertaItems.Sum(oi => oi.precioFinal);
            }

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
