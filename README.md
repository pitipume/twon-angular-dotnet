# Twon Platform — Angular + .NET

Dual-commerce platform: **Ebook Shop** + **E-Tarot Card Shop**

---

## Docs Structure

| Folder | Purpose |
|---|---|
| [`docs/business/`](docs/business/) | Product overview, business rules, user roles |
| [`docs/data-models/`](docs/data-models/) | User, product, order, library schemas |
| [`docs/api-contracts/`](docs/api-contracts/) | Auth, catalog, store, payment, library, admin |
| [`docs/infrastructure/`](docs/infrastructure/) | Cloud services, storage |
| [`docs/technical/`](docs/technical/) | Setup, commands, deployment |

---

## Quick Start

```bash
# 1. Install dependencies
cd frontend && npm install

# 2. Run migrations
cd backend && dotnet ef database update --project Twon.Infrastructure --startup-project Twon.API

# 3. Run backend (Terminal 1)
cd backend && dotnet run --project Twon.API
# → http://localhost:5000
# → http://localhost:5000/swagger

# 4. Run frontend (Terminal 2)
cd frontend && npm start
# → http://localhost:4200
```

---

## Docs

| File | What's in it |
|---|---|
| [`CLAUDE.md`](CLAUDE.md) | Tech stack, layer architecture, NestJS→.NET mapping |
| [`docs/technical/setup.md`](docs/technical/setup.md) | First-time setup (prerequisites, cloud services, secrets) |
| [`docs/technical/commands.md`](docs/technical/commands.md) | Daily development commands |
| [`docs/technical/deployment.md`](docs/technical/deployment.md) | Deploy to Railway (backend) + Netlify (frontend) |

---

## Stack

| Part | Technology |
|---|---|
| Frontend | Angular 17, Tailwind CSS v3, GSAP |
| Backend | ASP.NET Core 8, MediatR, EF Core 8 |
| Database | PostgreSQL (Neon) + MongoDB (Atlas) + Redis (Upstash) |
| Storage | Cloudflare R2 |
| Email | Resend |
