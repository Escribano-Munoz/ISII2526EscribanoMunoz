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
                ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

            if (alquilerCreate.FechaInicio >= alquilerCreate.FechaFin)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

            if (alquilerCreate.AlquilarItems.Count == 0)
                ModelState.AddModelError("RentalItems", "Error! You must include at least one movie to be rented");

            // if (!_context.ApplicationUsers.Any(au=>au.UserName==rentalForCreate.CustomerUserName))
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == rentalForCreate.CustomerUserName);
            if (user == null)
                ModelState.AddModelError("RentalApplicationUser", "Error! UserName is not registered");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var movieTitles = rentalForCreate.RentalItems.Select(ri => ri.Title).ToList<string>();

            var movies = _context.Movies.Include(m => m.RentalItems)
                .ThenInclude(ri => ri.Rent)
                .Where(m => movieTitles.Contains(m.Title))

                //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                .Select(m => new {
                    m.Id,
                    m.Title,
                    m.QuantityForRenting,
                    m.PriceForRenting,
                    //we count the number of rentalItems that are within the rental period
                    NumberOfRentedItems = m.RentalItems.Count(ri => ri.Rent.RentalDateFrom <= rentalForCreate.RentalDateTo
                            && ri.Rent.RentalDateTo >= rentalForCreate.RentalDateFrom)
                })
                .ToList();


            Rental rental = new Rental(rentalForCreate.CustomerUserName, rentalForCreate.CustomerNameSurname,
                user, rentalForCreate.DeliveryAddress, DateTime.Now,
                (AppForMovies.API.Models.PaymentMethodTypes)rentalForCreate.PaymentMethod,
rentalForCreate.RentalDateFrom, rentalForCreate.RentalDateTo, new List<RentalItem>());


            rental.TotalPrice = 0;
            var numDays = (rental.RentalDateTo - rental.RentalDateFrom).TotalDays;


            foreach (var item in rentalForCreate.RentalItems)
            {
                var movie = movies.FirstOrDefault(m => m.Title == item.Title);
                //we must check that there is enough quantity to be rented in the database
                if ((movie == null) || (movie.NumberOfRentedItems >= movie.QuantityForRenting))
                {
                    ModelState.AddModelError("RentalItems", $"Error! Movie titled '{item.Title}' is not available for being rented from {rentalForCreate.RentalDateFrom.ToShortDateString()} to {rentalForCreate.RentalDateTo.ToShortDateString()}");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    rental.RentalItems.Add(new RentalItem(movie.Id, rental, movie.PriceForRenting, item.Description));
                    item.PriceForRenting = movie.PriceForRenting;
                }
            }
            rental.TotalPrice = rental.RentalItems.Sum(ri => ri.PriceForRenting * numDays);


            //if there is any problem because of the available quantity of movies or because the movie does not exist
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(rental);

            try
            {
                //we store in the database both rental and its rentalitems
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Rental", $"Error! There was an error while saving your rental, plese, try again later");
                return Conflict("Error" + ex.Message);

            }

            //it returns rentalDetail
            var rentalDetail = new RentalDetailDTO(rental.Id, rental.RentalDate,
                rental.CustomerUserName, rental.CustomerNameSurname,
                rental.DeliveryAddress, rentalForCreate.PaymentMethod,
                rental.RentalDateFrom, rental.RentalDateTo,
                rentalForCreate.RentalItems);

            return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
        }


    }
}
