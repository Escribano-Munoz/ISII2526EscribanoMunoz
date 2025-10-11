namespace AppForSEII2526.API.Models
{
    public class Oferta
    {
        [Required]
        public DateTime fechaFinal { get; set; }
        [Required]
        public DateTime fechaInicio { get; set; }
        public DateTime fechaOferta { get; set; }
        public int Id { get; set; }

        [Required]
        public tiposMetodoPago metodoPago { get; set; }
        
        public tiposDirigidaOferta ? paraSocio { get; set; }

        public IList<OfertaItem> OfertaItems { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Oferta()
        {
        }

        public Oferta(int id, DateTime fechaInicio, DateTime fechaFinal, DateTime fechaOferta, IList<OfertaItem> OfertaItems) :
            this(fechaInicio, fechaFinal, fechaOferta,OfertaItems)
        {
            Id = id;
        }

        public Oferta(DateTime fechaInicio, DateTime fechaFinal, DateTime fechaOferta, IList<OfertaItem> ofertaItems)
        {
            this.fechaInicio = fechaInicio;
            this.fechaFinal = fechaFinal;
            this.fechaOferta = fechaOferta;
            OfertaItems = ofertaItems;
        }
    }

    public enum tiposMetodoPago
    {
        TarjetaCredito,
        PayPal,
        Efectivo
    }

    public enum tiposDirigidaOferta
    {
        Socios,
        Clientes
    }

}
