using System.Collections.Generic;

namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra()
        {
            CompraItems = new List<CompraItem>();
        }

        public Compra(int id, DateTime fechaCompra, IList<CompraItem> compraItems, ApplicationUser applicationUser) :
            this(fechaCompra, compraItems, applicationUser)
        {
            Id = id;

        }

        public Compra(DateTime fechaCompra, IList<CompraItem> compraItems, ApplicationUser applicationUser)
        {
            precioTotal = Math.Round(CompraItems.Sum(ci => (decimal)ci.Precio * ci.Cantidad), 2);

            FechaCompra = fechaCompra;
            CompraItems = compraItems.ToList();
            ApplicationUser = applicationUser;
        }


        public int Id { get; set; }

        [Precision(10, 2)]
        public decimal precioTotal { get; set; }

        public DateTime FechaCompra { get; set; }

        public List<CompraItem> CompraItems { get; set; }

        public ApplicationUser ApplicationUser { get; set; }



        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }


}
