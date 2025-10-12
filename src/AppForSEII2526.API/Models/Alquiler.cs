namespace AppForSEII2526.API.Models
{
    public class Alquiler
    {
        public Alquiler()
        {
        }

        public Alquiler(DateTime fechaAlquiler, DateTime fechaInicio, DateTime fechaFin, IList<AlquilarItem> alquilarItems)
        {
            PrecioTotal = alquilarItems.Sum(ai => ai.Precio * (fechaInicio - fechaFin).Days);
           
            FechaAlquiler = fechaAlquiler;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            AlquilarItems = alquilarItems;
        }

        public Alquiler(ApplicationUser applicationUser, double precioTotal, DateTime fechaAlquiler, DateTime fechaInicio, DateTime fechaFin, string direccionEnvio, TiposMetodoPago metodoPago, string nombreCliente, string apellidoCliente, IList<AlquilarItem> AlquilarItems)
            : this(fechaAlquiler, fechaInicio, fechaFin, AlquilarItems)
        {

            ApplicationUser = applicationUser;

        }


        [Key]
        public int Id { get; set; }

        public double PrecioTotal { get; set; }

        public DateTime FechaAlquiler { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int Periodo { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public IList<AlquilarItem> AlquilarItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Alquiler alquiler &&
                   Id == alquiler.Id &&
                   PrecioTotal == alquiler.PrecioTotal &&
                   FechaInicio.Subtract(alquiler.FechaInicio) < TimeSpan.FromMinutes(2) &&
                   FechaInicio == alquiler.FechaInicio &&
                   FechaFin == alquiler.FechaFin &&
                   AlquilarItems.SequenceEqual(alquiler.AlquilarItems);

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, PrecioTotal, FechaAlquiler, FechaInicio, FechaFin);
        }



    }

    public enum TiposMetodoPago
    {
        TarjetaCredito,
        PayPal,
        Efectivo
    }
}
