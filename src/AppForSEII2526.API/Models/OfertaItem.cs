namespace AppForSEII2526.API.Models
{
    public class OfertaItem
    {
        public int idHerramienta { get; set; }
        public int idOferta { get; set; }
        public decimal porcentaje { get; set; }
        public decimal precioFinal { get; set; }

        public OfertaItem()
        {
        }

        public OfertaItem(int idHerramienta, int idOferta, decimal porcentaje, decimal precioFinal) :
            this(idOferta, porcentaje, precioFinal)
        {
            this.idHerramienta = idHerramienta;
        }

        public OfertaItem(int idOferta, decimal porcentaje, decimal precioFinal)
        {
            this.idOferta = idOferta;
            this.porcentaje = porcentaje;
            this.precioFinal = precioFinal;
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
