namespace AppForSEII2526.API.Models
{
    public class Herramienta
    {
        public Herramienta()
        {
        }

        public Herramienta(Fabricante fabricante, string material, string nombre, float precio)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            Precio = precio;
        }

        public Herramienta(int id, Fabricante fabricante, string material, string nombre, float precio)
                : this(fabricante, material, nombre, precio)
        {
            Id = id;

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Fabricante Fabricante { get; set; }

        [Required]
        public string Material { get; set; }
        public string Nombre { get; set; }
        public int TiempoReparacion { get; set; }

        [Required]

        [Display(Name = "Precio")]
        public float Precio { get; set; }


        public IList<AlquilarItem> AlquilarItems { get; set; }

        public IList<CompraItem> CompraItems { get; set; }

        public IList<ReparacionItem> ReparacionItems { get; set; }

        public IList<OfertaItem> OfertaItems { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is Herramienta herramienta &&
                Fabricante == herramienta.Fabricante &&
                Id == herramienta.Id &&
                Precio == herramienta.Precio;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Fabricante, Precio);
        }
    }
}