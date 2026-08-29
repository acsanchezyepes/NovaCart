using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaAPI.Data;
using TiendaAPI.Models;

namespace TiendaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Inyección de dependencias: .NET nos entrega el AppDbContext listo para usar
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Get([FromQuery] string? categoria)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => p.Categoria.ToLower() == categoria.ToLower());
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/productos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado" });
            }

            return Ok(producto);
        }
    }
}