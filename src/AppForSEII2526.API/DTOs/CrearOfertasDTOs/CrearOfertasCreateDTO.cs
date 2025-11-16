using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CrearOfertasDTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrearOfertasCreateDTO
    {
        public CrearOfertasCreateDTO(DateTime fechaInicio, DateTime fechaFinal, tiposMetodoPago metodoPago, tiposDirigidaOferta dirigidoA, IList<OfertaItemDTO> ofertaItems)
        {
            FechaInicio = fechaInicio;
            FechaFinal = fechaFinal;
            MetodoPago = metodoPago;
            DirigidoA = dirigidoA;
            OfertaItems = ofertaItems ?? throw new ArgumentNullException(nameof(ofertaItems));
        }

        public CrearOfertasCreateDTO()
        {
            OfertaItems = new List<OfertaItemDTO>();
        }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFinal { get; set; }

        [Required]
        public tiposMetodoPago MetodoPago { get; set; }

        public tiposDirigidaOferta DirigidoA { get; set; }

        public IList<OfertaItemDTO> OfertaItems { get; set; }

        [Display(Name = "Descuento Total")]
        [JsonPropertyName("DescuentoTotal")]
        public decimal DescuentoTotal
        {
            get
            {
                if (OfertaItems == null || !OfertaItems.Any()) return 0;
                return OfertaItems.Sum(oi => oi.PrecioOriginal - oi.PrecioFinal);
            }
        }

        [Display(Name = "Precio Total Final")]
        [JsonPropertyName("PrecioTotalFinal")]
        public decimal PrecioTotalFinal
        {
            get
            {
                if (OfertaItems == null || !OfertaItems.Any()) return 0;
                return OfertaItems.Sum(oi => oi.PrecioFinal);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is CrearOfertasCreateDTO dTO &&
                   CompareDate(FechaInicio, dTO.FechaInicio) &&
                   CompareDate(FechaFinal, dTO.FechaFinal) &&
                   MetodoPago == dTO.MetodoPago &&
                   DirigidoA == dTO.DirigidoA &&
                   OfertaItems.SequenceEqual(dTO.OfertaItems) &&
                   DescuentoTotal == dTO.DescuentoTotal &&
                   PrecioTotalFinal == dTO.PrecioTotalFinal;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FechaInicio, FechaFinal, MetodoPago, DirigidoA, OfertaItems);
        }
    }
}
