namespace TiendaAPI.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public string EmailCliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public List<DetalleOrden> Detalles { get; set; } = new();
    }
}