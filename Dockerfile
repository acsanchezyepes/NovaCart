# Receta para ejecutar NovaCart en un contenedor (la usa Render para publicar la tienda).

# 1) Compilar y publicar la API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TiendaAPI/TiendaAPI.csproj TiendaAPI/
RUN dotnet restore TiendaAPI/TiendaAPI.csproj
COPY TiendaAPI/ TiendaAPI/
RUN dotnet publish TiendaAPI/TiendaAPI.csproj -c Release -o /app/publish --no-restore

# 2) Imagen final, solo con lo necesario para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Carpeta con permiso de escritura para la base de datos SQLite.
# En el plan gratuito de Render el disco no es permanente: las órdenes se borran al reiniciar.
RUN mkdir -p /app/data && chown $APP_UID /app/data
ENV ConnectionStrings__Tienda="Data Source=/app/data/tienda.db"
ENV ASPNETCORE_ENVIRONMENT=Production

# Ejecutar sin permisos de administrador
USER $APP_UID

# Render indica el puerto en la variable PORT; en local se usa 8080.
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "exec dotnet TiendaAPI.dll --urls http://0.0.0.0:${PORT:-8080}"]
