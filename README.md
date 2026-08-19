# CRN Product Management REST API

A production-ready RESTful Web API built with **.NET 8** for managing Products and Items with JWT authentication, refresh token rotation, role-based authorization, generic repository pattern, FluentValidation, API versioning, structured logging, Swashbuckle OpenAPI Swagger UI, xUnit tests, and Docker support.

---

## 1. Project Overview

This project fulfills the technical assessment requirements using **1 main Web API project** (`CRNProductApi`) and **1 xUnit test project** (`CRNProductApi.Tests`).

---

## 2. Technology Stack

* **Framework**: .NET 8 / C#
* **Web API**: ASP.NET Core Web API
* **Database**: SQL Server / Entity Framework Core 8 (Code First)
* **Identity & Auth**: ASP.NET Core Identity + JWT Bearer Tokens with Refresh Token Rotation & Revocation
* **Validation**: FluentValidation
* **Documentation**: Swagger / OpenAPI (Swashbuckle)
* **Logging**: Serilog (Console & File logging)
* **API Versioning**: `Asp.Versioning.Mvc` (v1.0)
* **Testing**: xUnit, Moq, `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`), EF Core InMemory
* **Containerization**: Docker, Docker Compose

---

## 3. Project Structure

```text
CRNProductApi.sln
│
├── CRNProductApi/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   └── ItemsController.cs
│   │
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── DatabaseSeeder.cs
│   │   └── Configurations/
│   │       ├── ProductConfiguration.cs
│   │       ├── ItemConfiguration.cs
│   │       ├── ApplicationUserConfiguration.cs
│   │       └── RefreshTokenConfiguration.cs
│   │
│   ├── Models/
│   │   ├── BaseEntity.cs
│   │   ├── Product.cs
│   │   ├── Item.cs
│   │   ├── ApplicationUser.cs
│   │   ├── RefreshToken.cs
│   │   └── UserRoles.cs
│   │
│   ├── DTOs/
│   │   ├── Product/
│   │   │   ├── CreateProductDto.cs
│   │   │   ├── UpdateProductDto.cs
│   │   │   └── ProductResponseDto.cs
│   │   ├── Item/
│   │   │   ├── CreateItemDto.cs
│   │   │   ├── UpdateItemDto.cs
│   │   │   └── ItemResponseDto.cs
│   │   ├── Auth/
│   │   │   ├── RegisterDto.cs
│   │   │   ├── LoginDto.cs
   │   │   ├── RefreshTokenRequestDto.cs
│   │   │   └── AuthResponseDto.cs
│   │   └── Common/
│   │       ├── PageRequest.cs
│   │       ├── PageResponse.cs
│   │       └── ApiResponse.cs
│   │
│   ├── Repository/
│   │   ├── IRepository.cs
│   │   └── Repository.cs
│   │
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IProductService.cs
│   │   │   ├── IItemService.cs
│   │   │   ├── IAuthService.cs
│   │   │   ├── ITokenService.cs
│   │   │   └── ICurrentUserService.cs
│   │   ├── ProductService.cs
│   │   ├── ItemService.cs
│   │   ├── AuthService.cs
│   │   ├── TokenService.cs
│   │   └── CurrentUserService.cs
│   │
│   ├── Validators/
│   │   ├── CreateProductValidator.cs
│   │   ├── UpdateProductValidator.cs
│   │   ├── CreateItemValidator.cs
│   │   ├── UpdateItemValidator.cs
│   │   ├── RegisterValidator.cs
│   │   ├── LoginValidator.cs
│   │   └── RefreshTokenRequestValidator.cs
│   │
│   ├── Middleware/
│   │   ├── ExceptionMiddleware.cs
│   │   └── SecurityHeadersMiddleware.cs
│   │
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs
│   │
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   └── ConflictException.cs
│   │
│   ├── Migrations/
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── CRNProductApi.Tests/
│   ├── Services/
│   │   ├── ProductServiceTests.cs
│   │   ├── ItemServiceTests.cs
│   │   └── AuthServiceTests.cs
│   ├── Integration/
│   │   ├── ProductsEndpointsTests.cs
│   │   ├── ItemsEndpointsTests.cs
│   │   └── AuthEndpointsTests.cs
│   └── CustomWebApplicationFactory.cs
│
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── .gitignore
└── README.md
```

