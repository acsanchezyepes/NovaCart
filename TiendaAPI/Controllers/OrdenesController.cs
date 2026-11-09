using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAPI.Data;
using TiendaAPI.Models;

namespace TiendaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ordenes (Obtener el historial de compras)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orden>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        // POST: api/ordenes (Crear una nueva compra/orden)
        [HttpPost]
        public async Task<ActionResult<Orden>> PostOrden(Orden orden)
        {
            if (orden.Detalles == null || !orden.Detalles.Any())
            {
                return BadRequest("La orden debe contener al menos un producto.");
            }

            // Calcular el total automáticamente
            decimal totalCalculado = 0;
            foreach (var detalle in orden.Detalles)
            {
                if (detalle.Cantidad <= 0)
                {
                    return BadRequest($"La cantidad del producto con ID {detalle.ProductoId} debe ser mayor que 0.");
                }

                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"El producto con ID {detalle.ProductoId} no existe.");
                }

                detalle.PrecioUnitario = producto.Precio;
                totalCalculado += producto.Precio * detalle.Cantidad;
            }

            orden.Total = totalCalculado;
            orden.Fecha = DateTime.UtcNow;

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrdenes), new { id = orden.Id }, orden);
        }
    }
}