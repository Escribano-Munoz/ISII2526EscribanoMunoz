namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra()
        {
            CompraItems = new List<CompraItem>();
        }

        public Compra(int id, string nombreCliente, string apellidoCliente, ApplicationUser applicationUser, DateTime fechaCompra, string direccionEnvio, IList<CompraItem> compraItems, string telefono, string correoElectronico, tiposMetodoPago metodoPago) :
            this(nombreCliente, apellidoCliente,applicationUser, fechaCompra, direccionEnvio, compraItems,telefono, correoElectronico, metodoPago)
        {
            Id = id;
            
        }

        public Compra(string nombreCliente, string apellidoCliente, ApplicationUser applicationUser, DateTime fechaCompra, string direccionEnvio, IList<CompraItem> compraItems, string telefono, string correoElectronico, tiposMetodoPago metodoPago)
        {
            precioTotal = decimal.Round(compraItems.Sum(pi => pi.Precio * pi.Cantidad), 2);

            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            ApplicationUser = applicationUser;
            FechaCompra = fechaCompra;
            DireccionEnvio = direccionEnvio;
            CompraItems = compraItems;
            Telefono = telefono;
            CorreoElectronico = correoElectronico;
            MetodoPago = metodoPago;
        }


        public int Id { get; set; }

        [Precision(10, 2)]
        public decimal precioTotal { get; set; }

        public string NombreCliente { get;set; }

        public string ApellidoCliente { get; set; }

        public DateTime FechaCompra { get; set; }

        public string DireccionEnvio { get; set; }

        public List<CompraItem> CompraItems { get; set; }

        public string Telefono { get; set; }

        public string CorreoElectronico { get; set; }

        [Display(Name = "Metodo de pago")]
        [Required]
        public TiposMetodoPago MetodoPago { get; set; }

        public enum tiposMetodoPago
        {
            TarjetaCredito,
            PayPal,
            Efectivo
        }

         public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}
