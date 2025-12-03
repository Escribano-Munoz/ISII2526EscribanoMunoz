namespace AppForSEII2526.API.DTOs.AlquilerDTOs
{
    public class AlquilerCreateDTO
    {
        public AlquilerCreateDTO(string nombreCliente, string apellidoCliente, string direccionEnvio, DateTime fechaInicio, DateTime fechaFin, IList<AlquilarItemDTO> alquilarItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            DireccionEnvio = direccionEnvio ?? throw new ArgumentNullException(nameof(direccionEnvio));
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            AlquilarItems = alquilarItems ?? throw new ArgumentNullException(nameof(alquilarItems));
        }

        public AlquilerCreateDTO(string nombreCliente, string apellidoCliente, string direccionEnvio, DateTime fechaInicio, DateTime fechaFin, TiposMetodoPago metodoPago, IList<AlquilarItemDTO> alquilarItems)
        {
            NombreCliente = nombreCliente ?? throw new ArgumentNullException(nameof(nombreCliente));
            ApellidoCliente = apellidoCliente ?? throw new ArgumentNullException(nameof(apellidoCliente));
            DireccionEnvio = direccionEnvio ?? throw new ArgumentNullException(nameof(direccionEnvio));
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            MetodoPago = metodoPago;
            AlquilarItems = alquilarItems ?? throw new ArgumentNullException(nameof(alquilarItems));
        }

        public AlquilerCreateDTO()
        {
            AlquilarItems = new List<AlquilarItemDTO>();
        }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }


        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Direccion de Envío")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "La direccion de envio debe tener al menos 10 caracteres")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduce tu direccion para el envio")]
        public string DireccionEnvio { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, pon tu nombre")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nombre debe contener al menos 3 caracteres")]
        public string NombreCliente { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, pon tu apellido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Apellido debe contener al menos 3 caracteres")]
        public string ApellidoCliente { get; set; }

        public TiposMetodoPago MetodoPago { get; set; }

        public IList<AlquilarItemDTO> AlquilarItems { get; set; }

        private int NumeroDeDias
        {
            get
            {
                return (FechaFin - FechaInicio).Days;
            }
        }

        [Display(Name = "Precio Total")]
        [JsonPropertyName("PrecioTotal")]
        public double PrecioTotal
        {
            get
            {
                return AlquilarItems.Sum(ai => ai.Precio * NumeroDeDias);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerCreateDTO dTO &&
                   CompareDate(FechaInicio, dTO.FechaInicio) &&
                   CompareDate(FechaFin, dTO.FechaFin) &&
                   DireccionEnvio == dTO.DireccionEnvio &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   AlquilarItems.SequenceEqual(dTO.AlquilarItems) &&
                   PrecioTotal == dTO.PrecioTotal;
        }
    }
}
