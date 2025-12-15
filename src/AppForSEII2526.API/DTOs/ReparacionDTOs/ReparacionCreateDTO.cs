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

        public ReparacionCreateDTO(string nombreCliente, string apellidoCliente, DateTime fechaRecogida,
            DateTime fechaEntrega, tiposMetodoPago metodoPago, IList<ReparacionItemDTO> reparacionItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            FechaRecogida = fechaRecogida;
            FechaEntrega = fechaEntrega;
            MetodoPago = metodoPago;
            ReparacionItems = reparacionItems ?? throw new ArgumentNullException(nameof(reparacionItems));
        }

        public ReparacionCreateDTO(string nombreCliente, string apellidoCliente, DateTime fechaRecogida,
            DateTime fechaEntrega, tiposMetodoPago metodoPago, string? telefono, IList<ReparacionItemDTO> reparacionItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            FechaRecogida = fechaRecogida;
            FechaEntrega = fechaEntrega;
            MetodoPago = metodoPago;
            Telefono = telefono;
            ReparacionItems = reparacionItems ?? throw new ArgumentNullException(nameof(reparacionItems));
        }

        public DateTime FechaRecogida { get; set; }

        public DateTime FechaEntrega { get; set; }

        public tiposMetodoPago MetodoPago { get; set; }

        public string? Telefono { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduce tu Nombre")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nombre debe tener al menos 3 caracteres")]
        public string NombreCliente { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduce tu Apellido")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Apellido debe tener al menos 4 caracteres")]
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
                   Telefono == dTO.Telefono &&
                   ReparacionItems.SequenceEqual(dTO.ReparacionItems) &&
                   PrecioTotal == dTO.PrecioTotal;
        }
    }
}
