namespace AppForSEII2526.API.Models
{
    [PrimaryKey("herramientaId", "reparacionId")]
    public class ReparacionItem
    {
        public ReparacionItem()
        {
        }

        public ReparacionItem(Herramienta herramienta, int cantidad, Reparacion reparacion)
        {
            Herramienta = herramienta;
            Reparacion = reparacion;
            precio = herramienta.Precio;
            this.cantidad = cantidad;
        }

        public ReparacionItem(Herramienta herramienta, Reparacion reparacion)
        {
            this.Herramienta = herramienta;
            this.herramientaId = herramienta.Id;
            this.Reparacion = reparacion;
            this.reparacionId = reparacion.Id;
            this.precio = herramienta.Precio;

        }

        public int reparacionId { get; set; }

        public int herramientaId { get; set; }

        public string? descripcion { get; set; }

        public float precio { get; set; }

        [Required]
        public int cantidad { get; set; }

        public Herramienta Herramienta { get; set; }

        public Reparacion Reparacion { get; set; }

        public ReparacionItem(int idReparacion, int idHerramienta, string descripcion, float precio, int cantidad)
        {
            this.reparacionId = idReparacion;
            this.herramientaId = idHerramienta;
            this.descripcion = descripcion;
            this.precio = precio;
            this.cantidad = cantidad;
        }

        public ReparacionItem(string descripcion, float precio, int cantidad)
        {
            this.descripcion = descripcion;
            this.precio = precio;
            this.cantidad = cantidad;
        }

        public ReparacionItem(int herramientaId, int cantidad, string descripcion, float precio, Reparacion reparacion)
        {
            this.herramientaId = herramientaId;
            this.cantidad = cantidad;
            this.descripcion = descripcion;
            this.precio = precio;
            this.Reparacion = reparacion;
        }


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