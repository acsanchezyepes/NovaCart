using Microsoft.EntityFrameworkCore;
using TiendaAPI.Models;

namespace TiendaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Teclado Mecánico RGB", Descripcion = "Teclado switch blue", Precio = 59.99m, Categoria = "Periféricos", ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 2, Nombre = "Mouse Inalámbrico Pro", Descripcion = "Mouse ergonómico 16000 DPI", Precio = 39.99m, Categoria = "Periféricos", ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 3, Nombre = "Monitor 24' 144Hz", Descripcion = "Pantalla Full HD", Precio = 179.99m, Categoria = "Monitores", ImagenUrl = "https://via.placeholder.com/150" }
            );
        }
    }
}