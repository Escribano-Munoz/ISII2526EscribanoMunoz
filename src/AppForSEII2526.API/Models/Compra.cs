using System.Collections.Generic;

namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra()
        {
            CompraItems = new List<CompraItem>();
        }

        public Compra(int id, string nombreCliente, string apellidoCliente, DateTime fechaCompra, string direccionEnvio, IList<CompraItem> compraItems, string telefono, string correoElectronico, TiposMetodoPago metodoPago) :
            this(nombreCliente, apellidoCliente, fechaCompra, direccionEnvio, compraItems, telefono, correoElectronico, metodoPago)
        {
            Id = id;
            
        }

        public Compra(string nombreCliente, string apellidoCliente, DateTime fechaCompra, string direccionEnvio, IList<CompraItem> compraItems, string telefono, string correoElectronico, TiposMetodoPago metodoPago)
        {
            precioTotal = Math.Round(CompraItems.Sum(ci => (decimal)ci.Precio * ci.Cantidad), 2);

            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            FechaCompra = fechaCompra;
            DireccionEnvio = direccionEnvio;
            CompraItems = compraItems.ToList();
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

        public enum TiposMetodoPago
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
