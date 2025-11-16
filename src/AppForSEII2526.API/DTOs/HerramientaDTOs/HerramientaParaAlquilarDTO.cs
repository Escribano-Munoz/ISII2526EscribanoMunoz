namespace AppForSEII2526.API.DTOs.HerramientaDTOs
{
    public class HerramientaParaAlquilarDTO
    {
        public HerramientaParaAlquilarDTO()
        {
        }

        public HerramientaParaAlquilarDTO(string fabricante, string material, string nombre, float precio)
        {
            Fabricante = fabricante;
            Material = material;
            Nombre = nombre;
            Precio = precio;
        }

        public HerramientaParaAlquilarDTO(int id, string fabricante, string material, string nombre, float precio)
                : this(fabricante, material, nombre, precio)
        {
            Id = id;

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Fabricante { get; set; }

        [Required]
        public string Material { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]

        [Display(Name = "Precio")]
        public float Precio { get; set; }


        public IList<AlquilarItem> AlquilarItems { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is HerramientaParaAlquilarDTO dTO &&
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
