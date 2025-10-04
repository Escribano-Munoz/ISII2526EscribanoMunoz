namespace AppForSEII2526.API.Models
{
    public class Herramienta
    {
        public Herramienta()
        {
        }

        public Herramienta(Fabricante fabricante, string material, string nombre, float precioCompra, float precioAlquiler)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            PrecioCompra = precioCompra;
            PrecioAlquiler = precioAlquiler;
        }

        public Herramienta(int id, Fabricante fabricante, string material, string nombre, float precioCompra, float precioAlquiler)
                : this(fabricante, material, nombre,  precioCompra, precioAlquiler)
        {
            Id = id;

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Fabricante Fabricante { get; set; }

        [Required]
        public string Material { get; set; }
        public string Nombre{ get; set; }
        public int TiempoReparacion { get; set; }

        [Required]

        [Display(Name = "Precio de Compra")]
        public float PrecioCompra { get; set; }

        [Display(Name = "Precio de Alquiler")]
        public float PrecioAlquiler { get; set; }




        public IList<AlquilarItem> AlquilarItems { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is Herramienta herramienta &&
                Fabricante == herramienta.Fabricante &&
                Id == herramienta.Id &&
                PrecioCompra == herramienta.PrecioCompra &&
                PrecioAlquiler == herramienta.PrecioAlquiler;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Fabricante, PrecioCompra, PrecioAlquiler);
        }
    }
}
