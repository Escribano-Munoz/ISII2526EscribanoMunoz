namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraItemDTO
    {
        public CompraItemDTO(int herramientaID, string nombre, string material, double precio, int cantidad, string descripcion = "")
        {
            HerramientaID = herramientaID;
            Nombre = nombre;
            Material = material;
            Precio = precio;
            Cantidad = cantidad;
            Descripcion = descripcion;
        }

        public int HerramientaID { get; set; }

        public string Nombre { get; set; }

        public string Material { get; set; }

        public double Precio { get; set; }

        public string? Descripcion { get; set; }

        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraItemDTO dTO &&
                   HerramientaID == dTO.HerramientaID &&
                   Nombre == dTO.Nombre &&
                   Material == dTO.Material &&
                   Precio == dTO.Precio &&
                   Cantidad == dTO.Cantidad &&
                   Descripcion == dTO.Descripcion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HerramientaID, Nombre, Material, Precio, Cantidad, Descripcion);
        }
    }
}
