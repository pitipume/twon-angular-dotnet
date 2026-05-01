# Infrastructure — External Services

All services below are used by every Twon implementation regardless of stack.

---

## Databases

| Service | Purpose | Provider | Free tier |
|---|---|---|---|
| PostgreSQL | Users, orders, payments, library | Neon (cloud) or Docker | Neon free tier |
| MongoDB | Ebook metadata, tarot deck config | Atlas M0 | Forever free |
| Redis | OTP, sessions, rate limiting, cache | Upstash (cloud) or Docker | Free tier |

### Local (Docker)

**.NET (`appsettings.Development.json`):**
```json
"ConnectionStrings": { "Postgres": "Host=localhost;Database=twon;Username=twon;Password=twon_dev" },
"MongoDB": { "ConnectionString": "mongodb://twon:twon_dev@localhost:27017", "DatabaseName": "twon" },
"Redis": { "ConnectionString": "localhost:6379" }
```

### Cloud

**.NET (`dotnet user-secrets` or Railway env vars):**
```
ConnectionStrings:Postgres   = Host=ep-xxx.neon.tech;Database=twon;Username=twon_owner;Password=xxx;Ssl Mode=Require
MongoDB:ConnectionString     = mongodb+srv://user:pass@cluster0.xxxxx.mongodb.net
Redis:ConnectionString       = rediss://default:xxx@xxx.upstash.io:6380
```

> **Note:** .NET nested config keys use `:` in `appsettings.json` and `dotnet user-secrets`,
> and `__` (double underscore) as environment variables on Railway.

---

## File Storage — Cloudflare R2

S3-compatible object storage. 10GB free, no egress fees.

**.NET config keys:**
```
R2:AccountUrl       = https://<account-id>.r2.cloudflarestorage.com
R2:BucketName       = twon
R2:AccessKeyId      = (from R2 → Manage API Tokens)
R2:SecretAccessKey  = (from R2 → Manage API Tokens — shown once only)
```

Sign up: **cloudflare.com** → R2

---

## Email — Resend

OTP delivery and order confirmations. 100 emails/day free.

**.NET config keys:**
```
Resend:ApiKey  = re_your-key    # leave blank in dev — OTP prints to console
```

Sign up: **resend.com**

> In development, leave `RESEND_API_KEY` blank.
> The backend will print OTP codes to the terminal instead of sending emails.

---

## Hosting

| Service | What | Provider | Cost |
|---|---|---|---|
| Frontend | Next.js / Angular | Vercel | Free |
| Backend | NestJS / .NET API | Railway | Free / $5/month |
| Domain | twon.com (future) | Cloudflare | ~$10/year |

---

## Monitoring (future)

| Service | Purpose | Cost |
|---|---|---|
| Sentry | Error tracking | Free tier |
| Railway metrics | Backend CPU/RAM | Included |
