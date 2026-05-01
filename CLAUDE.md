# CLAUDE.md — Twon Platform (Angular + C# .NET)

> **Read `docs/` first** for business rules, data models, and API contracts.
> [`docs/business/`](docs/business/) · [`docs/data-models/`](docs/data-models/) · [`docs/api-contracts/`](docs/api-contracts/)
>
> This file covers only tech-stack decisions for this repo.

---

## Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 17+ (standalone components, signals) |
| Frontend styling | Tailwind CSS v4 |
| Frontend state | Angular signals + services (no NgRx for now) |
| Frontend HTTP | Angular HttpClient + interceptors |
| Frontend forms | Angular Reactive Forms + Zod |
| Frontend animation | GSAP (tarot shuffle) + Angular Animations |
| Frontend PDF | ng2-pdf-viewer |
| Backend | ASP.NET Core 8 Web API |
| Backend CQRS | MediatR (= @nestjs/cqrs in NestJS version) |
| Backend ORM | Entity Framework Core 8 (PostgreSQL via Npgsql) |
| Backend MongoDB | MongoDB.Driver (official C# driver) |
| Backend Redis | StackExchange.Redis |
| Backend validation | FluentValidation |
| Backend email | Resend C# SDK |
| Backend storage | AWSSDK.S3 (Cloudflare R2 is S3-compatible) |

---

## Layer architecture (identical pattern to NestJS version)

```
Controller  → mediator.Send(command/query)        — no logic
Handler     → IRequestHandler<Command, Result>    — normalize → validate → call manager
Manager     → class XManager                      — business logic, orchestrates services
Service     → class XService                      — data access only, calls repository
Repository  → EF Core / MongoDB.Driver            — no logic
```

### NestJS → .NET mapping (for reference)

| NestJS | .NET |
|---|---|
| `@Module()` | Feature folder + DI in `Program.cs` |
| `@Controller()` | `[ApiController]` + `[Route()]` |
| `@CommandHandler` | `IRequestHandler<Command, Result>` |
| `CommandBus.execute()` | `await _mediator.Send(command)` |
| `@Injectable()` | `services.AddScoped<Interface, Impl>()` |
| `PrismaService` | `TwonDbContext : DbContext` |
| `JwtAuthGuard` | `[Authorize]` + JWT Bearer middleware |
| `@Roles()` | `[Authorize(Roles = "Admin")]` |
| `@CurrentUser()` | `User.FindFirst(ClaimTypes.NameIdentifier)` |
| `class-validator` | `FluentValidation` |
| `ApiResponse<T>` | `BaseResult<T>` (same pattern) |
| `BullMQ` | `Hangfire` |

---

## Project structure

```
twon-angular-dotnet/
├── CLAUDE.md
├── frontend/                ← Angular app
└── backend/                 ← ASP.NET Core solution
    ├── Twon.sln
    ├── Twon.API/
    ├── Twon.Application/
    ├── Twon.Domain/
    └── Twon.Infrastructure/
```

---

## Response envelope (identical to NestJS version)

```csharp
public class BaseResult<T>
{
    public string Code { get; set; }       // "A001" success, "A002" failure, "A401", "A404", "A409"
    public string Status { get; set; }     // "success" | "failure"
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static BaseResult<T> Success(T data) => ...
    public static BaseResult<T> Failure(string message) => ...
    public static BaseResult<T> NotFound(string message = "Not found.") => ...
    public static BaseResult<T> Conflict(string message) => ...
    public static BaseResult<T> Unauthorized(string message = "Unauthorized.") => ...
}
```

---

## AI Assistant Guidelines

- Always use `Edit` tool for existing files — never `Write` on existing files
- `Write` only for brand new files
- Follow the layer pattern above — never put business logic in controllers
- MediatR = the CQRS bus, same concept as NestJS CommandBus/QueryBus
- When in doubt about business rules → check `docs/`
- **Keep docs in sync with code** — whenever you add/change/remove an endpoint, entity field, enum value, or config key, update the relevant doc in `docs/` in the same response. Docs are the source of truth for future AI sessions.
