# 🔧 MechanicShop

A mechanic shop management system built with **.NET 10**, designed with **Clean Architecture** and **CQRS**.

The project manages the main operations of a mechanic shop, including customers, vehicles, work orders, repair tasks, parts, employees, and invoices.

---

## 🛠️ Technologies

- .NET 10
- ASP.NET Core Web API
- Blazor
- Entity Framework Core
- SQL Server
- MediatR
- CQRS
- Clean Architecture
- FluentValidation
- JWT Authentication & Authorization
- SignalR
- OpenTelemetry
- Prometheus
- Grafana
- Seq
- Docker & Docker Compose

---

## 🏗️ Architecture

The solution follows **Clean Architecture** with a clear separation of responsibilities:

src/
├── MechanicShop.Api
├── MechanicShop.Application
├── MechanicShop.Domain
├── MechanicShop.Infrastructure
├── MechanicShop.Contracts
└── MechanicShop.Client

### Main Responsibilities

- **Domain**  
  Contains business entities, domain rules, and domain events.

- **Application**  
  Contains use cases, CQRS commands and queries, validation, and application behaviors.

- **Infrastructure**  
  Contains database access, Entity Framework Core configurations, authentication, caching, logging, and external infrastructure services.

- **Contracts**  
  Contains shared request and response contracts between the API and client.

- **API**  
  Exposes the application through ASP.NET Core Web API endpoints and handles authentication, authorization, middleware, and API configuration.

- **Client**  
  Provides the Blazor user interface and communicates with the API.

---

## ✨ Features

### Customer & Vehicle Management

- Create and manage customers.
- Manage customer vehicles.
- Associate vehicles with their owners.

### Work Order Management

- Create and manage work orders.
- Assign work orders to employees.
- Schedule work orders based on available spots.
- Prevent scheduling conflicts.
- Validate operating hours.
- Track work order status.
- Real-time work order updates using SignalR.

### Repair Tasks & Parts

- Create repair tasks.
- Associate parts with repair tasks.
- Track labor costs and repair duration.

### Billing

- Generate invoices for completed work orders.
- Manage invoice line items.
- Calculate labor and parts costs.

### Authentication & Authorization

- JWT-based authentication.
- Role-based authorization.
- Support for different employee roles.

### Validation & Application Behaviors

- Request validation using FluentValidation.
- CQRS pipeline behaviors using MediatR.
- Application-level caching.
- Centralized error/result handling.

### Observability

The application includes an observability stack using:

- OpenTelemetry
- Prometheus
- Grafana
- Seq

This provides application metrics, tracing, and centralized structured logging.

---

## 🐳 Running with Docker

The project includes a Docker Compose setup for the main infrastructure services.

### Services

| Service          | Port   |
|------------------|--------|
| MechanicShop API | `5001` |
| SQL Server       | `1433` |
| Seq              | `8081` |
| Prometheus       | `9090` |
| Grafana          | `3000` |

### Start the Application

Make sure Docker Desktop is running, then execute:

docker compose up -d

### Check Running Containers

docker compose ps

### Stop the Application

docker compose down

The Docker volumes are preserved when using `docker compose down`, so persistent data such as the SQL Server database is not removed.

### Rebuild After Code Changes

docker compose up -d --build

---

## 📁 Project Structure

MECHANICSHOP
│
├── src
│   ├── MechanicShop.Api
│   ├── MechanicShop.Application
│   ├── MechanicShop.Domain
│   ├── MechanicShop.Infrastructure
│   ├── MechanicShop.Contracts
│   └── MechanicShop.Client
│
├── containers
│   └── prometheus
│       └── prometheus.yml
│
├── Dockerfile
├── docker-compose.yml
├── Directory.Build.props
├── Directory.Packages.props
└── README.md


---

## 🌐 API

Once the Docker containers are running, the API is available at:

**API**

http://localhost:5001

---

## 📊 Monitoring

### Prometheus

Prometheus collects application metrics exposed by the API.

**Prometheus Dashboard**

http://localhost:9090

**API Metrics Endpoint**

http://localhost:5001/metrics

### Grafana

Grafana can be used to visualize metrics collected by Prometheus.

**Grafana Dashboard**

http://localhost:3000

### Seq

Seq is used for centralized structured logging.

**Seq Dashboard**

http://localhost:8081

---

## 🧑‍💻 Development

The project is currently under active development.

The main goals are to practice and apply real-world backend development concepts including:

- Domain Modeling
- Clean Architecture
- CQRS
- MediatR
- Entity Framework Core
- Authentication & Authorization
- Distributed application infrastructure
- Observability
- Containerization

---

## 👤 Author

**Ahmed Ibrahim**

.NET Backend Developer

**GitHub:**  
https://github.com/ahmedmandoo494