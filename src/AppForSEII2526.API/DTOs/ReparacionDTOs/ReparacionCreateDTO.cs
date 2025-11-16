using System.Reflection.Metadata.Ecma335;

namespace AppForSEII2526.API.DTOs.ReparacionDTOs
{
    public class ReparacionCreateDTO
    {
        public ReparacionCreateDTO(string nombreCliente, string apellidoCliente, DateTime fechaRecogida,
            DateTime fechaEntrega, IList<ReparacionItemDTO> reparacionItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            FechaRecogida = fechaRecogida;
            FechaEntrega = fechaEntrega;
            ReparacionItems = reparacionItems ?? throw new ArgumentNullException(nameof(reparacionItems));
        }

        public ReparacionCreateDTO()
        {
            ReparacionItems = new List<ReparacionItemDTO>();
        }

        public DateTime FechaRecogida { get; set; }

        public DateTime FechaEntrega { get; set; }

        [JsonIgnore]
        public tiposMetodoPago MetodoPago { get; set; }

        [EmailAddress]
        [Required]
        public string NombreCliente { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduce tu Nombre y Apellido")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Nombre y Apellido deben tener al menos 10 caracteres")]
        public string ApellidoCliente { get; set; }

        public IList<ReparacionItemDTO> ReparacionItems { get; set; }


        [Display(Name = "Precio Total")]
        [JsonPropertyName("PrecioTotal")]
        public double PrecioTotal
        {
            get
            {
                return ReparacionItems.Sum(ri => ri.Precio * ri.Cantidad);
            }
        }


        public override bool Equals(object? obj)
        {
            return obj is ReparacionCreateDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   ReparacionItems.SequenceEqual(dTO.ReparacionItems) &&
                   PrecioTotal == dTO.PrecioTotal;
        }
    }
}
