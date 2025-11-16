namespace AppForSEII2526.API.Models
{
    [PrimaryKey("HerramientaId", "OfertaId")]
    public class OfertaItem
    {
        public int HerramientaId { get; set; }
        public int OfertaId { get; set; }

        public Oferta oferta { get; set; }

        public Herramienta herramienta { get; set; }

        [Required]
        public decimal porcentaje { get; set; }
        public decimal precioFinal { get; set; }
        public decimal precioOriginal { get; set; }

        public OfertaItem()
        {
        }

        public OfertaItem(Herramienta herramienta, Oferta oferta,decimal porcentaje, decimal precioFinal)
        {
            this.herramienta = herramienta;
            this.oferta = oferta;
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
