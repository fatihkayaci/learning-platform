# Online Learning Platform — Proje Devam Dokümanı

## Proje teknik özeti

### Stack
- **.NET 10** (LTS, Kasım 2025'te çıktı, Kasım 2028'e kadar destekli)
- **ASP.NET Core**, **EF Core**, **PostgreSQL**
- **MediatR** (CQRS), **FluentValidation**, **BCrypt.Net-Next**
- **Serilog + Seq** (logging)
- **YARP** (API Gateway, sonra eklenecek)
- **RabbitMQ + MassTransit** (asenkron messaging, sonra eklenecek)
- **Redis** (caching, gerekirse sonra)
- **Polly** (resilience, Enrollment'a geçince)
- **Docker + Docker Compose** (lokal)
- **Azure Container Apps** (production deploy hedefi)
- **GitHub Actions** (CI/CD)

### Mimari yapı
- 4 microservice + 1 API Gateway
- Her servis Clean Architecture: Domain / Application / Infrastructure / API
- Her servis kendi PostgreSQL veritabanı (database-per-service)
- Senkron: REST (servisler arası), Asenkron: RabbitMQ events
- JWT auth, shared signing key ile cross-service validation

### Servisler

| Servis | Sorumluluk | Durum |
|---|---|---|
| **Identity** | Auth, JWT, user management | CRUD bitti, exception handling bitti, **izole** |
| **Catalog** | Course, Lesson, Review | CRUD bitti, exception handling bitti, **izole** |
| **Enrollment** | Student enrollments, progress | Henüz başlanmadı |
| **Notification** | Email + in-app notifications | Henüz başlanmadı |
| **API Gateway (YARP)** | Routing, JWT validation | Henüz başlanmadı |

---

## Mimari kararlar (verilmiş)

1. **REST + RabbitMQ kombinasyonu** — senkron için REST, asenkron event'ler için RabbitMQ. gRPC değerlendirildi ama scope için fazla.
2. **Database-per-service** — her servis kendi DB'sinde, hiçbir servis başka servisin DB'sine direkt bağlanmaz.
3. **Clean Architecture + CQRS** — her servis dört katmanlı, MediatR ile command/query ayrımı.
4. **Rich Domain Model** — entity'ler private setter + Create factory method, validation Create içinde.
5. **Layered Defense Validation** — FluentValidation (Application, kullanıcı dostu) + Domain validation (defansif, "var olamaz" kuralları).
6. **Domain Exception Hierarchy** — `DomainException` abstract base, her use case için anlamlı exception isimleri. HTTP status code domain'de YOK, middleware'de pattern matching ile çevriliyor.
7. **Hybrid exception strategy** — specific business rules için ayrı exception class'ları (EmailAlreadyExistsException), generic "not found" durumları için tek class (NotFoundException) kullanılabilir.
8. **Guid.CreateVersion7()** — sıralı ID'ler için, B-tree index performansı.
9. **BCrypt work factor 12** — 2026 sektör standardı.
10. **Database-per-service'in pratik yansıması**: cross-service veri ihtiyacı HTTP call veya event payload zenginleştirmesi ile çözülüyor.

### Scope dışı bırakılanlar (bilinçli)
- Payment / ücretli kurslar
- Video upload / streaming
- Saga pattern (junior scope için fazla)
- Outbox pattern (junior scope için fazla)
- Distributed tracing (Jaeger, OpenTelemetry)
- Frontend (önceki projede React yapıldı, bu proje backend-focused)
- Real-time messaging (chat)
- Sertifika, quiz, forum

---

## Use case'ler (MVP, 17 madde)

### Identity ✓
1. Register (Student veya Instructor olarak)
2. Login (email + password)
3. Refresh token

### Catalog ✓
4. Browse and filter courses (Student)
5. Course detail with lessons (Student)
6. Create course (Instructor)
7. Add lesson to course (Instructor)
8. Update/delete own course (Instructor)


### Enrollment ⏳
1. Enroll in a course
2.  View enrolled courses
3.  Mark lesson as completed
4.  View progress percentage
5.  View enrolled students count (Instructor)
   
### Review (part of Catalog)⏳
1.  Submit review for enrolled course (Student)
2.  Read reviews (everyone)
    
### Notification ⏳
16. Welcome email on registration
17. Confirmation email on enrollment

---

## Event catalog

| Event | Publisher | Consumer | Trigger | Payload |
|---|---|---|---|---|
| `UserRegistered` | Identity | Notification | User registers | userId, email, role |
| `CourseCreated` | Catalog | — | Instructor creates course | courseId, instructorId, title |
| `LessonAdded` | Catalog | — | Instructor adds lesson | lessonId, courseId, title, order |
| `StudentEnrolled` | Enrollment | Notification | Student enrolls | enrollmentId, studentId, courseId, courseTitle |
| `LessonCompleted` | Enrollment | — | Student marks complete | enrollmentId, studentId, lessonId |
| `ReviewSubmitted` | Catalog | — | Review created | reviewId, courseId, studentId, rating |

Henüz hiçbir event publish edilmiyor — RabbitMQ kurulumu sıradaki adımlardan.

---

## Şu anki konum ve sıradaki adımlar

### Bitti
- ✅ Repo açıldı, README + architecture.md hazır
- ✅ Identity Service: Domain, Application, Infrastructure, API katmanları
- ✅ Identity'de register, login, refresh token endpoint'leri
- ✅ Identity'de exception hierarchy + GlobalExceptionMiddleware
- ✅ Catalog Service: aynı yapı, 8 use case CRUD çalışıyor
- ✅ Catalog'da exception handling
- ✅ Her servis kendi PostgreSQL'inde (docker-compose'da DB'ler)
- ✅ İki servis de izole olarak Postman/Swagger'dan test edilebilir

### Şu an yapacağım (öncelik sırası)
1. **Identity + Catalog arasında JWT cross-service** — Identity'nin verdiği JWT'yi Catalog kabul ediyor mu, test et. Aynı signing key, audience, issuer.
2. **Authorization Catalog'da test** — Instructor A, Instructor B'nin kursuna update isteği atınca 403 dönüyor mu.
3. **RabbitMQ + MassTransit kurulumu** — Identity'den UserRegistered event publish ettir, Catalog'da deneme amaçlı bir consumer ekle (sonra Notification'a taşınacak).
4. **Bu altyapı oturunca Enrollment Service'e başla** — pattern'ler hazır olduğu için daha hızlı yazılacak.

### Daha sonra
- Notification Service
- API Gateway (YARP)
- Docker Compose tüm sistemi tek komutla ayağa kaldırsın
- Integration testler (Testcontainers)
- Azure deploy
- CI/CD (GitHub Actions)

---

## Önemli "neden böyle yapıyoruz" notları

### Exception hierarchy
- `DomainException` abstract base, hepsi bundan türüyor
- HTTP status middleware'de **pattern matching** ile çevriliyor, exception'ın **içinde StatusCode property YOK**
- Specific business hatalar (`EmailAlreadyExistsException`, `UnauthorizedCourseAccessException`) için ayrı sınıflar
- Generic "not found" için tek sınıf kullanılabilir (`NotFoundException(entityName, id)`)
- Domain hiç HTTP/transport detayı bilmez

### Validation iki katmanda
- **FluentValidation (Application)**: kullanıcı dostu, format/uzunluk kontrolleri
- **Domain validation (entity Create)**: defansif, "var olamaz" kontrolleri
- İkisi farklı amaca hizmet, "tekrar değil"

### Rich Domain Model
- Entity property'leri `private set`
- `new User()` yerine `User.Create(...)` factory method
- Validation Create içinde, entity asla geçersiz state'e geçemiyor

### JWT cross-service
- Identity JWT üretir, signing key appsettings'te
- Catalog (ve diğerleri) **aynı signing key** ile JWT'yi kendi başına validate eder, Identity'ye sormaz
- Stateless authentication, her servis bağımsız doğrular

---

## Git akışı

- main protected (kuralı henüz koymadım ama mantığı uyguluyorum)
- Her feature için ayrı branch: `feat/xxx`, `fix/xxx`, `docs/xxx`
- Conventional Commits: `feat(scope): description`
- Her büyük iş için ayrı PR, anlamlı description
- Şimdiye kadar 2-3 anlamlı commit; iskelet + Identity + Catalog + exception handling

## LinkedIn stratejisi

- Düzenli post atıyorum, milestone-bazlı
- Stack listesi değil, **öğrenme hikayesi** odaklı
- External link post'a koymuyorum (algoritma cezası)
- Hafta içi 09:00-10:00 veya 12:00-13:00 (TR) ideal saatler