using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TiendaAPI.Migrations
{
    /// <summary>
    /// Agrega 6 productos nuevos y cambia las imágenes de ejemplo por fotos reales.
    /// No cambia la estructura de las tablas, solo los datos del catálogo.
    /// </summary>
    public partial class AmpliarCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "/img/productos/teclado.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "/img/productos/mouse.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "/img/productos/monitor-24.jpg");

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Categoria", "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 4, "Audio", "Sonido envolvente con micrófono", "/img/productos/audifonos.jpg", "Audífonos Gamer 7.1", 69.99m },
                    { 5, "Audio", "Ideal para streaming y llamadas", "/img/productos/microfono.jpg", "Micrófono USB Condensador", 54.99m },
                    { 6, "Accesorios", "Video nítido para tus reuniones", "/img/productos/webcam.jpg", "Webcam Full HD 1080p", 44.99m },
                    { 7, "Accesorios", "Superficie amplia y antideslizante", "/img/productos/alfombrilla.jpg", "Alfombrilla XL", 19.99m },
                    { 8, "Accesorios", "Ajustable y plegable", "/img/productos/soporte.jpg", "Soporte para Laptop", 29.99m },
                    { 9, "Monitores", "Resolución Ultra HD", "/img/productos/monitor-27.jpg", "Monitor 27\" 4K", 329.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 7);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 8);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 9);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "https://via.placeholder.com/150");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "https://via.placeholder.com/150");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "https://via.placeholder.com/150");
        }
    }
}
