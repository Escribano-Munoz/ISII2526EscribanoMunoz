using AppForSEII2526.API.DTOs.CompraDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            //any validation defined in CompraCreate is checked before running the method so they don't have to be checked again
            if (compraCreate.CompraItems.Count == 0)
                ModelState.AddModelError("CompraItems", "Error! Debes incluir al menos una herramienta para comprar");

            // if (!_context.ApplicationUsers.Any(au=>au.UserName==rentalForCreate.CustomerUserName))
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.NombreCliente == compraCreate.NombreCliente);
            if (user == null)
                ModelState.AddModelError("CompraApplicationUser", "Error! NombreCliente no esta registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var herramientaNombres = compraCreate.CompraItems.Select(ci => ci.Nombre).ToList<string>();

            var herramientas = _context.Herramienta.Include(h => h.CompraItems)
                .ThenInclude(ci => ci.Compra)
                .Where(h => herramientaNombres.Contains(h.Nombre))

                //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.Material,
                    h.Precio,
                    NumeroDeComprados = h.CompraItems
                        .Where(ci => herramientaNombres.Contains(h.Nombre))
                        .Sum(ci => ci.Cantidad),
                })
                .ToList();


            Compra compra = new Compra(DateTime.Now, new List<CompraItem>(),
                (AppForSEII2526.API.Models.TiposMetodoPago)compraCreate.MetodoPago, compraCreate.DireccionEnvio, user);


        compra.precioTotal = 0;


            foreach (var item in compraCreate.CompraItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == item.Nombre);
                //we must check that there is enough quantity to be rented in the database
                if ((herramienta == null) || (herramienta.NumeroDeComprados >= herramienta.Precio))
                {
                    ModelState.AddModelError("CompraItems", $"Error! Herramienta nombrada '{item.Nombre}' no es valido para ser comprado");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    compra.CompraItems.Add(new CompraItem(compra, herramienta.Id, herramienta.Precio, item.Cantidad, item.Descripcion));
                    item.Precio = herramienta.Precio;
                }
            }
            compra.precioTotal = compra.CompraItems.Sum(ci => (decimal)ci.Precio * ci.Cantidad);


            //if there is any problem because of the available quantity of movies or because the movie does not exist
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(compra);

            try
            {
                //we store in the database both rental and its rentalitems
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Compra", $"Error! Hubo un error al guardar su compra, por favor, intentelo mas tarde");
                return Conflict("Error" + ex.Message);

            }

            //it returns rentalDetail
            var compraDetail = new CompraDetailDTO(compra.Id,
                compra.ApplicationUser.NombreCliente, compra.ApplicationUser.ApellidoCliente,
                compra.ApplicationUser.DireccionEnvio, compra.FechaCompra,
                compraCreate.CompraItems);

            return CreatedAtAction("GetCompra", new { id = compra.Id }, compraDetail);
        }

    }
}
