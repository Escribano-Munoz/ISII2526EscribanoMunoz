namespace AppForSEII2526.API.Models
{
    public class Herramienta
    {
        public Herramienta()
        {
        }

        public Herramienta(Fabricante fabricante, double precioCompra, int cantidadCompra, double precioAlquiler, int cantidadAlquiler)
        {
            Fabricante = fabricante;
            PrecioCompra = precioCompra;
            CantidadCompra = cantidadCompra;
            PrecioAlquiler = precioAlquiler;
            CantidadAlquiler = cantidadAlquiler;
        }

        public Herramienta(int id, Fabricante fabricante, double precioCompra, int cantidadCompra, double precioAlquiler, int cantidadAlquiler)
                : this(fabricante, precioCompra, cantidadCompra, precioAlquiler, cantidadAlquiler)
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


        [Required]
        [DataType(DataType.Currency)]
        [Range(900, float.MaxValue, ErrorMessage = "El precio mínimo es 900")]
        [Display(Name = "Precio de Compra")]
        public double PrecioCompra { get; set; }
        [Range(35, 1000, ErrorMessage = "El precio debe estar entre 35 y 1000")]
        [Display(Name = "Precio de Alquiler")]
        public double PrecioAlquiler { get; set; }

        [Required]
        [Display(Name = "Cantidad para Comprar")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad minima es 1")]
        public float CantidadCompra { get; set; }

        [Required]
        [Display(Name = "Cantidad para Alquiler")]
        public float CantidadAlquiler { get; set; }

        public IList<AlquilarItem> AlquilarItems { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is Herramienta herramienta &&
                Fabricante == herramienta.Fabricante &&
                Id == herramienta.Id &&
                PrecioCompra == herramienta.PrecioCompra &&
                CantidadCompra == herramienta.CantidadCompra &&
                PrecioAlquiler == herramienta.PrecioAlquiler &&
                CantidadAlquiler == herramienta.CantidadAlquiler;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Fabricante, PrecioCompra, PrecioAlquiler, CantidadCompra, CantidadAlquiler);
        }
    }
}
