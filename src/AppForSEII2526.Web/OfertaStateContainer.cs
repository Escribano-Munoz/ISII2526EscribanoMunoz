using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class OfertaStateContainer
    {
        public CrearOfertasCreateDTO Oferta { get; private set; } = new CrearOfertasCreateDTO()
        {
            OfertaItems = new List<OfertaItemDTO>()
        };

        public double PrecioTotalFinal
        {
            get
            {
                return Oferta.PrecioTotalFinal;
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddHerramientaParaOfertar(HerramientaParaCrearOfertaDTO herramienta, int porcentajeDescuento)
        {
            if (!Oferta.OfertaItems.Any(oi => oi.Nombre == herramienta.Nombre))
            {
                decimal precioFinal = (decimal)herramienta.Precio * (1 - (decimal)(porcentajeDescuento / 100.0));

                Oferta.OfertaItems.Add(new OfertaItemDTO()
                {
                    Nombre = herramienta.Nombre,
                    Material = herramienta.Material,
                    Fabricante = herramienta.Fabricante.Nombre,
                    PrecioOriginal = (double)herramienta.Precio,
                    PrecioFinal = (double)precioFinal,
                    PorcentajeDescuento = porcentajeDescuento
                });
                NotifyStateChanged();
            }
        }

        public void RemoveOfertaItem(OfertaItemDTO item)
        {
            Oferta.OfertaItems.Remove(item);
            NotifyStateChanged();
        }

        public void ClearOfertaCart()
        {
            Oferta.OfertaItems.Clear();
            NotifyStateChanged();
        }

        public void OfertaProcessed()
        {
            Oferta = new CrearOfertasCreateDTO()
            {
                OfertaItems = new List<OfertaItemDTO>()
            };
            NotifyStateChanged();
        }
    }
}