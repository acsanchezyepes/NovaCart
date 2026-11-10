using System.Net;
using System.Net.Http.Json;
using TiendaAPI.Models;

namespace TiendaAPI.Tests;

public class PagosTests : IClassFixture<TiendaApiFactory>
{
    private readonly HttpClient _client;

    public PagosTests(TiendaApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Crea una orden nueva (estado Pendiente) para poder pagarla.
    private async Task<Orden> CrearOrdenPendiente()
    {
        var cuerpo = new { emailCliente = "cliente@example.com", detalles = new[] { new { productoId = 2, cantidad = 1 } } };
        var respuesta = await _client.PostAsJsonAsync("/api/ordenes", cuerpo);
        respuesta.EnsureSuccessStatusCode();
        return (await respuesta.Content.ReadFromJsonAsync<Orden>())!;
    }

    private Task<HttpResponseMessage> Pagar(int ordenId, object cuerpo) =>
        _client.PostAsJsonAsync($"/api/ordenes/{ordenId}/pago", cuerpo);

    [Fact]
    public async Task PagoAprobado_CambiaElEstadoAPagado()
    {
        var orden = await CrearOrdenPendiente();

        var respuesta = await Pagar(orden.Id, new { aprobado = true });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var pagada = await respuesta.Content.ReadFromJsonAsync<Orden>();
        Assert.Equal(EstadosOrden.Pagado, pagada?.Estado);
    }

    [Fact]
    public async Task PagoRechazado_CambiaElEstadoARechazado()
    {
        var orden = await CrearOrdenPendiente();

        var respuesta = await Pagar(orden.Id, new { aprobado = false });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var rechazada = await respuesta.Content.ReadFromJsonAsync<Orden>();
        Assert.Equal(EstadosOrden.Rechazado, rechazada?.Estado);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PagarUnaOrdenYaProcesada_Devuelve409(bool primerResultado)
    {
        var orden = await CrearOrdenPendiente();
        await Pagar(orden.Id, new { aprobado = primerResultado });

        var segundoIntento = await Pagar(orden.Id, new { aprobado = true });

        Assert.Equal(HttpStatusCode.Conflict, segundoIntento.StatusCode);
    }

    [Fact]
    public async Task PagoSinIndicarElResultado_Devuelve400()
    {
        var orden = await CrearOrdenPendiente();

        var respuesta = await Pagar(orden.Id, new { });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task PagoDeUnaOrdenInexistente_Devuelve404()
    {
        var respuesta = await Pagar(999999, new { aprobado = true });

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
