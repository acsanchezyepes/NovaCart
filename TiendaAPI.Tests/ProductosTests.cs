using System.Net;
using System.Net.Http.Json;
using TiendaAPI.Models;

namespace TiendaAPI.Tests;

public class ProductosTests : IClassFixture<TiendaApiFactory>
{
    private readonly HttpClient _client;

    public ProductosTests(TiendaApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_DevuelveLosNueveProductosDelCatalogo()
    {
        var productos = await _client.GetFromJsonAsync<List<Producto>>("/api/productos");

        Assert.NotNull(productos);
        Assert.Equal(9, productos.Count);
    }

    [Fact]
    public async Task Filtrar_PorCategoria_NoDistingueMayusculas()
    {
        var productos = await _client.GetFromJsonAsync<List<Producto>>("/api/productos?categoria=monitores");

        Assert.NotNull(productos);
        Assert.Equal(2, productos.Count);
        Assert.All(productos, p => Assert.Equal("Monitores", p.Categoria));
    }

    [Fact]
    public async Task ObtenerPorId_Existente_DevuelveElProducto()
    {
        var producto = await _client.GetFromJsonAsync<Producto>("/api/productos/1");

        Assert.NotNull(producto);
        Assert.Equal("Teclado Mecánico RGB", producto.Nombre);
        Assert.Equal(59.99m, producto.Precio);
    }

    [Fact]
    public async Task ObtenerPorId_Inexistente_Devuelve404()
    {
        var respuesta = await _client.GetAsync("/api/productos/999");

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
