# 🛒 E-Commerce Web API

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

A robust and scalable E-Commerce Backend system built with **ASP.NET Core**, following **Clean Architecture** principles. The project handles complex business workflows like product management, shopping carts, and secure checkout processes.

## 🏗 Architecture & Design Patterns
This project is built using **Clean Architecture** to ensure a clear separation of concerns, maintainability, and testability.
- **Domain Layer:** Contains entities, exceptions, and business logic.
- **Application Layer:** Contains DTOs, Mapping, and Service interfaces.
- **Infrastructure Layer:** Implements data access (EF Core) and external services.
- **API Layer:** Handles HTTP requests and middlewares.

### 🔹 Key Design Patterns:
* **Repository Pattern & Unit of Work:** For decoupled data access and atomic transactions.
* **Service Layer:** To encapsulate business logic and validation.
* **Result Pattern:** To provide consistent API responses.

## 🚀 Technical Features
- **Soft Delete & Restore:** Implemented for Products and Categories using **EF Core Interceptors** and **Global Query Filters**.
- **Data Consistency:** Managed complex checkout operations using **Database Transactions**.
- **Security:** Integrated **JWT Authentication** and Authorization.
- **Global Error Handling:** Custom middleware to catch and format exceptions.
- **Data Mapping:** Used **AutoMapper** (or manual mapping) to decouple Domain Entities from DTOs.
- **Documentation:** Interactive API documentation using **Swagger (OpenAPI)**.

## 🛠 Tech Stack
- **Framework:** ASP.NET Core 8.0
- **Language:** C#
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Tools:** Git, Postman, Swagger

## 📖 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation & Setup
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/Serag-mohamed/E-Commerce-API.git](https://github.com/Serag-mohamed/E-Commerce-API.git)
