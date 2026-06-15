# Online Learning Platform — Architecture

A microservices-based online learning platform built with .NET 10.
This document captures the architectural decisions made before implementation.

---

## 1. Project goals

- Demonstrate microservices fundamentals: service decomposition, async messaging, database-per-service, API gateway.
- Practice Clean Architecture and CQRS within services.
- Keep the scope deliberately small (4 services, 6 events) so each piece can be implemented and explained with confidence.

Out of scope for the MVP: payments, video upload/streaming, certificates, quizzes, real-time messaging between users.
These were excluded to keep the focus on the microservices patterns rather than feature breadth.

---

## 2. Use cases

### Identity
1. As a user, I want to register as a student or instructor.
2. As a user, I want to log in with email and password.
3. As a user, I want my access token to be refreshable without re-login.

### Catalog
4. As a student, I want to browse and filter all available courses.
5. As a student, I want to see a course detail (including its lessons).
6. As an instructor, I want to create my own course.
7. As an instructor, I want to add lessons to my course.
8. As an instructor, I want to update or delete my own course.

### Enrollment
9. As a student, I want to enroll in a course.
10. As a student, I want to see the courses I am enrolled in.
11. As a student, I want to mark a lesson as completed.
12. As a student, I want to see my course progress as a percentage.
13. As an instructor, I want to see how many students enrolled in my course.

### Review (part of Catalog)
14. As a student, I want to leave a rating and comment for a course I'm enrolled in.
15. As a user, I want to read reviews for a course.

### Notification
16. As a student, I want to receive a welcome email when I register.
17. As a student, I want to receive a confirmation email when I enroll in a course.

---

## 3. Domain model

### Entities
- **User** — represents a person in the system, with a role of Student or Instructor.
- **Course** — created by an instructor, contains lessons.
- **Lesson** — belongs to a course, has an order.
- **Enrollment** — a student's registration to a course.
- **Progress** — lesson-level completion tracking within an enrollment.
- **Review** — a rating + comment a student leaves for a course.
- **Notification** — a message sent to a user (email or in-app).

### Relationships
- A user has one role (Student or Instructor).
- An instructor can own many courses; a course has exactly one instructor.
- A course has many lessons; a lesson belongs to exactly one course.
- A student can enroll in many courses; an enrollment belongs to exactly one student and one course.
- An enrollment can have many lesson-progress records.
- A student can submit one review per course; a course can have many reviews.
- A user can receive many notifications.

---

## 4. Service decomposition

The system is split into 4 services based on bounded contexts:

| Service | Responsibility | Owns |
|---|---|---|
| **Identity** | Authentication, authorization, user management | User, roles, refresh tokens |
| **Catalog** | Course, lesson, and review management | Course, Lesson, Review |
| **Enrollment** | Student enrollments and progress tracking | Enrollment, Progress |
| **Notification** | Email and in-app notifications | Notification log |

Plus one infrastructure component:

- **API Gateway (YARP)** — single entry point, JWT validation, request routing.

### Why these boundaries?

- **Identity** is its own service because authentication concerns are orthogonal to the business domain.
- **Catalog** groups Course, Lesson, and Review because they change together — a course can't exist without lessons, and reviews are tightly bound to a specific course.
- **Enrollment** is separate because enrollments + progress have their own lifecycle independent of course definitions.
- **Notification** is separate because it's purely a side-effect service — purely event-driven, no synchronous endpoints needed.

---

## 5. Communication patterns

### Synchronous (HTTP)
Used when the caller cannot proceed without the response. Examples:
- Enrollment → Catalog: "does this course exist?" before creating an enrollment.
- Catalog → Enrollment (if needed): "is this student enrolled?" before allowing a review.

