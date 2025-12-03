namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraCreateDTO
    {
        public CompraCreateDTO(string nombreCliente, string apellidoCliente,
            string direccionEnvio, DateTime fechaCompra,
            IList<CompraItemDTO> compraItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            DireccionEnvio = direccionEnvio ?? throw new ArgumentNullException(nameof(direccionEnvio));
            FechaCompra = fechaCompra;
            CompraItems = compraItems ?? throw new ArgumentNullException(nameof(compraItems));
        }

        public CompraCreateDTO()
        {
            CompraItems = new List<CompraItemDTO>();
        }

        public CompraCreateDTO(string nombreCliente, string apellidoCliente,
            string direccionEnvio, DateTime fechaCompra, TiposMetodoPago metodoPago,
            IList<CompraItemDTO> compraItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            DireccionEnvio = direccionEnvio ?? throw new ArgumentNullException(nameof(direccionEnvio));
            FechaCompra = fechaCompra;
            MetodoPago = metodoPago;
            CompraItems = compraItems ?? throw new ArgumentNullException(nameof(compraItems));
        }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Direccion de Envio")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Direccion de envio debe tener al menos 10 caracteres")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su direccion para envio")]
        public string DireccionEnvio { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su Nombre")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nombre debe tener al menos 3 caracteres")]
        public string NombreCliente { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su Apellido")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Apellido debe tener al menos 4 caracteres")]
        public string ApellidoCliente { get; set; }

        public TiposMetodoPago MetodoPago { get; set; }

        public IList<CompraItemDTO> CompraItems { get; set; }
        [Required]

        public DateTime FechaCompra { get; set; }


        [Display(Name = "Precio Total")]
        [JsonPropertyName("PrecioTotal")]
        public double PrecioTotal
        {
            get
            {
                return CompraItems.Sum(ci => ci.Precio * ci.Cantidad);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is CompraCreateDTO dTO &&
                   DireccionEnvio == dTO.DireccionEnvio &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   CompraItems.SequenceEqual(dTO.CompraItems) &&
                   PrecioTotal == dTO.PrecioTotal;
        }
    }
}
