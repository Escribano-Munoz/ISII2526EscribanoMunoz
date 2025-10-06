namespace AppForSEII2526.API.Models
{
    public class CompraItem
    {
        public CompraItem()
        {
        }

        public CompraItem(Herramienta herramienta, Compra compra, int idCompra, int idHerramienta, int cantidad, string descripcion)
        {
            Herramienta = herramienta;
            Compra = compra;
            IdCompra = idCompra;
            IdHerramienta = idHerramienta;
            Cantidad = cantidad;
            Precio = herramienta.Precio;
            Descripcion = descripcion;
        }

        public Herramienta Herramienta { get; set; }

        public Compra Compra { get; set; }

        public int IdCompra { get; set; }

        public int IdHerramienta { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [Precision(10, 2)]
        public float Precio { get; set; }

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
