using AppForSEII2526.API.DTOs.CompraDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasController> _logger;

        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompraDetail(int id)
        {
            if (_context.Compra == null)
            {
                _logger.LogError("Error: Compra table does not exist");
                return NotFound();
            }

            var compra = await _context.Compra
             .Where(c => c.Id == id)
                 .Include(c => c.CompraItems) //join table RentalItems
                    .ThenInclude(ci => ci.Herramienta) //then join table Movies
                        .ThenInclude(herramienta => herramienta.Fabricante) //then join table Genre
             .Select(c => new CompraDetailDTO(c.ApplicationUser.Id, c.ApplicationUser.NombreCliente,
                    c.ApplicationUser.ApellidoCliente, c.ApplicationUser.DireccionEnvio, c.FechaCompra,
                    c.CompraItems
                        .Select(ci => new CompraItemDTO(ci.Herramienta.Id,
                                ci.Herramienta.Nombre, ci.Herramienta.Material,
                                ci.Herramienta.Precio, ci.Cantidad, ci.Descripcion)).ToList<CompraItemDTO>()))
             .FirstOrDefaultAsync();


            if (compra == null)
            {
                _logger.LogError($"Error: Compra with id {id} does not exist");
                return NotFound();
            }


            return Ok(compra);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateCompra(CompraCreateDTO compraCreate)
        {
            // Validaciones iniciales
            if (compraCreate.CompraItems.Count == 0)
                ModelState.AddModelError("CompraItems", "Error! Debes incluir al menos una herramienta para comprar");

            if (compraCreate.FechaCompra.Date < DateTime.Today.Date)
                ModelState.AddModelError("FechaCompra", "Error! Tu fecha de compra debe empezar al menos hoy");

            if (!Enum.IsDefined(typeof(TiposMetodoPago), compraCreate.MetodoPago))
            {
                ModelState.AddModelError("MetodoPago", "Error! El Metodo de Pago no es valido");
            }

            foreach (var item in compraCreate.CompraItems)
            {
                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("CompraItems", "Error! Debes seleccionar al menos una herramienta de ese tipo para comprarla");
                }
            }

            // Buscar usuario de forma asíncrona
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(au => au.NombreCliente == compraCreate.NombreCliente
                                        && au.ApellidoCliente == compraCreate.ApellidoCliente);

            if (user == null)
                ModelState.AddModelError("CompraApplicationUser", "Error! Cliente no esta registrado");

            // Si hay errores de validación, retornar BadRequest
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Obtener IDs de herramientas del DTO
            var herramientaIDs = compraCreate.CompraItems.Select(ci => ci.HerramientaID).Distinct().ToList();

            // Obtener información básica de herramientas para validación
            var herramientasInfo = await _context.Herramienta
                .Where(h => herramientaIDs.Contains(h.Id))
                .Select(h => new
                {
                    h.Id,
                    h.Nombre,
                    h.Material,
                    h.Precio
                })
                .ToListAsync();

            // Obtener las entidades Herramienta reales para establecer relaciones
            var herramientasReales = await _context.Herramienta
                .Where(h => herramientaIDs.Contains(h.Id))
                .ToListAsync();

            // Diccionario para acceso rápido a herramientas por ID
            var herramientasDict = herramientasReales.ToDictionary(h => h.Id, h => h);

            // Crear la compra
            Compra compra = new Compra(
                compraCreate.FechaCompra,
                new List<CompraItem>(),
                compraCreate.MetodoPago,
                compraCreate.DireccionEnvio,
                0,
                user);

            // Procesar cada item del DTO
            foreach (var itemDTO in compraCreate.CompraItems)
            {
                // Buscar información de la herramienta para validar existencia
                var herramientaInfo = herramientasInfo.FirstOrDefault(h => h.Id == itemDTO.HerramientaID);

                // Validar si la herramienta existe
                if (herramientaInfo == null)
                {
                    ModelState.AddModelError("CompraItems", $"Error! Herramienta con ID '{itemDTO.HerramientaID}' no encontrada");
                    continue;
                }

                // Obtener la entidad Herramienta real para establecer la relación
                if (!herramientasDict.TryGetValue(itemDTO.HerramientaID, out var herramientaReal))
                {
                    ModelState.AddModelError("CompraItems", $"Error! Herramienta con ID '{itemDTO.HerramientaID}' no disponible");
                    continue;
                }

                try
                {
                    // Crear el CompraItem
                    var compraItem = new CompraItem(
                        compra: compra,
                        herramientaId: herramientaReal.Id,
                        precio: herramientaReal.Precio,
                        cantidad: itemDTO.Cantidad,
                        descripcion: itemDTO.Descripcion
                    );

                    // IMPORTANTE: Establecer la relación con la Herramienta
                    // Esto es necesario para que EF Core reconozca la relación
                    compraItem.Herramienta = herramientaReal;

                    // Agregar a la colección de CompraItems
                    compra.CompraItems.Add(compraItem);

                    // Actualizar precio en el DTO (opcional)
                    itemDTO.Precio = (double)herramientaReal.Precio;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CompraItems",
                        $"Error al crear item para herramienta '{herramientaReal.Nombre}': {ex.Message}");
                    continue;
                }
            }

            // Si hay errores después de procesar los items
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Calcular precio total de la compra
            compra.precioTotal = compra.CompraItems.Sum(ci => (decimal)ci.Precio * ci.Cantidad);

            try
            {
                // Agregar la compra al contexto (esto incluye sus CompraItems)
                _context.Compra.Add(compra);

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al guardar la compra");

                // Log del inner exception para debugging
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception detalles");
                    return Conflict($"Error en base de datos: {ex.InnerException.Message}");
                }

                return Conflict("Error! No se pudo guardar la compra en la base de datos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar la compra");
                return Conflict($"Error! Hubo un problema al procesar tu compra: {ex.Message}");
            }

            // Recargar la compra con todas las relaciones necesarias para el DTO
            // Esto asegura que tengamos todos los datos cargados
            var compraCompleta = await _context.Compra
                .AsNoTracking() // Solo lectura, mejora performance
                .Include(c => c.ApplicationUser)
                .Include(c => c.CompraItems)
                    .ThenInclude(ci => ci.Herramienta)
                .FirstOrDefaultAsync(c => c.Id == compra.Id);

            if (compraCompleta == null)
            {
                return Conflict("Error! La compra fue creada pero no se puede recuperar para mostrar detalles");
            }

            // Convertir CompraItems a DTOs
            var compraItemDTOs = compraCompleta.CompraItems.Select(ci =>
                new CompraItemDTO(
                    ci.herramientaId,
                    ci.Herramienta?.Nombre ?? "Herramienta no disponible",
                    ci.Herramienta?.Material ?? "Material no especificado",
                    (double)ci.Precio,
                    ci.Cantidad,
                    ci.Descripcion ?? string.Empty
                )).ToList();

            // Crear el DTO de respuesta
            var compraDetail = new CompraDetailDTO(
                compraCompleta.Id,
                compraCompleta.ApplicationUser?.NombreCliente ?? compraCreate.NombreCliente,
                compraCompleta.ApplicationUser?.ApellidoCliente ?? compraCreate.ApellidoCliente,
                compraCompleta.DireccionEnvio,
                compraCompleta.FechaCompra,
                compraCompleta.MetodoPago,
                compraItemDTOs
            );
            return CreatedAtAction("GetCompra", new { id = compra.Id }, compraDetail);
        }

    }
}
