using AppForSEII2526.API.DTOs.ReparacionDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReparacionesController> _logger;

        public ReparacionesController(ApplicationDbContext context, ILogger<ReparacionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReparacionDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReparacionDetail(int id)
        {
            if (_context.Reparacion == null)
            {
                _logger.LogError("Error: Reparaciones table does not exist");
                return NotFound();
            }

            var reparacion = await _context.Reparacion
             .Where(r => r.Id == id)
                 .Include(r => r.ApplicationUser)
                 .Include(r => r.ReparacionItems)
                    .ThenInclude(ri => ri.Herramienta)
                        .ThenInclude(herramienta => herramienta.Fabricante)
             .Select(r => new ReparacionDetailDTO(r.Id, r.ApplicationUser.NombreCliente, r.ApplicationUser.ApellidoCliente,
                    r.FechaRecogida, r.FechaEntrega,
                    r.ReparacionItems
                        .Select(ri => new ReparacionItemDTO(ri.Herramienta.Id,
                                ri.Herramienta.Nombre, ri.Herramienta.Precio,
                                ri.cantidad, ri.descripcion)).ToList<ReparacionItemDTO>()))
             .FirstOrDefaultAsync();


            if (reparacion == null)
            {
                _logger.LogError($"Error: Reparacion with id {id} does not exist");
                return NotFound();
            }


            return Ok(reparacion);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReparacionDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateReparacion(ReparacionCreateDTO reparacionForCreate)
        {
            if (reparacionForCreate.FechaEntrega <= DateTime.Today)
                ModelState.AddModelError("Fecha Entrega", "Error. Fecha de entrega debe ser posterior a hoy");

            if (reparacionForCreate.FechaRecogida <= reparacionForCreate.FechaEntrega)
                ModelState.AddModelError("Fecha Entrega", "Error. Fecha de recogida debe ser posterior a Fecha de entrega");

            if (reparacionForCreate.ReparacionItems.Count == 0)
                ModelState.AddModelError("ReparacionItems", "Error. Debes incluir al menos una herramienta para reparar");

            if (!Enum.IsDefined(typeof(tiposMetodoPago), reparacionForCreate.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error! El método de pago seleccionado no es válido");


            foreach (var item in reparacionForCreate.ReparacionItems)
            {
                if (item.Cantidad <= 0)
                    ModelState.AddModelError($"ReparacionItems-{item.HerramientaID}", $"Error. La cantidad debe ser mayor a 0");
            }

            var user = _context.ApplicationUsers.FirstOrDefault(au => au.NombreCliente == reparacionForCreate.NombreCliente && au.ApellidoCliente == reparacionForCreate.ApellidoCliente);
            if (user == null)
                ModelState.AddModelError("Cliente", "Error. Cliente no registrado");


            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var herramientaIDs = reparacionForCreate.ReparacionItems.Select(ri => ri.HerramientaID).ToList();

            var herramientas = _context.Herramienta.Include(h => h.ReparacionItems)
                .ThenInclude(ri => ri.Reparacion)
                .Where(h => herramientaIDs.Contains(h.Id))

                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.TiempoReparacion,
                    h.Precio,
                    NumReparacionItems = h.ReparacionItems.Count(ri =>
                ri.Reparacion.FechaEntrega <= reparacionForCreate.FechaRecogida &&
                ri.Reparacion.FechaRecogida >= reparacionForCreate.FechaEntrega)
                })
                .ToList();

            Reparacion reparacion = new Reparacion(new List<ReparacionItem>(), reparacionForCreate.FechaRecogida, reparacionForCreate.FechaEntrega, (float)reparacionForCreate.PrecioTotal, reparacionForCreate.MetodoPago, user);

            reparacion.PrecioTotal = 0;


            foreach (var item in reparacionForCreate.ReparacionItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Id == item.HerramientaID);

                if ((herramienta == null) || (herramienta.NumReparacionItems >= item.Cantidad))
                {
                    ModelState.AddModelError("ReparacionItems", $"Error! La herramienta con ID {item.HerramientaID} no existe");
                }
                else
                {
                    var diasReparacion = (reparacionForCreate.FechaRecogida - reparacionForCreate.FechaEntrega).TotalDays;

                    if (diasReparacion < herramienta.TiempoReparacion)
                    {
                        ModelState.AddModelError("Fecha Recogida", $"Error. La herramienta '{herramienta.Nombre}' requiere {herramienta.TiempoReparacion} días para reparacion, no hay suficiente tiempo entre las fechas seleccionadas");
                    }
                    else
                    {
                        reparacion.ReparacionItems.Add(new ReparacionItem(
                        herramienta.Id,
                        item.Cantidad,
                        item.Descripcion,
                        herramienta.Precio,
                        reparacion));

                        item.Precio = herramienta.Precio;
                    }
                }
            }

            reparacion.PrecioTotal = reparacion.ReparacionItems.Sum(ri => ri.precio * ri.cantidad);

            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            _context.Add(reparacion);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Reparacion", $"Error. Hubo un error al guardar la reparacion, por favor, intentelo mas tarde");
                return Conflict("Error" + ex.Message);

            }

            var reparacionDetail = new ReparacionDetailDTO(
                reparacion.Id,
                reparacion.ApplicationUser.NombreCliente,
                reparacion.ApplicationUser.ApellidoCliente,
                reparacion.FechaRecogida,
                reparacion.FechaEntrega,
                reparacionForCreate.ReparacionItems);

            return CreatedAtAction("GetReparacion", new { id = reparacion.Id }, reparacionDetail);
        }
    }
}