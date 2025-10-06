namespace AppForSEII2526.API.Models
{
    public class Alquiler
    {
        public Alquiler()
        {
        }

        public Alquiler(DateTime fechaAlquiler, DateTime fechaInicio, DateTime fechaFin, string direccionEnvio, TiposMetodoPago metodoPago, string nombreCliente, string apellidoCliente, IList<AlquilarItem> alquilarItems)
        {
            PrecioTotal = alquilarItems.Sum(ai => ai.Precio * (fechaInicio - fechaFin).Days);
           
            FechaAlquiler = fechaAlquiler;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            DireccionEnvio = direccionEnvio;
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            AlquilarItems = alquilarItems;
            MetodoPago = metodoPago;
        }

        public Alquiler(string numeroTelefono, string correo, double precioTotal, DateTime fechaAlquiler, DateTime fechaInicio, DateTime fechaFin, string direccionEnvio, TiposMetodoPago metodoPago, string nombreCliente, string apellidoCliente, IList<AlquilarItem> AlquilarItems)
            : this(fechaAlquiler, fechaInicio, fechaFin, direccionEnvio, metodoPago, nombreCliente, apellidoCliente, AlquilarItems)
        {
            NumeroTelefono = numeroTelefono;
            Correo = correo;

        }


        [Key]
        public int Id { get; set; }

        public double PrecioTotal { get; set; }

        public DateTime FechaAlquiler { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        [Display(Name = "Direccion de envio")]
        [DisplayFormat(DataFormatString = "{0:C/CALLE, PROVINCIA}", ApplyFormatInEditMode = true)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, Introduzca su direccion de envio")]
        public string DireccionEnvio { get; set; }

        [Display(Name = "Metodo de pago")]
        [Required]
        public TiposMetodoPago MetodoPago { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, Introduzca su nombre")]
        public string NombreCliente { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, Introduzca sus apellidos")]
        public string ApellidoCliente { get; set; }

        public string Correo { get; set; }

        public string NumeroTelefono {  get; set; }

        public int Periodo { get; set; }

        public IList<AlquilarItem> AlquilarItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Alquiler alquiler &&
                   Id == alquiler.Id &&
                   PrecioTotal == alquiler.PrecioTotal &&
                   FechaInicio.Subtract(alquiler.FechaInicio) < TimeSpan.FromMinutes(2) &&
                   FechaInicio == alquiler.FechaInicio &&
                   FechaFin == alquiler.FechaFin &&
                   DireccionEnvio == alquiler.DireccionEnvio &&
                   NombreCliente == alquiler.NombreCliente &&
                   ApellidoCliente == alquiler.ApellidoCliente &&
                   MetodoPago == alquiler.MetodoPago &&
                   AlquilarItems.SequenceEqual(alquiler.AlquilarItems);

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, PrecioTotal, FechaAlquiler, FechaInicio, FechaFin, ApellidoCliente, NombreCliente, MetodoPago);
        }

        public enum TiposMetodoPago
        {
            TarjetaCredito,
            PayPal,
            Efectivo
        }


    }
}
