namespace TiendaAPI.Models
{
    // Estados posibles de una orden. Solo una orden Pendiente se puede pagar.
    public static class EstadosOrden
    {
        public const string Pendiente = "Pendiente";
        public const string Pagado = "Pagado";
        public const string Rechazado = "Rechazado";
    }
}
