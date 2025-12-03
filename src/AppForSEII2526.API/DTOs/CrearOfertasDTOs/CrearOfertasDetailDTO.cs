using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CrearOfertasDTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrearOfertasDetailDTO : CrearOfertasCreateDTO
    {
        public CrearOfertasDetailDTO(int id, DateTime fechaCreacion, DateTime fechaInicio, DateTime fechaFinal,
             tiposMetodoPago metodoPago, tiposDirigidaOferta? paraSocio, IList<OfertaItemDTO> ofertaItems)
             : base(fechaInicio, fechaFinal, metodoPago, paraSocio ?? tiposDirigidaOferta.Clientes, ofertaItems)
        {
            Id = id;
            FechaCreacion = fechaCreacion;
        }

        public int Id { get; set; }

        public DateTime FechaCreacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CrearOfertasDetailDTO dTO &&
                   base.Equals(obj) &&
                   Id == dTO.Id &&
                   CompareDate(FechaCreacion, dTO.FechaCreacion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, FechaCreacion);
        }
    }
}
