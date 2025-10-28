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

        private readonly ILogger<HerramientasController> _logger;

        public AlquileresController(ApplicationDbContext context, ILogger<HerramientasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetRental(int id)
        {
            if (_context.Rentals == null)
            {
                _logger.LogError("Error: Rentals table does not exist");
                return NotFound();
            }

            var rental = await _context.Rentals
             .Where(r => r.Id == id)
                 .Include(r => r.RentalItems) //join table RentalItems
                    .ThenInclude(ri => ri.Movie) //then join table Movies
                        .ThenInclude(movie => movie.Genre) //then join table Genre
             .Select(r => new RentalDetailDTO(r.Id, r.RentalDate, r.CustomerUserName,
                    r.CustomerNameSurname, r.DeliveryAddress,
                    (PaymentMethodTypes)r.PaymentMethod,
                    r.RentalDateFrom, r.RentalDateTo,
                    r.RentalItems
                        .Select(ri => new RentalItemDTO(ri.Movie.Id,
                                ri.Movie.Title, ri.Movie.Genre.Name,
                                ri.Movie.PriceForRenting, ri.Description)).ToList<RentalItemDTO>()))
             .FirstOrDefaultAsync();


            if (rental == null)
            {
                _logger.LogError($"Error: Rental with id {id} does not exist");
                return NotFound();
            }


            return Ok(rental);
        }


    }
}
