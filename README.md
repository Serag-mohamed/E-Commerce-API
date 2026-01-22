# 🛒 E-Commerce RESTful Web API

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)

## 🌟 Overview
A robust, scalable, and high-performance **E-Commerce Backend API** built with **ASP.NET Core** following **Clean Architecture** principles.  
This project is production-ready and focuses on **data integrity, security, and maintainable backend logic**.

---

## 🏗 Architecture & Design Patterns
The project uses **Clean Architecture** to ensure a clear separation of concerns:

- **Domain Layer:** Core entities, business rules, and custom exceptions.  
- **Application Layer:** DTOs, Mapping profiles, and Service interfaces.  
- **Infrastructure Layer:** Persistence logic (EF Core), Repository implementations.  
- **API Layer:** Controllers and Middlewares.

### 🔹 Key Design Patterns:
- **Repository & Unit of Work:** Decoupled data access and atomic transactions.  
- **Service Layer:** Encapsulates business rules away from controllers.  
- **Result Pattern:** Standardized API responses for consistent client handling.  

---

## 🚀 Key Features
- **Soft Delete & Restore:** Using **EF Core Interceptors** and **Global Query Filters**.  
- **Database Transactions:** Ensures data consistency during checkout or multi-step operations.  
- **Security:** JWT Authentication with Role-based Authorization.  
- **Global Error Handling:** Unified exception management via custom middleware.  
- **Validation:** Business rules enforced using **Fluent Validation**.  
- **API Documentation:** Fully documented endpoints using **Swagger (OpenAPI)**.  

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

### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/Serag-mohamed/E-Commerce-API.git
