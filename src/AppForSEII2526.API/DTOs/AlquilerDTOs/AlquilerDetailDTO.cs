namespace AppForSEII2526.API.DTOs.AlquilerDTOs
{
    public class AlquilerDetailDTO
    {
        public AlquilerDetailDTO(int id, DateTime fechaAlquiler, string nombreCliente, string apellidoCliente,
    string direccionEnvio, DateTime fechaInicio, DateTime fechaFin, IList<AlquilarItemDTO> alquilarItems)
    : base(nombreCliente,
           apellidoCliente,
           direccionEnvio,
           fechaInicio,
           fechaFin,
           alquilarItems)
        {
            Id = id;
            FechaAlquiler = fechaAlquiler;
        }
        public int Id { get; set; }

        public DateTime FechaAlquiler { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerDetailDTO dTO &&
                   base.Equals(obj) &&
                   PrecioTotal == dTO.PrecioTotal &&
                   Id == dTO.Id &&
                   CompareDate(FechaAlquiler, dTO.FechaAlquiler);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, FechaAlquiler);
        }
    }
}
