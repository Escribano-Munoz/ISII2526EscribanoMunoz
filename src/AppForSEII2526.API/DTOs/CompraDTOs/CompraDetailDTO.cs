namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraDetailDTO : CompraCreateDTO
    {
        public CompraDetailDTO(int id, string nombreCliente, string apellidoCliente,
            string direccionEnvio, DateTime fechaCompra, IList<CompraItemDTO> compraItems)
            : base(
                   nombreCliente,
                   apellidoCliente,
                   direccionEnvio,
                   compraItems)
        {
            Id = id;
            FechaCompra = fechaCompra;
        }
        public int Id { get; set; }

        public DateTime FechaCompra { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraDetailDTO dTO &&
                   base.Equals(obj) &&
                   PrecioTotal == dTO.PrecioTotal &&
                   Id == dTO.Id &&
                   CompareDate(FechaCompra, dTO.FechaCompra);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, FechaCompra);
        }
    }
}
