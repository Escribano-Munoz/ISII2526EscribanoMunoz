using Humanizer.Localisation;

namespace AppForSEII2526.API.DTOs.HerramientaDTOs
{
    public class HerramientaParaRepararDTO
    {
        public HerramientaParaRepararDTO()
        {
        }

        public HerramientaParaRepararDTO(string fabricante, string material, string nombre, float precio, int tiempoReparacion)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            Precio = precio;
            TiempoReparacion = tiempoReparacion;
        }

        public HerramientaParaRepararDTO(int id, string fabricante, string material, string nombre, float precio, int tiempoReparacion)
                : this(fabricante, material, nombre, precio, tiempoReparacion)
        {
            Id = id;

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Fabricante { get; set; }

        [Required]
        public string Material { get; set; }
        public string Nombre { get; set; }
        public int TiempoReparacion { get; set; }

        [Required]

        [Display(Name = "Precio")]
        public float Precio { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is HerramientaParaRepararDTO dTO &&
                Fabricante == dTO.Fabricante &&
                Id == dTO.Id &&
                Precio == dTO.Precio;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Fabricante, Precio);
        }
    }
}
