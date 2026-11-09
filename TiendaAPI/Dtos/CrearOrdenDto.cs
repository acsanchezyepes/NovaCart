using System.ComponentModel.DataAnnotations;

namespace TiendaAPI.Dtos
{
    // Datos que el cliente puede enviar al crear una orden.
    // Total, Estado, Fecha y PrecioUnitario no se aceptan: los fija el servidor.
    public class CrearOrdenDto
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        [MaxLength(254, ErrorMessage = "El email es demasiado largo.")]
        public string EmailCliente { get; set; } = string.Empty;

        public List<CrearDetalleOrdenDto> Detalles { get; set; } = new();
    }

    public class CrearDetalleOrdenDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
