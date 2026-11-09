# 🛒 TiendaAPI - E-Commerce Backend Service

A lightweight, robust RESTful Web API built with **.NET 8** and **Entity Framework Core**, utilizing **SQLite** as its relational database. This project serves as a structured, multi-phase implementation of a modern e-commerce backend system.

---

## 🚀 Key Features & Architecture

* **RESTful Architecture:** Clear endpoint design following standard HTTP methods (`GET`, `POST`).
* **ORM Integration:** Entity Framework Core for database schema creation and data access using C#.
* **Automated Data Seeding:** Initial catalog pre-loaded directly upon application startup.
* **Interactive API Documentation:** Integrated Swagger UI for testing endpoints in real time.
* **Relational Data Modeling:** One-to-Many entity mapping between Orders and Order Details.

---

## 🛠️ Tech Stack

* **Language:** C#
* **Framework:** .NET 8 Web API
* **Database:** SQLite
* **ORM:** Entity Framework Core
* **Tools:** Visual Studio Code, Swagger UI

---

## 🗺️ Project Roadmap & Status

- [x] **Phase 1: Base Catalog & REST API**
  - Project setup, SQLite configuration, `Producto` model, and `ProductosController` (`GET /api/productos`).
- [x] **Phase 2: Shopping Cart & Order Management** *(In Progress)*
  - `Orden` and `DetalleOrden` models, relational mapping in `AppDbContext`, and `OrdenesController` (`POST /api/ordenes`).
- [ ] **Phase 3: Payment Gateway Simulator (Mock Service)**
  - Payment processing logic and order status updating (`Pending`, `Paid`, `Rejected`).
- [ ] **Phase 4: Web User Interface (Frontend)**
  - Interactive HTML/CSS/JavaScript interface connected via Fetch API.
- [ ] **Phase 5: Integrated Testing & Local Deployment**
  - End-to-end testing of the complete checkout flow.
- [ ] **Phase 6: Version Control & GitHub Publication**
  - Repository optimization, `.gitignore` setup, and final documentation.

---

## 📂 Project Structure

```plaintext
TiendaAPI/
├── Controllers/
│   ├── ProductosController.cs   # API Endpoints for Catalog Management
│   └── OrdenesController.cs     # API Endpoints for Order Creation & History
├── Data/
│   └── AppDbContext.cs          # EF Core Database Context & Seeding
├── Migrations/                  # EF Core schema migrations
├── Models/
│   ├── Producto.cs              # Product Entity Model
│   ├── Orden.cs                 # Order Entity Model
│   └── DetalleOrden.cs          # Order Detail Entity Model
├── Program.cs                   # Application Entry Point & Service Pipeline
├── TiendaAPI.http               # Integration test requests (REST Client)
└── tienda.db                    # SQLite Database File (created on startup, not versioned)
```

---

## ▶️ Running & Testing

```bash
cd TiendaAPI
dotnet run --launch-profile http
```

* API: `http://localhost:5232`
* Swagger UI: `http://localhost:5232/swagger`
* Integration tests: open `TiendaAPI.http` and click **Send Request** above each request (requires the REST Client extension). Each request states the expected status code.

On GitHub Codespaces, the `.devcontainer` configuration installs .NET 8 and the REST Client extension and forwards port 5232.