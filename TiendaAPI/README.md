# 🛒 NovaCart - E-Commerce Store (TiendaAPI)

A lightweight e-commerce project built with **.NET 8**, **Entity Framework Core** and **SQLite**, with a web storefront in plain **HTML, CSS and JavaScript**. The API serves both the REST endpoints and the store page.

---

## 🚀 Key Features

* **Product catalog** with category filtering.
* **Shopping cart** in the browser: add, remove and change quantities (saved between visits).
* **Checkout** that sends real orders to the API with `fetch`.
* **Server-side pricing:** the API ignores prices, totals and status sent by the client and calculates them from the database.
* **Input validation:** rejects empty orders, unknown products, quantities ≤ 0 and invalid emails.
* **Privacy:** there is no public endpoint that lists every order and customer email.
* **EF Core migrations** applied automatically on startup, with seeded catalog data.
* **Swagger UI** for exploring the API in development.

---

## 🛠️ Tech Stack

* **Backend:** C#, .NET 8 Web API, Entity Framework Core, SQLite
* **Frontend:** HTML, CSS, JavaScript (no frameworks)
* **Tools:** Visual Studio Code, Swagger UI, REST Client, GitHub Codespaces

---

## 🔌 API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/productos?categoria=` | List products, optionally filtered by category |
| GET | `/api/productos/{id}` | Get one product (`404` if it does not exist) |
| POST | `/api/ordenes` | Create an order from `emailCliente` and `detalles` (`productoId`, `cantidad`) |
| GET | `/api/ordenes/{id}` | Get one order with its items (`404` if it does not exist) |

Example order request:

```json
{
  "emailCliente": "cliente@example.com",
  "detalles": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 2, "cantidad": 1 }
  ]
}
```

---

## 🗺️ Project Roadmap & Status

- [x] **Phase 1: Base Catalog & REST API**
  - Project setup, SQLite configuration, `Producto` model and `ProductosController`.
- [x] **Phase 2: Orders**
  - `Orden` and `DetalleOrden` models, relational mapping, migrations and `OrdenesController`.
- [x] **Phase 3: Business Logic & Security**
  - Server-side totals, product and quantity validation, input DTO, email validation.
- [x] **Phase 4: Integration Testing**
  - Request suite in `TiendaAPI.http` covering `201`, `400`, `404` and `405` responses.
- [x] **Phases 5 & 6: Web Storefront & API Connection**
  - Catalog, cart and checkout in `wwwroot/`, connected to the API with `fetch`.

### Next steps

- [ ] Payment simulator that updates order status (`Pendiente`, `Pagado`, `Rechazado`).
- [ ] Customer authentication to view personal order history.

---

## 📂 Project Structure

```plaintext
TiendaAPI/
├── Controllers/
│   ├── ProductosController.cs   # Catalog endpoints
│   └── OrdenesController.cs     # Order creation and lookup
├── Data/
│   └── AppDbContext.cs          # EF Core context & seed data
├── Dtos/
│   └── CrearOrdenDto.cs         # Accepted input when creating an order
├── Migrations/                  # EF Core schema migrations
├── Models/
│   ├── Producto.cs
│   ├── Orden.cs
│   └── DetalleOrden.cs
├── wwwroot/                     # Storefront (served by the API)
│   ├── index.html
│   ├── css/styles.css
│   └── js/app.js
├── Program.cs                   # Services, migrations, static files and routing
├── TiendaAPI.http               # Integration test requests (REST Client)
└── tienda.db                    # SQLite database (created on startup, not versioned)
```

---

## ▶️ Running & Testing

```bash
cd TiendaAPI
dotnet run --launch-profile http
```

* Store: `http://localhost:5232`
* Swagger UI: `http://localhost:5232/swagger`
* Integration tests: open `TiendaAPI.http` and click **Send Request** above each request, in order (requires the REST Client extension). Each request states the expected status code.

On GitHub Codespaces, the `.devcontainer` configuration installs .NET 8 and the REST Client extension and forwards port 5232.
