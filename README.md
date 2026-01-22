# 🛒 E-Commerce RESTful Web API

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)

## 🌟 Overview
A robust, scalable, and high-performance E-Commerce Backend system built with **ASP.NET Core**. This project is a production-ready implementation of **Clean Architecture** principles, focusing on data integrity, security, and maintainable backend logic.

---

## 🏗 Architecture & Design Patterns
The project follows **Clean Architecture** to ensure a clear separation of concerns:
- **Domain Layer:** Core entities, business rules, and custom exceptions.
- **Application Layer:** DTOs, Mapping profiles, and Service interfaces.
- **Infrastructure Layer:** Persistence logic (EF Core), Repository implementations.
- **API Layer:** Controllers and Middlewares.

### 🔹 Key Design Patterns:
* **Repository & Unit of Work:** For decoupled data access and atomic transactions.
* **Service Layer:** Encapsulating business rules away from controllers.
* **Result Pattern:** Providing consistent and predictable API responses.

---

## 🚀 Key Technical Features
- **Soft Delete & Restore:** Implemented using **EF Core Interceptors** and **Global Query Filters** to preserve data history.
- **Database Transactions:** Ensures data consistency during complex **Checkout** processes.
- **Security:** Integrated **JWT Authentication** and Role-based Authorization.
- **Global Error Handling:** Custom middleware for unified exception management.
- **Validation:** Strict business rule enforcement using **Fluent Validation** principles.
- **Documentation:** Fully documented endpoints using **Swagger (OpenAPI)**.

---

## 🛠 Tech Stack
- **Framework:** ASP.NET Core 8.0
- **Language:** C#
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Tools:** Git, Postman, Swagger

---

## 📖 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
