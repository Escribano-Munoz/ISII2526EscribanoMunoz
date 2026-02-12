using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CrearOfertasDTOs
{
    public class OfertaItemDTO
    {
        public OfertaItemDTO()
        {
        }

       

        public OfertaItemDTO(string nombre, string material, string fabricante,
                          decimal precioOriginal, decimal precioFinal)
        {
            Nombre = nombre;
            Material = material;
            Fabricante = fabricante;
            PrecioOriginal = precioOriginal;
            PrecioFinal = precioFinal;
        }

       

       
        public string Nombre { get; set; }
        public string Material { get; set; }
        public string Fabricante { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal PrecioFinal { get; set; }

        public decimal PorcentajeDescuento { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is OfertaItemDTO dTO &&
                  
                   Nombre == dTO.Nombre &&
                   Material == dTO.Material &&
                   Fabricante == dTO.Fabricante &&
                   PrecioOriginal == dTO.PrecioOriginal &&
                   PrecioFinal == dTO.PrecioFinal;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Nombre, Material, Fabricante, PrecioOriginal, PrecioFinal);
        }
    }
}
