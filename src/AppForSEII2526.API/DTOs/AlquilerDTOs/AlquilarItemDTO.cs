namespace AppForSEII2526.API.DTOs.AlquilerDTOs
{
    public class AlquilarItemDTO
    {
        public AlquilarItemDTO(int herramientaID, string nombre, string material, double precio, int cantidad)
        {
            HerramientaID = herramientaID;
            Nombre = nombre;
            Precio = precio;
            Material = material;
            Cantidad = cantidad;
        }

        public int HerramientaID { get; set; }


        public string Nombre { get; set; }


        public double Precio { get; set; }

        public string Material { get; set; }

        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AlquilarItemDTO dTO &&
                   HerramientaID == dTO.HerramientaID &&
                   Nombre == dTO.Nombre &&
                   Precio == dTO.Precio &&
                   Material == dTO.Material &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HerramientaID, Nombre, Precio, Material, Cantidad);
        }
    }
}
