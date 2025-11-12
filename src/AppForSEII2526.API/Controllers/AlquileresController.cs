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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(AlquilerDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateAlquiler(AlquilerCreateDTO alquilerCreate)
        {
            //any validation defined in PurchaseForCreate is checked before running the method so they don't have to be checked again
            if (alquilerCreate.FechaInicio <= DateTime.Today)
                ModelState.AddModelError("FechaInicio", "Error! Tu fecha de alquiler debe empezar despues hoy");

            if (alquilerCreate.FechaInicio >= alquilerCreate.FechaFin)
                ModelState.AddModelError("FechaInicio&FechaFin", "Error! Tu fecha de alquiler final debe terminar despues de la fecha de inicio");

            if (alquilerCreate.AlquilarItems.Count == 0)
                ModelState.AddModelError("AlquilarItems", "Error! Debes incluir una herramienta para que pueda ser alquilada");

            // if (!_context.ApplicationUsers.Any(au=>au.UserName==rentalForCreate.CustomerUserName))
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.NombreCliente == alquilerCreate.NombreCliente);
            if (user == null)
                ModelState.AddModelError("AlquilerApplicationUser", "Error! Nombre de usuario no registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var herramientaNombres = alquilerCreate.AlquilarItems.Select(ai => ai.Nombre).ToList<string>();

            var herramientas = _context.Herramienta.Include(h => h.AlquilarItems)
                .ThenInclude(ai => ai.Alquiler)
                .Where(h => herramientaNombres.Contains(h.Nombre))

                //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                .Select(h => new {
                    h.Id,
                    h.Nombre,
                    h.Material,
                    h.Precio,
                    //we count the number of rentalItems that are within the rental period
                    NumeroDeAlquilados = h.AlquilarItems.Count(ai => ai.Alquiler.FechaInicio <= alquilerCreate.FechaFin
                            && ai.Alquiler.FechaFin >= alquilerCreate.FechaInicio)
                })
                .ToList();


            Alquiler alquiler = new Alquiler(user, alquilerCreate.PrecioTotal, DateTime.Now, alquilerCreate.FechaInicio, alquilerCreate.FechaFin, alquilerCreate.DireccionEnvio, (AppForSEII2526.API.Models.TiposMetodoPago)alquilerCreate.MetodoPago, alquilerCreate.NombreCliente, alquilerCreate.ApellidoCliente, new List<AlquilarItem>())
            {

            };
            alquiler.PrecioTotal = 0;
            var numDays = (alquiler.FechaFin - alquiler.FechaInicio).TotalDays;


            foreach (var item in alquilerCreate.AlquilarItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == item.Nombre);
                //we must check that there is enough quantity to be rented in the database
                if ((herramienta == null) || (herramienta.NumeroDeAlquilados >= item.Cantidad))
                {
                    ModelState.AddModelError("AlquilarItems", $"Error! El nombre de la herramienta '{item.Nombre}' no esta disponible para ser alquilado desde {alquilerCreate.FechaInicio.ToShortDateString()} hasta {alquilerCreate.FechaFin.ToShortDateString()}");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    alquiler.AlquilarItems.Add(new AlquilarItem(herramienta.Id, alquiler, herramienta.Precio, item.Cantidad));
                    item.Precio = herramienta.Precio;
                }
            }
            alquiler.PrecioTotal = alquiler.AlquilarItems.Sum(ai => ai.Precio * numDays);


            //if there is any problem because of the available quantity of movies or because the movie does not exist
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(alquiler);

            try
            {
                //we store in the database both rental and its rentalitems
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Alquiler", $"Error! Ha habido un error guardando tu alquiler, por favor, prueba de nuevo más tarde");
                return Conflict("Error" + ex.Message);

            }

            //it returns rentalDetail
            var alquilerDetail = new AlquilerDetailDTO(alquiler.Id, alquiler.FechaAlquiler,
                alquiler.ApplicationUser.NombreCliente, alquiler.ApplicationUser.ApellidoCliente,
                alquiler.ApplicationUser.DireccionEnvio,
                alquiler.FechaInicio, alquiler.FechaFin,
                alquilerCreate.AlquilarItems);

            return CreatedAtAction("GetAlquiler", new { id = alquiler.Id }, alquilerDetail);
        }

    }
}
