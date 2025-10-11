namespace AppForSEII2526.API.Models
{
    public class ReparacionItem
    {
        public ReparacionItem()
        {
        }

        public ReparacionItem(Herramienta herramienta, int cantidad, Reparacion reparacion)
        {
            Herramienta = herramienta;
            idHerramienta = herramienta.Id;
            Reparacion = reparacion;
            idReparacion = reparacion.Id;
            precio = herramienta.Precio; 
            cantidad = cantidad;
        }

        public int idReparacion { get; set; }

        public int idHerramienta { get; set; }

        public string? descripcion { get; set; }

        public float precio { get; set; }

        [Required]
        public int cantidad { get; set; }

        public Herramienta Herramienta { get; set; }

        public Reparacion Reparacion { get; set; }

        public ReparacionItem(int idReparacion, int idHerramienta, string descripcion, float precio, int cantidad)
        {
            this.idReparacion = idReparacion;
            this.idHerramienta = idHerramienta;
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
