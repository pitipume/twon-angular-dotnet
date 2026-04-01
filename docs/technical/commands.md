# CLI Commands Reference — twon-angular-dotnet

> Run these in your own terminal (PowerShell, Git Bash, or Windows Terminal).
> Claude Code's bash tool cannot run these for you — open a terminal and run them directly.

---

## Backend (.NET)

Run from `backend/`:

```bash
cd backend

# Run the API (http://localhost:5000)
dotnet run --project Twon.API

# Build all projects
dotnet build Twon.sln

# Restore NuGet packages
dotnet restore Twon.sln
```

---

## EF Core Migrations

Run from `backend/Twon.API/` (needs the startup project for DI/config):

```bash
cd backend

# Add a new migration
dotnet ef migrations add <MigrationName> --project Twon.Infrastructure --startup-project Twon.API

# Apply migrations to database
dotnet ef database update --project Twon.Infrastructure --startup-project Twon.API

# List all migrations
dotnet ef migrations list --project Twon.Infrastructure --startup-project Twon.API

# Remove last migration (if not yet applied)
dotnet ef migrations remove --project Twon.Infrastructure --startup-project Twon.API

# Drop the database (destructive!)
dotnet ef database drop --project Twon.Infrastructure --startup-project Twon.API
```

> First time: `dotnet tool install --global dotnet-ef`

---

## Frontend (Angular)

Run from `frontend/`:

```bash
cd frontend

# Install dependencies (first time only)
npm install

# Start dev server (http://localhost:4200)
npm start

# Build for production
npm run build

# Generate a new standalone component
npx ng generate component features/xxx/yyy --standalone
```

---

## Git (local config for this repo)

```bash
# Set identity for this repo only (not global)
git config user.name "pitipume"
git config user.email "pitipume.boonyawan@gmail.com"

# Verify
git config user.name
git config user.email
```

---

## Environment variables

The backend reads config from `appsettings.json` + environment variables.
Secrets (`JWT_SECRET`, R2 keys, Resend API key) must be set as env vars or via .NET User Secrets.

```bash
# .NET User Secrets (dev only — stored outside the repo)
cd backend/Twon.API
dotnet user-secrets init
dotnet user-secrets set "JWT_SECRET" "your-256-bit-secret-here"
dotnet user-secrets set "R2:AccessKeyId" "your-r2-key"
dotnet user-secrets set "R2:SecretAccessKey" "your-r2-secret"
dotnet user-secrets set "Resend:ApiKey" "re_your-key"

# List all secrets
dotnet user-secrets list
```
