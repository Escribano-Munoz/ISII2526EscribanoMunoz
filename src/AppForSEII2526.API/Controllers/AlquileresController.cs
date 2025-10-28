using AppForSEII2526.API.DTOs.AlquilerDTOs;
using AppForSEII2526.API.DTOs.HerramientaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<AlquileresController> _logger;

        public AlquileresController(ApplicationDbContext context, ILogger<AlquileresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(AlquilerDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetAlquilerDetail(int id)
        {
            if (_context.Alquiler == null)
            {
                _logger.LogError("Error: Alquileres table does not exist");
                return NotFound();
            }

            var alquiler = await _context.Alquiler
             .Where(a => a.Id == id)
                 .Include(a => a.AlquilarItems) //join table AlquilarItems
                    .ThenInclude(ai => ai.Herramienta) //then join table Herramientas
                        .ThenInclude(herramienta => herramienta.Fabricante) //then join table Fabricante
             .Select(a => new AlquilerDetailDTO(a.Id, a.FechaAlquiler, a.ApplicationUser.NombreCliente,
                    a.ApplicationUser.ApellidoCliente, a.ApplicationUser.DireccionEnvio,
                    a.FechaInicio, a.FechaFin,
                    a.AlquilarItems
                        .Select(ai => new AlquilarItemDTO(ai.Herramienta.Id,
                                ai.Herramienta.Nombre, ai.Herramienta.Material,
                                ai.Herramienta.Precio, ai.Cantidad)).ToList<AlquilarItemDTO>()))
             .FirstOrDefaultAsync();


            if (alquiler == null)
            {
                _logger.LogError($"Error: Alquiler with id {id} does not exist");
                return NotFound();
            }


            return Ok(alquiler);
        }


    }
}
