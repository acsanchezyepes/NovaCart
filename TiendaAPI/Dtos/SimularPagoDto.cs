using System.ComponentModel.DataAnnotations;

namespace TiendaAPI.Dtos
{
    // Resultado del pago simulado. No se pide ni se guarda ningún dato de tarjeta.
    public class SimularPagoDto
    {
        [Required(ErrorMessage = "Indica si el pago simulado se aprueba (true) o se rechaza (false).")]
        public bool? Aprobado { get; set; }
    }
}
