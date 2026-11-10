# 🛒 NovaCart - E-Commerce Store (TiendaAPI)

A lightweight e-commerce project built with **.NET 10**, **Entity Framework Core** and **SQLite**, with a web storefront in plain **HTML, CSS and JavaScript**. The API serves both the REST endpoints and the store page.

> **Demo store:** orders are not charged or shipped. Do not enter real personal data.

---

## 🚀 Key Features

* **Product catalog** of 9 products with real photos and category filtering.
* **Shopping cart** in the browser: add, remove and change quantities (saved between visits).
* **Checkout** that sends real orders to the API with `fetch`.
* **Payment simulator:** approve or reject the payment of a pending order (no card data is requested).
* **Server-side pricing:** the API ignores prices, totals and status sent by the client and calculates them from the database.
* **Input validation:** rejects empty orders, unknown products, quantities ≤ 0 and invalid emails.
* **Privacy:** there is no public endpoint that lists every order and customer email.
* **EF Core migrations** applied automatically on startup, with seeded catalog data.
* **Swagger UI** for exploring the API in development.

---

## 🛠️ Tech Stack

* **Backend:** C#, .NET 10 Web API, Entity Framework Core, SQLite
* **Frontend:** HTML, CSS, JavaScript (no frameworks)
* **Deployment:** Docker-ready (`Dockerfile` included)
* **Tools:** Visual Studio Code, Swagger UI, REST Client, GitHub Codespaces

---

## 🔌 API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/productos?categoria=` | List products, optionally filtered by category |
| GET | `/api/productos/{id}` | Get one product (`404` if it does not exist) |
| POST | `/api/ordenes` | Create an order from `emailCliente` and `detalles` (`productoId`, `cantidad`) |
| GET | `/api/ordenes/{id}` | Get one order with its items (`404` if it does not exist) |
| POST | `/api/ordenes/{id}/pago` | Simulate the payment of a pending order with `{ "aprobado": true }` or `false` (`409` if it was already processed) |

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
  - Request suite in `TiendaAPI.http` covering `200`, `201`, `400`, `404`, `405` and `409` responses.
- [x] **Phases 5 & 6: Web Storefront & API Connection**
  - Catalog, cart and checkout in `wwwroot/`, connected to the API with `fetch`.
- [x] **Expanded catalog & payment simulator**
  - 9 products with free-licensed photos and order status updates (`Pendiente`, `Pagado`, `Rechazado`).

### Next steps

- [ ] Customer authentication to view personal order history.

---

## 📷 Photo Credits

Product photos come from [Wikimedia Commons](https://commons.wikimedia.org) under free licenses (CC BY-SA, CC0 and public domain) and are shown resized. Authors and licenses are listed on the store's credits page: `TiendaAPI/wwwroot/creditos.html` (served at `/creditos.html`).

---

## 📂 Project Structure

```plaintext
.
├── Dockerfile                   # Container build used for deployment
├── .devcontainer/               # GitHub Codespaces configuration
└── TiendaAPI/
    ├── Controllers/
    │   ├── ProductosController.cs   # Catalog endpoints
    │   └── OrdenesController.cs     # Order creation and lookup
    ├── Data/
    │   └── AppDbContext.cs          # EF Core context & seed data
    ├── Dtos/
    │   ├── CrearOrdenDto.cs         # Accepted input when creating an order
    │   └── SimularPagoDto.cs        # Accepted input when simulating a payment
    ├── Migrations/                  # EF Core schema migrations
    ├── Models/
    │   ├── Producto.cs
    │   ├── Orden.cs
    │   ├── DetalleOrden.cs
    │   └── EstadosOrden.cs          # Pendiente, Pagado, Rechazado
    ├── wwwroot/                     # Storefront (served by the API)
    │   ├── index.html
    │   ├── creditos.html            # Photo credits
    │   ├── img/productos/           # Product photos
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

On GitHub Codespaces, the `.devcontainer` configuration installs .NET 10 and the REST Client extension and forwards port 5232.

---

## ☁️ Deployment

The repository includes a `Dockerfile`, so it can be deployed to any container host. On [Render](https://render.com):

1. Create a new **Web Service** and connect this GitHub repository.
2. Choose the **Docker** runtime (Render detects the `Dockerfile` in the root).
3. Deploy. The app listens on the port Render provides in the `PORT` variable.

The SQLite database lives inside the container, so on hosts without a persistent disk the orders are reset on every restart or deploy. The product catalog is recreated automatically from the seed data.
