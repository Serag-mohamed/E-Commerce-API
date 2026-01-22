# 🛒 E-Commerce RESTful Web API

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)

**E-Commerce API** is a full-featured backend system for online stores, built with **ASP.NET Core 10** following **Clean Architecture** principles.  
It provides high performance, security, and maintainable backend logic for managing products, categories, users, orders, and payments.

---

## 🚀 Key Features

### 📦 Products & Categories Management
- Full CRUD operations for Products.
- Categories management.
- **Soft Delete** for products and categories to preserve historical data.
- Support for multiple images per product with main image selection.

### 👤 User Management
- JWT Authentication with **Role-based Authorization**:
  - Admin
  - Customer
  - Seller (optional future role)
- User creation and role management.
- Secure password hashing and token management.

### 🛒 Orders Management
- Create and track orders.
- Total price and discount calculation.
- **Database transactions** for safe checkout and multi-step operations.

### 🔄 Smart System Features
- **Soft Delete & Restore** for data recovery.
- **Global Query Filters** applied automatically on entities.
- **Global Error Handling Middleware** for unified API responses.
- **Fluent Validation** to enforce business rules and input validation.

---

## 🎨 Developer Experience (DX)
- **Swagger (OpenAPI)** for complete endpoint documentation.
- Ready-to-use request/response examples.
- Easy testing via **Postman** or similar tools.

---

## 🧰 Tech Stack
- **Framework:** ASP.NET Core 10.0  
- **Language:** C#  
- **Database:** Microsoft SQL Server  
- **ORM:** Entity Framework Core  
- **Tools:** Git, Postman, Swagger  

---

## 📖 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
