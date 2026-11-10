using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TiendaAPI.Data;

namespace TiendaAPI.Tests;

// Enciende la API en memoria para las pruebas, con una base de datos SQLite
// temporal y propia. Así las pruebas nunca tocan tienda.db.
public class TiendaApiFactory : WebApplicationFactory<Program>
{
    private readonly string _rutaDb =
        Path.Combine(Path.GetTempPath(), $"tienda-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Quita la configuración de la base de datos real y usa la temporal.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={_rutaDb}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // Borra la base de datos temporal al terminar.
        SqliteConnection.ClearAllPools();
        foreach (var sufijo in new[] { "", "-shm", "-wal" })
        {
            File.Delete(_rutaDb + sufijo);
        }
    }
}
