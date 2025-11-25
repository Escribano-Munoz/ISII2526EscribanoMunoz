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
        public async Task<ActionResult> CreateReparacion( [FromBody]ReparacionCreateDTO reparacionForCreate) //FromBody para que funcione correctamente
        {
            // Validaciones básicas (Flujo Alternativo 1)
            if (reparacionForCreate.FechaEntrega <= DateTime.Today)
                ModelState.AddModelError("FechaEntrega", "Error! La fecha de entrega debe ser posterior a hoy");

            if (reparacionForCreate.FechaRecogida <= reparacionForCreate.FechaEntrega)
                ModelState.AddModelError("FechaRecogida", "Error! La fecha de recogida debe ser posterior a la fecha de entrega");

            if (reparacionForCreate.ReparacionItems.Count == 0)
                ModelState.AddModelError("ReparacionItems", "Error! Debes incluir al menos una herramienta para reparar");

            if (!Enum.IsDefined(typeof(tiposMetodoPago), reparacionForCreate.MetodoPago))
                ModelState.AddModelError("MetodoPago", "Error! El método de pago seleccionado no es válido");

            // Validar items
            foreach (var item in reparacionForCreate.ReparacionItems)
            {
                if (item.Cantidad <= 0)
                    ModelState.AddModelError($"ReparacionItems", $"Error! La cantidad para la herramienta debe ser mayor a 0");
            }

            // Buscar usuario
            var user = _context.ApplicationUsers.FirstOrDefault(au =>
                au.NombreCliente == reparacionForCreate.NombreCliente &&
                au.ApellidoCliente == reparacionForCreate.ApellidoCliente);

            if (user == null)
                ModelState.AddModelError("Cliente", "Error! Cliente no registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Obtener herramientas y verificar disponibilidad
            var herramientaIDs = reparacionForCreate.ReparacionItems.Select(ri => ri.HerramientaID).ToList();

            var herramientas = await _context.Herramienta
                .Include(h => h.ReparacionItems)
                    .ThenInclude(ri => ri.Reparacion)
                .Where(h => herramientaIDs.Contains(h.Id))
                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.TiempoReparacion,
                    h.Precio,
                    // Verificar disponibilidad en el período solicitado
                    NumReparacionesEnPeriodo = h.ReparacionItems.Count(ri =>
                        ri.Reparacion.FechaEntrega <= reparacionForCreate.FechaRecogida &&
                        ri.Reparacion.FechaRecogida >= reparacionForCreate.FechaEntrega)
                })
                .ToListAsync();

            // Crear reparación
            Reparacion reparacion = new Reparacion(
                new List<ReparacionItem>(),
                reparacionForCreate.FechaRecogida,
                reparacionForCreate.FechaEntrega,
                0, // Precio total se calculará después
                reparacionForCreate.MetodoPago,
                user
            );

            // Crear items y validar
            foreach (var item in reparacionForCreate.ReparacionItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Id == item.HerramientaID);

                if (herramienta == null)
                {
                    ModelState.AddModelError("ReparacionItems", $"Error! La herramienta con ID {item.HerramientaID} no existe");
                    continue;
                }

                // Verificar tiempo de reparación
                var diasDisponibles = (reparacionForCreate.FechaRecogida - reparacionForCreate.FechaEntrega).TotalDays;

                if (diasDisponibles < herramienta.TiempoReparacion)
                {
                    ModelState.AddModelError("FechaRecogida",
                        $"Error! La herramienta '{herramienta.Nombre}' requiere {herramienta.TiempoReparacion} días para reparación, pero solo hay {diasDisponibles} días disponibles");
                    continue;
                }

                // Crear item de reparación
                var reparacionItem = new ReparacionItem(
                    herramientaId: herramienta.Id,
                    cantidad: item.Cantidad,
                    descripcion: item.Descripcion,
                    precio: (float)herramienta.Precio,
                    reparacion: reparacion
                );

                reparacion.ReparacionItems.Add(reparacionItem);

                // Actualizar precio en el DTO
                item.Precio = (double)herramienta.Precio;
            }

            // Calcular precio total
            reparacion.PrecioTotal = reparacion.ReparacionItems.Sum(ri => ri.precio * ri.cantidad);

            // Si hay errores en las validaciones de items
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Guardar en base de datos
            _context.Reparacion.Add(reparacion);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la reparación");
                ModelState.AddModelError("Reparacion", "Error! Hubo un problema al guardar la reparación, por favor intente más tarde");
                return Conflict("Error: " + ex.Message);
            }

            // Crear DTO de respuesta
            var reparacionDetail = new ReparacionDetailDTO(
                reparacion.Id,
                reparacion.ApplicationUser.NombreCliente,
                reparacion.ApplicationUser.ApellidoCliente,
                reparacion.FechaRecogida,
                reparacion.FechaEntrega,
                reparacionForCreate.ReparacionItems
            );

            return CreatedAtAction("GetReparacionDetail", new { id = reparacion.Id }, reparacionDetail);
        }
    }
}
