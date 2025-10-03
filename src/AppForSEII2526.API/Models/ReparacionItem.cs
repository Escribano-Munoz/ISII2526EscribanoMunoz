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
            precio = herramienta.Precio; // Asumiendo que Herramienta tiene una propiedad Precio
            cantidad = cantidad;
        }

        // PROPIEDADES según la imagen
        public int idReparacion { get; set; }

        public int idHerramienta { get; set; }

        public string descripcion { get; set; }

        public float precio { get; set; }

        public int cantidad { get; set; }

        // Relaciones (no aparecen en la imagen pero son necesarias)
        public Herramienta Herramienta { get; set; }

        public Reparacion Reparacion { get; set; }

        // CONSTRUCTORES
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

        // MÉTODO Equals() : bool
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            ReparacionItem otroItem = (ReparacionItem)obj;

            // Comparar por claves primarias si están asignadas
            if (this.idReparacion != 0 && this.idHerramienta != 0 &&
                otroItem.idReparacion != 0 && otroItem.idHerramienta != 0)
            {
                return this.idReparacion == otroItem.idReparacion &&
                       this.idHerramienta == otroItem.idHerramienta;
            }

            // Si no, comparar por todos los campos
            return this.idReparacion == otroItem.idReparacion &&
                   this.idHerramienta == otroItem.idHerramienta &&
                   this.descripcion == otroItem.descripcion &&
                   this.precio == otroItem.precio &&
                   this.cantidad == otroItem.cantidad;
        }

        // MÉTODO GetHashCode()
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + idReparacion.GetHashCode();
                hash = hash * 23 + idHerramienta.GetHashCode();
                hash = hash * 23 + (descripcion?.GetHashCode() ?? 0);
                hash = hash * 23 + precio.GetHashCode();
                hash = hash * 23 + cantidad.GetHashCode();
                return hash;
            }
        }

        // MÉTODO adicional útil
        public override string ToString()
        {
            return $"{descripcion} - Cantidad: {cantidad} - Precio: {precio:C}";
        }
    }
}
