# **Summary Setup Guide**

## **1. Project Setup**

Install prerequisites:

* .NET 8 SDK
* PostgreSQL

---

## **2. Database Setup (Local PostgreSQL)**

Create database manually:

```sql
CREATE DATABASE "claimsdb";
```

---

## **3. Adjust Connection String**

Update both **Project.WebApi** and **Project.Worker** → `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=claimsdb;Username=<your-username>;Password=<your-password>"
  }
}
```

Replace:

* `<your-username>`
* `<your-password>`

---

## **4. Database Migration**

Run script below if migrations is empty
```bash
dotnet ef migrations add InitialCreate -p Project.Infrastructure/Project.Infrastructure.csproj -s Project.WebApi/Project.WebApi.csproj
```

Then Run:
```bash
dotnet ef database update -p Project.Infrastructure/Project.Infrastructure.csproj -s Project.WebApi/Project.WebApi.csproj
```

---

## **5. Running Backend (WebApi)**

```bash
cd Project.WebApi
dotnet run --launch-profile https
```

API available at:

```
http://localhost:5068
https://localhost:7032
```

Swagger:

```
/swagger
```

---

## **6. Running Background Worker**

```bash
cd Project.Worker
dotnet run --environment Development
```

Worker will automatically process claims:

* **Processing → Approved** after **5 minutes**

You must run:

* WebAPI
* Worker
  **simultaneously**

---

## **7. Running Unit Test**

```bash
cd Project.Test
dotnet test
```

---

## **8. Running Unit Test Integration**

```bash
cd Project.Test.Integration
dotnet test
```