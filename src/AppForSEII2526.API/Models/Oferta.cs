namespace AppForSEII2526.API.Models
{
    public class Oferta
    {
        public DateTime fechaFinal { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaOferta { get; set; }
        public int Id { get; set; }
        public tiposMetodoPago metodoPago { get; set; }

        public tiposDirigidaOferta paraSocio { get; set; }

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

        public Oferta(int id, DateTime fechaInicio, DateTime fechaFinal, DateTime fechaOferta) :
            this(fechaInicio, fechaFinal, fechaOferta)
        {
            Id = id;
        }

        public Oferta(DateTime fechaInicio, DateTime fechaFinal, DateTime fechaOferta)
        {
            this.fechaInicio = fechaInicio;
            this.fechaFinal = fechaFinal;
            this.fechaOferta = fechaOferta;
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
