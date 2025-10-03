namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra()
        {

        }
        public Compra()
        {
            CompraItems = new List<CompraItem>();
        }

        public Compra(int id, string nombreCliente, string apellidoCliente, DateTime fechaCompra, string direccionEnvio, List<CompraItem> compraItems, string telefono, string correoElectronico, tiposMetodoPago metodoPago)
        {
            Id = id;
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            FechaCompra = fechaCompra;
            DireccionEnvio = direccionEnvio;
            CompraItems = compraItems;
            Telefono = telefono;
            CorreoElectronico = correoElectronico;
            MetodoPago = metodoPago;
        }

        public int Id { get; set; }

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


    }
}
