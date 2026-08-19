
# CRN Product Management API

This is a .NET 8 Web API created for the CRN Technosoft technical assessment.

The project provides Product and Item management with authentication and role-based access.

## Tech Used

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- FluentValidation
- Swagger
- Serilog
- xUnit and Moq
- Docker

## Features

- Product CRUD operations
- Add, update and delete items
- JWT authentication
- Refresh token support
- Role-based authorization
- Admin and User roles
- Pagination and product search
- FluentValidation
- Global exception handling
- API versioning
- Swagger documentation
- Logging
- Unit and integration tests
- Docker support

## Project Structure

```text
CRNProductApi.sln
│
├── CRNProductApi
│   ├── Controllers
│   ├── Data
│   │   └── Configurations
│   ├── DTOs
│   │   ├── Auth
│   │   ├── Common
│   │   ├── Item
│   │   └── Product
│   ├── Exceptions
│   ├── Extensions
│   ├── Middleware
│   ├── Models
│   ├── Repository
│   ├── Services
│   │   └── Interfaces
│   ├── Validators
│   ├── Migrations
│   ├── Program.cs
│   └── appsettings.json
│
├── CRNProductApi.Tests
│   ├── Integration
│   └── Services
│
├── Dockerfile
├── docker-compose.yml
└── README.md
````

## Running the Project

Update the database connection string in `appsettings.Development.json`.

Run the migration:

```bash
dotnet ef database update --project CRNProductApi/CRNProductApi.csproj
```

Then run the API:

```bash
dotnet run --project CRNProductApi/CRNProductApi.csproj
```

Open Swagger:

```text
http://localhost:5225/swagger
```

## Authentication

There are two roles:

* **User** – Can view products and items.
* **Admin** – Can create, update and delete products and items.

A development admin account is seeded for testing:

```text
Email: admin@crnproductapi.com
Password: Admin@123
```

## API Endpoints

### Auth

| Method | Endpoint                |
| ------ | ----------------------- |
| POST   | `/api/v1/auth/register` |
| POST   | `/api/v1/auth/login`    |
| POST   | `/api/v1/auth/refresh`  |
| POST   | `/api/v1/auth/logout`   |

### Products

| Method | Endpoint                |
| ------ | ----------------------- |
| GET    | `/api/v1/products`      |
| GET    | `/api/v1/products/{id}` |
| POST   | `/api/v1/products`      |
| PUT    | `/api/v1/products/{id}` |
| DELETE | `/api/v1/products/{id}` |

You can use pagination and search:

```text
/api/v1/products?pageNumber=1&pageSize=10&search=laptop
```

### Items

| Method | Endpoint                                      |
| ------ | --------------------------------------------- |
| GET    | `/api/v1/products/{productId}/items`          |
| GET    | `/api/v1/products/{productId}/items/{itemId}` |
| POST   | `/api/v1/products/{productId}/items`          |
| PUT    | `/api/v1/products/{productId}/items/{itemId}` |
| DELETE | `/api/v1/products/{productId}/items/{itemId}` |

## Tests

Run all tests with:

```bash
dotnet test
```

The project contains unit tests for the services and integration tests for the API endpoints.

## Docker

To build and run the API with SQL Server:

```bash
docker compose up --build -d
```

To check the logs:

```bash
docker compose logs -f api
```

To stop the containers:

```bash
docker compose down
```
