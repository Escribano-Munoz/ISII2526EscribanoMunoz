using AppForSEII2526.API.DTOs.CompraDTOs;
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

       
    }
}
