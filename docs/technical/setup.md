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

| Service | Provider | Sign up |
|---|---|---|
| PostgreSQL | Neon (free tier) | https://neon.tech |
| MongoDB | Atlas M0 (free forever) | https://cloud.mongodb.com |
| Redis | Upstash (free tier) | https://upstash.com |

After signing up, copy the connection strings into step 4.

**Option B — Local Docker**

```bash
# From project root (requires Docker Desktop)
docker compose up -d
```

---

## 4. Configure environment

```bash
cd backend/Twon.API

dotnet user-secrets init

# Required
dotnet user-secrets set "JWT_SECRET" "generate-a-32-char-random-string-here"

# PostgreSQL (Neon example)
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=ep-xxx.neon.tech;Database=twon;Username=twon_owner;Password=xxx;Ssl Mode=Require"

# MongoDB (Atlas example)
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://user:pass@cluster0.xxxxx.mongodb.net"

# Redis (Upstash example)
dotnet user-secrets set "Redis:ConnectionString" "rediss://default:xxx@xxx.upstash.io:6380"

# Cloudflare R2 (get from R2 dashboard → Manage API tokens)
dotnet user-secrets set "R2:AccountUrl" "https://<account-id>.r2.cloudflarestorage.com"
dotnet user-secrets set "R2:AccessKeyId" "your-access-key-id"
dotnet user-secrets set "R2:SecretAccessKey" "your-secret-access-key"

# Resend (get from resend.com → API Keys)
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

R2 is free up to 10GB storage + 1M writes/month — no credit card needed.
