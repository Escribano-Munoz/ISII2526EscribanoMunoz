namespace AppForSEII2526.API.DTOs.HerramientaDTOs
{
    public class HerramientaParaCrearOfertaDTO
    {
        public HerramientaParaCrearOfertaDTO()
        {
        }

        public HerramientaParaCrearOfertaDTO(Fabricante fabricante, string material, string nombre, float precio)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            Precio = precio;
        }

        public HerramientaParaCrearOfertaDTO(int id, Fabricante fabricante, string material, string nombre, float precio)
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

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public float Precio { get; set; }


        


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