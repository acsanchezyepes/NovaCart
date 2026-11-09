using Microsoft.EntityFrameworkCore;
using TiendaAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// 2. Registrar controladores (esto activa ProductosController)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Crear o actualizar la base de datos aplicando las migraciones pendientes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. Servir la página de la tienda (wwwroot/index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

// 5. Mapear los controladores
app.MapControllers();

app.Run();