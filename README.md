# Talebook.WebApi

A .NET 6+ Web API that fetches and returns the top 200 stories (tales) from Hacker News using clean architecture principles.

---

## 📦 Project Structure

- `Talebook.Application` – DTOs and interfaces
- `Talebook.Infrastructure` – External API calls, caching
- `Talebook.WebApi` – API controllers and DI
- `Talebook.Tests` – Unit tests (xUnit, Moq)

---

## 🚀 How to Run

1. Open solution in **Visual Studio 2022**
2. Set `Talebook.WebApi` as Startup Project
3. Run the application
4. Open browser to:  
   👉 `https://localhost:<port>/swagger/index.html`

---

## 🔧 API Endpoint

| Method | Route                | Description                  |
|--------|----------------------|------------------------------|
| GET    | `/api/tales/top?count=200`     | Get top 200 tales (title/url) |

---

## ✅ Features

- Clean Architecture
- Caching with IMemoryCache
- Swagger UI
- XML Comments
- Dependency Injection
- Unit Testing with 60–70% coverage

---

## 🧪 Running Unit Tests

```bash
dotnet test --collect:"XPlat Code Coverage"
