# Online Learning Platform

A microservices-based online learning platform built with .NET 10, demonstrating service decomposition, asynchronous messaging, and clean architecture principles.

> Second microservices project after [Restaurant Bill](https://github.com/fatihkayaci/RestaurantBill). Focus: backend architecture depth over feature breadth.

---

## Architecture overview

4 independent microservices behind a single API Gateway, communicating via REST (synchronous) and RabbitMQ (asynchronous events).

| Service | Responsibility |
|---|---|
| **Identity** | Authentication, JWT issuance, user management |
| **Catalog** | Courses, lessons, reviews |
| **Enrollment** | Student enrollments and progress |
| **Notification** | Email and in-app notifications (event-driven) |

Plus a **YARP API Gateway** as the single entry point.

Full architecture decisions, sequence diagrams, event catalog, and data ownership tables: see [`docs/architecture.md`](docs/architecture.md).

---

## Tech stack

**Per service**
- .NET 10, ASP.NET Core, EF Core, PostgreSQL
- MediatR (CQRS), FluentValidation
- Serilog (structured logging)

**Cross-cutting**
- API Gateway: YARP
- Messaging: RabbitMQ + MassTransit
- Cache: Redis
- Resilience: Polly
- Logging: Serilog + Seq

**DevOps**
- Docker + Docker Compose (local)
- GitHub Actions (CI)
- Azure Container Apps (production)

**Testing**
- xUnit + FluentAssertions
- Testcontainers (real PostgreSQL in integration tests)

---

## Running locally

> Requires Docker Desktop.

```bash
git clone https://github.com/fatihkayaci/learning-platform.git
cd learning-platform
docker-compose up -d
```

Services will be available at:

| Service | URL |
|---|---|
| API Gateway | http://localhost:5000 |
| Seq (logs) | http://localhost:5341 |
| RabbitMQ management | http://localhost:15672 |

---

## Project status

🚧 Work in progress — see [project board](#) for current milestone.

- [x] Architecture design
- [ ] Identity Service
- [ ] Catalog Service
- [ ] API Gateway
- [ ] Enrollment Service
- [ ] Notification Service
- [ ] CI/CD pipeline
- [ ] Azure deployment

---

## License

MIT
