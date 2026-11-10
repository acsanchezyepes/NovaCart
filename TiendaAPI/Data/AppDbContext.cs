using Microsoft.EntityFrameworkCore;
using TiendaAPI.Models;

namespace TiendaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<DetalleOrden> DetallesOrden { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Catálogo inicial. Las fotos están en wwwroot/img/productos (créditos en wwwroot/creditos.html).
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado switch blue", Precio = 59.99m, Categoria = "Periféricos", ImagenUrl = "/img/productos/teclado.jpg" },
                new Producto { Id = 2, Nombre = "Mouse Inalámbrico Pro", Descripcion = "Mouse ergonómico 16000 DPI", Precio = 39.99m, Categoria = "Periféricos", ImagenUrl = "/img/productos/mouse.jpg" },
                new Producto { Id = 3, Nombre = "Monitor 24' 144Hz", Descripcion = "Pantalla Full HD", Precio = 179.99m, Categoria = "Monitores", ImagenUrl = "/img/productos/monitor-24.jpg" },
                new Producto { Id = 4, Nombre = "Audífonos Gamer 7.1", Descripcion = "Sonido envolvente con micrófono", Precio = 69.99m, Categoria = "Audio", ImagenUrl = "/img/productos/audifonos.jpg" },
                new Producto { Id = 5, Nombre = "Micrófono USB Condensador", Descripcion = "Ideal para streaming y llamadas", Precio = 54.99m, Categoria = "Audio", ImagenUrl = "/img/productos/microfono.jpg" },
                new Producto { Id = 6, Nombre = "Webcam Full HD 1080p", Descripcion = "Video nítido para tus reuniones", Precio = 44.99m, Categoria = "Accesorios", ImagenUrl = "/img/productos/webcam.jpg" },
                new Producto { Id = 7, Nombre = "Alfombrilla XL", Descripcion = "Superficie amplia y antideslizante", Precio = 19.99m, Categoria = "Accesorios", ImagenUrl = "/img/productos/alfombrilla.jpg" },
                new Producto { Id = 8, Nombre = "Soporte para Laptop", Descripcion = "Ajustable y plegable", Precio = 29.99m, Categoria = "Accesorios", ImagenUrl = "/img/productos/soporte.jpg" },
                new Producto { Id = 9, Nombre = "Monitor 27\" 4K", Descripcion = "Resolución Ultra HD", Precio = 329.99m, Categoria = "Monitores", ImagenUrl = "/img/productos/monitor-27.jpg" }
            );
        }
    }
}