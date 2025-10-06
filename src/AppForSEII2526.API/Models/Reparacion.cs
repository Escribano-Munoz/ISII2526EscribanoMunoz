namespace AppForSEII2526.API.Models
{
    public class Reparacion
    {
        public Reparacion()
        {
            ReparacionItems = new List<ReparacionItem>();
        }

        public Reparacion(int id, string nombreCliente, string apellidoCliente, string numTelefono, IList<ReparacionItem> reparacionItems,
            DateTime fechaRecogida, DateTime fechaEntrega, float precioTotal, tiposMetodosPago metodoPago) :
            this(nombreCliente, apellidoCliente, numTelefono, reparacionItems, fechaRecogida, fechaEntrega, precioTotal, metodoPago)
        {
            Id = id;
        }

        public Reparacion(string nombreCliente, string apellidoCliente, string numTelefono, IList<ReparacionItem> reparacionItems,
            DateTime fechaRecogida, DateTime fechaEntrega, float precioTotal, tiposMetodosPago metodoPago)
        {
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            NumTelefono = numTelefono;
            ReparacionItems = reparacionItems;
            FechaRecogida = fechaRecogida;
            FechaEntrega = fechaEntrega;
            PrecioTotal = precioTotal;
        }
        public int Id { get; set; }

        [Precision(10, 2)]
        public float PrecioTotal { get; set; }

        public DateTime FechaRecogida { get; set; }

        [Required]
        public DateTime FechaEntrega { get; set; }

        public IList<ReparacionItem> ReparacionItems { get; set; }

        [Display(Name = "Nombre del Cliente")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese el nombre del cliente")]
        public string NombreCliente { get; set; }

        [Display(Name = "Apellido del Cliente")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, ingrese el apellido del cliente")]
        public string ApellidoCliente { get; set; }

        [Display(Name = "Número de Teléfono")]
        public string NumTelefono { get; set; }

        [Display(Name = "Metodo de pago")]
        [Required]
        public tiposMetodosPago MetodoPago { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public enum tiposMetodosPago
        {
            TarjetaCredito,
            PayPal,
            Efectivo
        }
    }
}
