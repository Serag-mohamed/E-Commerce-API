# 🛒 E-Commerce RESTful Web API

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)
![JWT](https://img.shields.io/badge/Security-JWT-red)

**E-Commerce API** is a robust backend system for online stores, built with **ASP.NET Core 8.0** using **Clean Architecture** principles.  
It is designed for **high performance, secure operations, and maintainable backend logic**, including products, categories, users, orders, and payment management.

---

## 🚀 Key Features

### 📦 Products & Categories Management
- Full CRUD operations for **Products** and **Categories**.
- **Soft Delete & Restore** using EF Core Interceptors and Global Query Filters.
- Support for multiple product images with main image selection.

### 👤 User & Role Management
- **JWT Authentication** for secure login.
- **Role-based Authorization** (Admin, Customer, Seller).
- Secure password hashing.
- User creation and role management via Admin endpoints.

### 🛒 Orders & Checkout
- Create, update, and track orders.
- Total price calculation including discounts.
- **Database Transactions** to ensure data consistency for multi-step operations.

### 🔄 Advanced System Features
- **Global Error Handling Middleware** for consistent API responses.
- **Fluent Validation** for enforcing business rules.
- **Seed Data** support for testing and development environments.

### 📊 Monitoring & Documentation
- Fully documented endpoints using **Swagger (OpenAPI)**.
- Ready-to-use request/response examples.
- Easy testing via **Postman** or any HTTP client.

---

## 🧰 Tech Stack & Tools
- **Framework:** ASP.NET Core 8.0  
- **Language:** C#  
- **Database:** Microsoft SQL Server  
- **ORM:** Entity Framework Core  
- **Authentication:** JWT Bearer Tokens  
- **Architecture:** Clean Architecture (Domain – Application – Infrastructure – API layers)  
- **Tools:** Git, Swagger, Postman  

---

## 📖 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
