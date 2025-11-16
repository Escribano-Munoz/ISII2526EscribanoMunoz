namespace AppForSEII2526.API.Models
{
    [PrimaryKey("herramientaId", "ofertaId")]
    public class OfertaItem
    {
        public int herramientaId { get; set; }
        public int ofertaId { get; set; }

        public Oferta oferta { get; set; }

        public Herramienta herramienta { get; set; }
        [Required]
        public decimal porcentaje { get; set; }
        public decimal precioFinal { get; set; }
        public decimal precioOriginal { get; set; }

        public OfertaItem()
        {
        }

        public OfertaItem(int idHerramienta, int idOferta, decimal porcentaje, decimal precioFinal, Oferta oferta, Herramienta herramienta) :
            this(idOferta, porcentaje, precioFinal, oferta, herramienta)
        {
            this.herramientaId = idHerramienta;
            this.oferta = oferta;
            this.herramienta = herramienta;
        }

        public OfertaItem(int idOferta, decimal porcentaje, decimal precioFinal, Oferta oferta, Herramienta herramienta)
        {
            this.ofertaId = idOferta;
            this.porcentaje = porcentaje;
            this.precioFinal = precioFinal;
            this.oferta = oferta;
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