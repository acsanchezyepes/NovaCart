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

// 3. Crear la base de datos automáticamente si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// 4. Mapear los controladores
app.MapControllers();

app.Run();