namespace AppForSEII2526.API.Models
{
    [PrimaryKey("herramientaId", "compraId")]
    public class CompraItem
    {
        public CompraItem()
        {
        }

        public CompraItem(Herramienta herramienta, Compra compra, int compraId, int herramientaId, int cantidad, string descripcion)
        {
            Herramienta = herramienta;
            Compra = compra;
            this.compraId = compraId;
            this.herramientaId = herramientaId;
            Cantidad = cantidad;
            Precio = herramienta.Precio;
            Descripcion = descripcion;
        }

        public CompraItem(Compra compra, int herramientaId, float precio, int cantidad, string descripcion)
        {
            Compra = compra;
            herramientaId = herramientaId;
            Precio = precio;
            Cantidad = cantidad;
            Descripcion = descripcion;
        }

        public CompraItem(Herramienta herramienta, Compra compra)
        {
            Herramienta = herramienta;
            Compra = compra;
            herramientaId = herramienta.Id;
            this.compraId = compra.Id;

        }

        public Herramienta Herramienta { get; set; }

        public Compra Compra { get; set; }

        public int compraId { get; set; }

        public int herramientaId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        public float Precio { get; set; }

        [Required]
        public string Descripcion { get; set; }

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
