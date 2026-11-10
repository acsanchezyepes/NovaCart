using System.Net;
using System.Net.Http.Json;
using TiendaAPI.Models;

namespace TiendaAPI.Tests;

public class OrdenesTests : IClassFixture<TiendaApiFactory>
{
    private readonly HttpClient _client;

    public OrdenesTests(TiendaApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Cuerpo de una orden con el email de prueba y los productos indicados.
    private static object NuevaOrden(params (int productoId, int cantidad)[] items) => new
    {
        emailCliente = "cliente@example.com",
        detalles = items.Select(i => new { productoId = i.productoId, cantidad = i.cantidad }),
    };

    [Fact]
    public async Task Crear_OrdenValida_Devuelve201ConElTotalCalculadoPorElServidor()
    {
        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", NuevaOrden((1, 2), (2, 1)));

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var orden = await respuesta.Content.ReadFromJsonAsync<Orden>();
        Assert.NotNull(orden);
        Assert.Equal(159.97m, orden.Total); // 2 × 59.99 + 1 × 39.99
        Assert.Equal(EstadosOrden.Pendiente, orden.Estado);
        Assert.Equal(2, orden.Detalles.Count);
        Assert.NotNull(respuesta.Headers.Location);
        Assert.EndsWith($"/{orden.Id}", respuesta.Headers.Location.ToString());
    }

    [Fact]
    public async Task Crear_IgnoraPrecioTotalYEstadoEnviadosPorElCliente()
    {
        var ordenManipulada = new
        {
            emailCliente = "cliente@example.com",
            total = 0.01m,
            estado = "Pagado",
            detalles = new[] { new { productoId = 1, cantidad = 1, precioUnitario = 0.01m } },
        };

        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", ordenManipulada);

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var orden = await respuesta.Content.ReadFromJsonAsync<Orden>();
        Assert.NotNull(orden);
        Assert.Equal(59.99m, orden.Total);
        Assert.Equal(EstadosOrden.Pendiente, orden.Estado);
        Assert.Equal(59.99m, orden.Detalles[0].PrecioUnitario);
    }

    [Fact]
    public async Task Crear_SinProductos_Devuelve400()
    {
        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", NuevaOrden());

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Crear_ConCantidadMenorOIgualACero_Devuelve400(int cantidad)
    {
        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", NuevaOrden((1, cantidad)));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Crear_ConProductoInexistente_Devuelve400()
    {
        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", NuevaOrden((999, 1)));

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Theory]
    [InlineData("esto-no-es-un-email")]
    [InlineData("")]
    public async Task Crear_ConEmailInvalido_Devuelve400(string email)
    {
        var orden = new { emailCliente = email, detalles = new[] { new { productoId = 1, cantidad = 1 } } };

        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", orden);

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Consultar_OrdenExistente_DevuelveSusProductos()
    {
        var creada = await (await _client.PostAsJsonAsync("/api/ordenes", NuevaOrden((3, 1))))
            .Content.ReadFromJsonAsync<Orden>();
        Assert.NotNull(creada);

        var orden = await _client.GetFromJsonAsync<Orden>($"/api/ordenes/{creada.Id}");

        Assert.NotNull(orden);
        Assert.Equal(179.99m, orden.Total);
        var detalle = Assert.Single(orden.Detalles);
        Assert.NotNull(detalle.Producto);
        Assert.Equal("Monitor 24' 144Hz", detalle.Producto.Nombre);
    }

    [Fact]
    public async Task Consultar_OrdenInexistente_Devuelve404()
    {
        var respuesta = await _client.GetAsync("/api/ordenes/999999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    [Fact]
    public async Task ListarTodasLasOrdenes_NoEstaPermitido()
    {
        // Protege los emails de los clientes: no hay listado general de órdenes.
        var respuesta = await _client.GetAsync("/api/ordenes");

        Assert.Equal(HttpStatusCode.MethodNotAllowed, respuesta.StatusCode);
    }
}