Resilience note: cross-service HTTP calls are currently direct. Polly (retry + circuit breaker) is on the [roadmap](#11-roadmap--not-yet-built) but not yet applied.

### Asynchronous (Events via RabbitMQ)
Used when the action can complete without waiting on the listener. Examples:
- Identity publishes `UserRegistered` → Notification sends a welcome email.
- Enrollment publishes `StudentEnrolled` → Notification sends a confirmation email.

The publisher does not know or care which services consume the event. This enables loose coupling and the addition of new consumers without modifying publishers.

---

## 6. Key sequence diagrams

### 6.1 Student registers

```
Client  → Gateway   : POST /auth/register {email, password}
Gateway → Identity  : forward (public route, no JWT required)
Identity            : validate input (email format, password strength)
Identity → DB       : check if email exists
Identity → DB       : (if not) save user with hashed password
Identity → RabbitMQ : publish UserRegistered event
Identity → Gateway  : 201 Created
Gateway  → Client   : 201 Created
~~~ asynchronous ~~~
RabbitMQ → Notification : deliver UserRegistered event
Notification         : send welcome email
```

### 6.2 Student enrolls in a course

```
Client     → Gateway     : POST /enrollments {courseId} + JWT
Gateway    → Enrollment  : JWT validate, forward
Enrollment → Catalog     : GET /courses/{id}  (verify course exists)
Catalog    → Enrollment  : course info
Enrollment               : check if already enrolled (idempotency)
Enrollment → DB          : save enrollment
Enrollment → RabbitMQ    : publish StudentEnrolled event
Enrollment → Gateway     : 201 Created
Gateway    → Client      : 201 Created
~~~ asynchronous ~~~
RabbitMQ → Notification : deliver StudentEnrolled event
Notification             : send enrollment confirmation email
```

### 6.3 Instructor adds a lesson

```
Client   → Gateway   : POST /courses/{id}/lessons + JWT
Gateway  → Catalog   : JWT validate, forward
Catalog              : extract instructorId from JWT
Catalog  → DB        : check course owner
Catalog              : if owner != instructorId → return 403
Catalog  → DB        : save lesson
Catalog  → RabbitMQ  : publish LessonAdded event (no consumers yet)
Catalog  → Gateway   : 201 Created
Gateway  → Client    : 201 Created
```

---

## 7. Event catalog

| Event | Publisher | Consumer(s) | Triggered by | Payload |
|---|---|---|---|---|
| `UserRegistered` | Identity | Notification | User registers | `userId`, `email`, `role`, `registeredAt` |
| `CourseCreated` | Catalog | — | Instructor creates a course | `courseId`, `instructorId`, `title`, `createdAt` |
| `LessonAdded` | Catalog | **Enrollment** (increments each enrollment's total lesson count) | Instructor adds a lesson | `lessonId`, `courseId`, `lessonTitle`, `order` |
| `StudentEnrolled` | Enrollment | Notification, **Catalog** (increments course enrollment count) | Student enrolls in a course | `enrollmentId`, `studentId`, `courseId`, `courseTitle` |
| `LessonCompleted` | Enrollment | — | Student marks a lesson complete | `enrollmentId`, `studentId`, `lessonId`, `completedAt` |
| `ReviewSubmitted` | Catalog | — | Student submits a review | `reviewId`, `courseId`, `studentId`, `rating` |

`LessonAdded` and `StudentEnrolled` demonstrate **eventual consistency**: a consumer maintains a local, denormalized count (Enrollment's `TotalLessonCount`, Catalog's `EnrollmentCount`) instead of querying the owning service at read time.

Events without consumers are still published. This follows event-first thinking: future services (Search, Analytics, Achievements) can be added without modifying publishers.

---

## 8. Data ownership

Each service owns its own database. No service reads from another service's database directly.

| Data | Source of truth | Replicated to | Sync mechanism |
|---|---|---|---|
| User (id, email, role) | Identity | — | — |
| Course (id, title, instructorId, ...) | Catalog | Enrollment (id + title only) | Carried inside `StudentEnrolled` event payload |
| Lesson | Catalog | — | — |
| Enrollment | Enrollment | — | — |
| Progress | Enrollment | — | — |
| Review | Catalog | — | — |
| Notification log | Notification | — | — |

Cross-service data needs are met through HTTP calls (for live consistency) or by enriching event payloads (for eventual consistency).

---

## 9. Technology stack

### Per service
- .NET 10 + ASP.NET Core
- Entity Framework Core + PostgreSQL
- MediatR (CQRS), FluentValidation
- Serilog (structured logging)

### Cross-cutting
- API Gateway: **YARP**
- Messaging: **RabbitMQ** via the native `RabbitMQ.Client` (fanout exchanges, manual ack/nack) — see [ADR-002](#adr-002-rabbitmq-via-native-client-not-kafka-not-masstransit)
- Cache: **Redis** (Catalog course listings + idempotency keys)
- Idempotency: Redis-backed, applied to write commands via a MediatR pipeline behavior
- Logging: **Serilog + Seq**
- Auth: **JWT** with shared signing key across services

### DevOps
- Docker Compose (local infrastructure)
- GitHub Actions (CI: restore → build → test)

### Testing
- xUnit + FluentAssertions + NSubstitute (unit tests)

Polly resilience, OpenTelemetry tracing, the Outbox pattern, Testcontainers integration tests, and Azure Container Apps deployment are tracked in section [11. Roadmap](#11-roadmap--not-yet-built).

---

## 10. Architectural decisions (ADRs)

### ADR-001: Use YARP instead of Ocelot for API Gateway
Microsoft's actively maintained reverse proxy; better long-term support and more modern configuration model.

### ADR-002: RabbitMQ via native client (not Kafka, not MassTransit)
Kafka is overkill for a project of this scale; RabbitMQ provides the needed publish/subscribe semantics with a much simpler operational footprint.

I deliberately use the native `RabbitMQ.Client` instead of MassTransit. MassTransit would hide the mechanics behind abstractions — the goal here is to *learn the mechanics*: declaring exchanges and queues, binding routing keys, manual `ack`/`nack`, and consumer lifecycle as `BackgroundService`s. Each consumer owns its connection, channel, fanout exchange, and durable queue explicitly. MassTransit (with its built-in outbox, retry, and scheduling) is a sensible later refactor once these fundamentals are internalized.

### ADR-003: No saga pattern in MVP
The only multi-service transaction in scope (enrollment → notification) is non-critical: if notification fails, the enrollment remains valid. A saga would add complexity without addressing a real correctness need at this scope.

### ADR-004: No outbox pattern in MVP
Acknowledged risk: if an event publish fails after a DB commit, downstream services miss it. For MVP, failures are logged. Outbox pattern is the natural next iteration.

### ADR-005: Database-per-service, even for small services
Even Notification has its own DB. This enforces service independence from day one; merging databases later is far cheaper than splitting them.

### ADR-006: No frontend
A previous project (Restaurant Bill) covered React + full-stack development. This project focuses on backend architecture. Postman collection + Swagger UI are sufficient for demonstration.

---

## 11. Roadmap — not yet built

### Planned (next iterations, rough priority order)
These strengthen the operational/production-readiness story and are the natural next steps:

- **Health checks** — `/health` endpoints with DB / RabbitMQ / Redis readiness probes.
- **Resilience** — Polly (or `Microsoft.Extensions.Http.Resilience`) retry + circuit breaker on cross-service HTTP calls.
- **Distributed tracing** — OpenTelemetry + Jaeger/Tempo across the gateway → service → broker request path. (Serilog + Seq covers logging today, but not end-to-end traces.)
- **Outbox pattern** — atomic DB-commit + event-publish (see ADR-004).
- **Integration tests** — Testcontainers with real PostgreSQL + RabbitMQ.
- **Azure deployment** — Dockerfiles per service + Azure Container Apps (scale-to-zero).

### Intentionally out of scope
Deliberately excluded to keep the focus on microservices patterns rather than feature breadth:

- **Payment processing** — all courses are free. Would be a separate Payment Service with Stripe integration (and a real use case for the saga pattern).
- **Video upload and streaming** — lessons carry an external video URL field (YouTube/Vimeo). A dedicated Media Service with object storage + HLS transcoding would come later.
- **Service mesh** — unnecessary at this scale.
- **CQRS event sourcing** — too complex for the value provided at this scope.

---

## 12. Project structure

```
LearningPlatform/
├── src/
│   ├── ApiGateways/
│   │   └── Gateway.Yarp/
│   ├── Services/
│   │   ├── Identity/
│   │   │   ├── Identity.API/
│   │   │   ├── Identity.Application/
│   │   │   ├── Identity.Domain/
│   │   │   └── Identity.Infrastructure/
│   │   ├── Catalog/
│   │   ├── Enrollment/
│   │   └── Notification/
│   └── BuildingBlocks/
│       ├── BuildingBlocks.Messaging/   # shared event contracts
│       └── BuildingBlocks.Common/       # shared base classes
├── tests/
├── docs/
│   └── architecture.md                  # this file
├── docker-compose.yml
└── README.md
```
