# Setup Guide — twon-angular-dotnet

> Follow this in order when setting up on a new machine for the first time.

---

## 1. Install prerequisites

| Tool | Version | Link |
|---|---|---|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Node.js | 20+ LTS | https://nodejs.org |
| Git | any | https://git-scm.com |
| Angular CLI | 17+ | `npm install -g @angular/cli` |
| EF Core tools | latest | `dotnet tool install --global dotnet-ef` |

Verify:
```bash
dotnet --version      # 8.x.x
node -v               # v20.x.x
ng version            # Angular CLI: 17.x.x
dotnet ef             # Entity Framework Core .NET Command-line Tools
```

---

## 2. Clone / restore the repo

```bash
# From git bundle (USB / Google Drive transfer)
git clone twon-angular-dotnet.bundle twon-angular-dotnet
cd twon-angular-dotnet
git config user.name "pitipume"
git config user.email "pitipume.boonyawan@gmail.com"

# Or from GitHub
git clone https://github.com/pitipume/twon-angular-dotnet.git
cd twon-angular-dotnet
git config user.name "pitipume"
git config user.email "pitipume.boonyawan@gmail.com"
```

---

## 3. Set up databases

**Option A — Cloud (recommended, no Docker needed)**

| Service | Purpose | Provider | Free tier | Sign up |
|---|---|---|---|---|
| PostgreSQL | Users, orders, payments | Neon | 0.5GB, 1 project | https://neon.tech |
| MongoDB | Ebook/tarot metadata | Atlas M0 | 512MB, forever | https://cloud.mongodb.com |
| Redis | OTP, cache, rate-limit | Upstash | 10K commands/day | https://upstash.com |
| File storage | PDFs, card images | Cloudflare R2 | 10GB, 1M writes/month | https://dash.cloudflare.com → R2 |
| Email | OTP delivery | Resend | 100 emails/day | https://resend.com |

> **R2 note:** Free tier is genuinely free but Cloudflare requires a **payment method on file** to activate R2 (you won't be charged under free limits).

After signing up for all 5, copy the connection strings/keys into step 4.

**Option B — Local Docker**

> No `docker-compose.yml` in this repo yet — use Option A (cloud) for now.

---

## 4. Configure environment

Where to get each value:

| Secret | Where to get it |
|---|---|
| `JWT_SECRET` | Generate yourself — run: `node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"` |
| `ConnectionStrings:Postgres` | Neon dashboard → your project → Connection string |
| `MongoDB:ConnectionString` | Atlas → your cluster → Connect → Drivers → copy URI |
| `Redis:ConnectionString` | Upstash → your database → REST API → copy `UPSTASH_REDIS_URL` |
| `R2:AccountUrl` | Cloudflare → R2 → Overview → Account ID → `https://<id>.r2.cloudflarestorage.com` |
| `R2:AccessKeyId` / `SecretAccessKey` | Cloudflare → R2 → Manage API tokens → Create token |
| `Resend:ApiKey` | Resend → API Keys → Create API key |

```bash
cd backend/Twon.API

dotnet user-secrets init

# Generate JWT_SECRET first:
# node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
dotnet user-secrets set "JWT_SECRET" "paste-generated-secret-here"

# PostgreSQL (Neon — copy from dashboard)
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=ep-xxx.neon.tech;Database=twon;Username=twon_owner;Password=xxx;Ssl Mode=Require"

# MongoDB (Atlas — copy from dashboard)
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://user:pass@cluster0.xxxxx.mongodb.net"

# Redis (Upstash — copy from dashboard)
dotnet user-secrets set "Redis:ConnectionString" "rediss://default:xxx@xxx.upstash.io:6380"

# Cloudflare R2 (R2 dashboard → Manage API tokens)
dotnet user-secrets set "R2:AccountUrl" "https://<account-id>.r2.cloudflarestorage.com"
dotnet user-secrets set "R2:AccessKeyId" "your-access-key-id"
dotnet user-secrets set "R2:SecretAccessKey" "your-secret-access-key"

# Resend (resend.com → API Keys)
dotnet user-secrets set "Resend:ApiKey" "re_your-key"
```

---

## 5. Run database migrations

```bash
cd backend

dotnet ef database update --project Twon.Infrastructure --startup-project Twon.API
```

This creates all PostgreSQL tables (Users, Products, Orders, Payments, LibraryItems, etc.).

---

## 6. Install frontend dependencies

```bash
cd frontend
npm install
```

> **Note:** This project uses **Tailwind CSS v3** (not v4). Angular 17's build tool only supports v2/v3.

---

## 7. Run the project

Open two terminals:

**Terminal 1 — Backend:**
```bash
cd backend
dotnet run --project Twon.API
# → http://localhost:5000
# → Swagger: http://localhost:5000/swagger
```

**Terminal 2 — Frontend:**
```bash
cd frontend
npm start
# → http://localhost:4200
```

---

## Daily workflow

```bash
# Backend
cd backend && dotnet run --project Twon.API

# Frontend
cd frontend && npm start

# After pulling changes that add a new migration
cd backend && dotnet ef database update --project Twon.Infrastructure --startup-project Twon.API
```

---

## Cloudflare R2 setup (one-time)

1. Go to https://dash.cloudflare.com → R2
2. Create a bucket named `twon` (or `twon-dev` for dev)
3. Go to **Manage API tokens** → Create token with **Object Read & Write** on your bucket
4. Copy Account ID, Access Key ID, Secret Access Key → paste into user-secrets above

R2 is free up to 10GB storage + 1M writes/month. A payment method is required to activate R2 on your Cloudflare account, but you won't be charged unless you exceed the free limits.
