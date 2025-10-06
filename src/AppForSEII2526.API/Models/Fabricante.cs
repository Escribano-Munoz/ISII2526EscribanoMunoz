namespace AppForSEII2526.API.Models
{
    public class Fabricante
    {
        public Fabricante()
        {
        }

        public Fabricante(string nombre)
        {
            this.nombre = nombre;
        }

        public int id { get; set; }
        public string nombre { get; set; }

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
