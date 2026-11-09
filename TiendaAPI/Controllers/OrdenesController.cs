using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAPI.Data;
using TiendaAPI.Dtos;
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

        // GET: api/ordenes/5 (Consultar una orden concreta)
        // No hay listado general para no exponer los emails de todos los clientes.
        [HttpGet("{id}")]
        public async Task<ActionResult<Orden>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
            {
                return NotFound(new { mensaje = "Orden no encontrada" });
            }

            return Ok(orden);
        }

        // POST: api/ordenes (Crear una nueva compra/orden)
        [HttpPost]
        public async Task<ActionResult<Orden>> PostOrden(CrearOrdenDto dto)
        {
            if (dto.Detalles == null || dto.Detalles.Count == 0)
            {
                return BadRequest("La orden debe contener al menos un producto.");
            }

            // El servidor fija precio, total, estado y fecha: el cliente solo elige productos y cantidades.
            var orden = new Orden
            {
                EmailCliente = dto.EmailCliente.Trim(),
                Estado = "Pendiente",
                Fecha = DateTime.UtcNow,
            };

            foreach (var item in dto.Detalles)
            {
                if (item.Cantidad <= 0)
                {
                    return BadRequest($"La cantidad del producto con ID {item.ProductoId} debe ser mayor que 0.");
                }

                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"El producto con ID {item.ProductoId} no existe.");
                }

                orden.Detalles.Add(new DetalleOrden
                {
                    ProductoId = producto.Id,
                    Producto = producto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio,
                });
                orden.Total += producto.Precio * item.Cantidad;
            }

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrden), new { id = orden.Id }, orden);
        }
    }
}
