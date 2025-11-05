namespace AppForSEII2526.API.DTOs.ReparacionDTOs
{
    public class ReparacionItemDTO
    {
        public ReparacionItemDTO(int herramientaID, string nombre, double precio, int cantidad, string descripcion = "")
        {
            HerramientaID = herramientaID;
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
            Descripcion = descripcion;
        }

        public int HerramientaID { get; set; }


        public string Nombre { get; set; }


        public double Precio { get; set; }

        public string? Descripcion { get; set; }

        public int Cantidad { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReparacionItemDTO dTO &&
                   HerramientaID == dTO.HerramientaID &&
                   Nombre == dTO.Nombre &&
                   Precio == dTO.Precio &&
                   Cantidad == dTO.Cantidad &&
                   Descripcion == dTO.Descripcion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HerramientaID, Nombre, Precio, Cantidad, Descripcion);
        }
    }
}
