namespace AppForSEII2526.API.Models
{
    public class Reparacion
    {
        public Reparacion()
        {
            ReparacionItems = new List<ReparacionItem>();
        }

        public Reparacion(int id, IList<ReparacionItem> reparacionItems, DateTime fechaRecogida, DateTime fechaEntrega, float precioTotal, tiposMetodoPago metodoPago, ApplicationUser applicationUser) :
            this(reparacionItems, fechaRecogida, fechaEntrega, precioTotal, metodoPago, applicationUser)
        {
            Id = id;
        }

        public Reparacion(IList<ReparacionItem> reparacionItems, DateTime fechaRecogida, DateTime fechaEntrega, float precioTotal, tiposMetodoPago metodoPago, ApplicationUser applicationUser)
        {
            ReparacionItems = reparacionItems;
            FechaRecogida = fechaRecogida;
            FechaEntrega = fechaEntrega;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            ApplicationUser = applicationUser;
        }
        public int Id { get; set; }

        public float PrecioTotal { get; set; }
        public tiposMetodoPago MetodoPago { get; set; }
        [Required]
        public DateTime FechaRecogida { get; set; }

        [Required]
        public DateTime FechaEntrega { get; set; }

        public IList<ReparacionItem> ReparacionItems { get; set; }


        public ApplicationUser ApplicationUser { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public enum tiposMetodosPago
    {
        TarjetaCredito,
        PayPal,
        Efectivo
    }
}

