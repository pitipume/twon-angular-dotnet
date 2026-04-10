# Deployment Guide — twon-angular-dotnet

---

## Hosting Stack

| Part | Service | Free Tier | Notes |
|---|---|---|---|
| Angular frontend | Netlify | Yes | Static files after `ng build` |
| .NET backend | Railway | Yes / $5/mo | Runs `dotnet run` in cloud |
| PostgreSQL | Neon | 0.5GB | Already configured |
| MongoDB | Atlas M0 | 512MB | Already configured |
| Redis | Upstash | 10K cmd/day | Already configured |
| File storage | Cloudflare R2 | 10GB | Already configured |
| Email | Resend | 100/day | Already configured |

---

## Deploy Backend — Railway

1. Go to [railway.app](https://railway.app) → New Project → Deploy from GitHub repo
2. Select `twon-angular-dotnet` → set **Root directory** to `backend`
3. Railway auto-detects .NET — set:
   - **Build command:** `dotnet publish -c Release -o out`
   - **Start command:** `dotnet out/Twon.API.dll`
4. Go to your service → **Variables** tab → add all secrets:

```
JWT_SECRET                          = your-generated-secret
ConnectionStrings__Postgres         = Host=ep-xxx.neon.tech;Database=twon;Username=twon_owner;Password=xxx;Ssl Mode=Require
MongoDB__ConnectionString           = mongodb+srv://user:pass@cluster0.xxxxx.mongodb.net
Redis__ConnectionString             = rediss://default:xxx@xxx.upstash.io:6380
R2__AccountUrl                      = https://<account-id>.r2.cloudflarestorage.com
R2__BucketName                      = twon
R2__AccessKeyId                     = your-access-key-id
R2__SecretAccessKey                 = your-secret-access-key
Resend__ApiKey                      = re_your-key
ASPNETCORE_ENVIRONMENT              = Production
```

> **Note:** .NET nested config keys use `__` (double underscore) as separator in environment variables, not `:`.

5. Railway gives you a URL like `https://twon-api.up.railway.app` — save this for the frontend.

---

## Deploy Frontend — Netlify

1. Go to [netlify.com](https://netlify.com) → Add new site → Import from GitHub
2. Select `twon-angular-dotnet` → set:
   - **Base directory:** `frontend`
   - **Build command:** `ng build --configuration production`
   - **Publish directory:** `frontend/dist/frontend/browser`
3. Go to **Site settings → Environment variables** → add:

```
API_URL = https://twon-api.up.railway.app
```

> Angular environment variables are **compiled into the bundle** at build time via `src/environments/environment.prod.ts` — not read at runtime like Node.js apps.

4. Netlify gives you a URL like `https://twon.netlify.app`

---

## Environment Files (Local vs Production)

### Backend

| File | Purpose | Commit? |
|---|---|---|
| `appsettings.json` | Non-sensitive defaults (port, CORS) | Yes |
| `appsettings.Development.json` | Local dev overrides | Yes (no secrets) |
| `appsettings.Production.json` | Do not use — use Railway env vars instead | No |

### Frontend

| File | Purpose | Commit? |
|---|---|---|
| `src/environments/environment.ts` | Local dev (`apiUrl: http://localhost:5000`) | Yes |
| `src/environments/environment.prod.ts` | Production (`apiUrl: https://twon-api.up.railway.app`) | Yes |

---

## CORS — Update After Deploy

After getting your Netlify URL, update the allowed origins in:

`backend/Twon.API/Program.cs` — add your Netlify URL to the CORS policy.

---

## Migration to AWS (Future)

| Current | AWS Equivalent | Change needed |
|---|---|---|
| Railway (.NET) | ECS Fargate | Dockerfile + env vars |
| Neon PostgreSQL | RDS PostgreSQL | Change connection string |
| Netlify (Angular) | S3 + CloudFront | `ng build` output → S3 bucket |
| Cloudflare R2 | AWS S3 | Change endpoint URL in config |
| Upstash Redis | ElastiCache | Change connection string |
| Resend | AWS SES | Change email service class |

All changes are **config/env var only** — no business logic changes required.
