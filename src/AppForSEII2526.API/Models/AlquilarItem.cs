namespace AppForSEII2526.API.Models
{
    public class AlquilarItem
    {
        public AlquilarItem()
        {
        }

        public AlquilarItem(Herramienta herramienta, Alquiler alquiler)
        {
            Herramienta = herramienta;
            HerramientaId = herramienta.Id;
            Alquiler = alquiler;
            AlquilerId = alquiler.Id;
            Precio = herramienta.Precio;

        }

        public AlquilarItem(int herramientaId, Alquiler alquiler, double precio, int cantidad)
        {
            HerramientaId = herramientaId;
            Alquiler = alquiler;
            Precio = precio;
            Cantidad = cantidad;
        }

        [Key]
        public int Id { get; set; }
        public Herramienta Herramienta { get; set; }

        public int HerramientaId { get; set; }

        public Alquiler Alquiler { get; set; }

        public int AlquilerId { get; set; }

        public double Precio { get; set; }

        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AlquilarItem linea &&
                   EqualityComparer<Herramienta>.Default.Equals(Herramienta, linea.Herramienta) &&
                   Precio == linea.Precio &&
                   HerramientaId == linea.HerramientaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HerramientaId, AlquilerId);
        }
    }
}
