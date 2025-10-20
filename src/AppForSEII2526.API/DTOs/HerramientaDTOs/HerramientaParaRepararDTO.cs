using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs.HerramientaDTOs
{
    public class HerramientaParaRepararDTO
    {
        public HerramientaParaRepararDTO()
        {
        }

        public HerramientaParaRepararDTO(Fabricante fabricante, string material, string nombre, float precio, int tiempoReparacion)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            Precio = precio;
            TiempoReparacion = tiempoReparacion;
        }

        public HerramientaParaRepararDTO(int id, Fabricante fabricante, string material, string nombre, float precio, int tiempoReparacion)
                : this(fabricante, material, nombre, precio, tiempoReparacion)
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


        public IList<ReparacionItem> ReparacionItems { get; set; }



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
