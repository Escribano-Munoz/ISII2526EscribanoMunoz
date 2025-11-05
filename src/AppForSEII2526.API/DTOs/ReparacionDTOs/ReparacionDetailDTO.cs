namespace AppForSEII2526.API.DTOs.ReparacionDTOs
{
    public class ReparacionDetailDTO : ReparacionCreateDTO
    {
        public ReparacionDetailDTO(int id, string nombreCliente, string apellidoCliente, DateTime fechaRecogida, 
            DateTime fechaEntrega, IList<ReparacionItemDTO> reparacionItems)
            : base(
                   nombreCliente,
                   apellidoCliente,
                   fechaRecogida,
                   fechaEntrega,
                   reparacionItems
                  )
        {
            Id = id;
        }
        public int Id { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is ReparacionDetailDTO dTO &&
                   base.Equals(obj) &&
                   PrecioTotal == dTO.PrecioTotal &&
                   Id == dTO.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id);
        }
    }
}