---

## 4. Installed Packages

* `Microsoft.EntityFrameworkCore.SqlServer` (8.0.11)
* `Microsoft.EntityFrameworkCore.Tools` (8.0.11)
* `Microsoft.EntityFrameworkCore.Design` (8.0.11)
* `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.11)
* `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11)
* `System.IdentityModel.Tokens.Jwt` (8.0.1)
* `FluentValidation.DependencyInjectionExtensions` (11.10.0)
* `Asp.Versioning.Mvc` (8.1.0)
* `Asp.Versioning.Mvc.ApiExplorer` (8.1.0)
* `Swashbuckle.AspNetCore` (6.9.0)
* `Serilog.AspNetCore` (8.0.2)
* `Serilog.Sinks.Console` (6.0.0)
* `Serilog.Sinks.File` (6.0.0)
* `Moq` (4.20.72)
* `Microsoft.AspNetCore.Mvc.Testing` (8.0.11)
* `Microsoft.EntityFrameworkCore.InMemory` (8.0.11)

---

## 5. Migration Commands

```bash
# Add Initial EF Core Migration
dotnet ef migrations add InitialCreate --project CRNProductApi/CRNProductApi.csproj

# Update Database
dotnet ef database update --project CRNProductApi/CRNProductApi.csproj
```

---

## 6. How to Run Locally

```bash
dotnet run --project CRNProductApi/CRNProductApi.csproj
```

---

## 7. Swagger URL

* **Base URL**: `http://localhost:5225`
* **Swagger UI**: [http://localhost:5225/swagger](http://localhost:5225/swagger)

---

## 8. Authentication & Authorization

* **`User` Role**: Read-only access (`GET /api/v1/products`, `GET /api/v1/products/{id}`, `GET /api/v1/products/{productId}/items`).
* **`Admin` Role**: Full access (`GET`, `POST`, `PUT`, `DELETE`).
* **Development Admin Credentials**:
  * **Email**: `admin@crnproductapi.com`
  * **Password**: `Admin@123`

---

## 9. API Endpoint Summary

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Anonymous | Register user |
| `POST` | `/api/v1/auth/login` | Anonymous | Authenticate & issue tokens |
| `POST` | `/api/v1/auth/refresh` | Anonymous | Rotate refresh token |
| `POST` | `/api/v1/auth/logout` | Anonymous | Revoke refresh token |
| `GET` | `/api/v1/products` | User/Admin | Get paginated products (`?pageNumber=1&pageSize=10&search=term`) |
| `GET` | `/api/v1/products/{id}` | User/Admin | Get product by ID |
| `POST` | `/api/v1/products` | Admin | Create product |
| `PUT` | `/api/v1/products/{id}` | Admin | Update product |
| `DELETE` | `/api/v1/products/{id}` | Admin | Delete product |
| `GET` | `/api/v1/products/{productId}/items` | User/Admin | Get items for product |
| `GET` | `/api/v1/products/{productId}/items/{itemId}` | User/Admin | Get specific item |
| `POST` | `/api/v1/products/{productId}/items` | Admin | Create item under product |
| `PUT` | `/api/v1/products/{productId}/items/{itemId}` | Admin | Update item quantity |
| `DELETE` | `/api/v1/products/{productId}/items/{itemId}` | Admin | Delete item |

---

## 10. How to Run Tests

```bash
dotnet test
```
All **21 unit and integration tests** pass cleanly with 0 failures.

---

## 11. Docker Commands

```bash
# Build and run API + SQL Server
docker compose up --build -d

# View API logs
docker compose logs -f api
```
