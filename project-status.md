# CalorieCounter — Project Status

## Project Structure
```
Calorie/
└── CalorieCounter/
    ├── CalorieCounter.sln
    ├── CalorieCounter.AppHost/
    │   ├── AppHost.cs
    │   └── CalorieCounter.AppHost.csproj
    ├── CalorieCounter.ApiService/
    │   ├── Program.cs
    │   ├── appsettings.json
    │   ├── CalorieCounter.ApiService.csproj
    │   ├── Models/
    │   │   ├── User.cs
    │   │   ├── FoodItem.cs
    │   │   ├── FoodEntry.cs
    │   │   ├── BarcodeEntry.cs
    │   │   └── AuthRequests.cs
    │   ├── Data/
    │   │   └── CalorieDbContext.cs
    │   ├── Endpoints/
    │   │   └── AuthEndpoints.cs
    │   └── Services/
    │       └── JwtService.cs
    ├── CalorieCounter.ServiceDefaults/
    │   └── Extensions.cs
    └── CalorieCounter.Tests/
```

## Models
- **User** — IdentityUser\<int\>, adds CreatedAt
- **FoodItem** — Id, InternalId (Guid), Name, CaloriesPer100g, Protein, Carbs, Fat, LastModified
- **FoodEntry** — Id, UserId (FK→User), FoodItemId (FK→FoodItem), AmountInGramm, EatenAt
- **BarcodeEntry** — Id, Code (string, unique), FoodItemId (Guid FK→FoodItem.InternalId)

## Status
- [x] Aspire scaffolding (AppHost + ApiService + ServiceDefaults)
- [x] SQLite resource via CommunityToolkit (13.4.0 host, 9.7.2 EF Core client)
- [x] EF Core + Identity + JWT auth configured
- [x] All models created with FK relationships
- [x] Auth endpoints (POST /api/register, POST /api/login)
- [ ] Food CRUD endpoints
- [ ] Barcode lookup service (Open Food Facts)
- [ ] Food entry logging endpoints
- [ ] HTMX dashboard
- [ ] Android app (future)
- [ ] Huawei step tracker (future)

## Configuration
- **JWT Key** — via Aspire parameter `jwt-key` (secret), set via `dotnet user-secrets set "Parameters:jwt-key" "..."` in AppHost
- **JWT Issuer** — "CalorieCounter" (appsettings.json)
- **JWT Audience** — "CalorieCounterApi" (appsettings.json)
- **JWT Lifetime** — 7 days
- **Db** — Aspire SQLite resource (temp dir)
- **Identity** — RequireUniqueEmail = true

## AppHost.cs
```csharp
var sqlite = builder.AddSqlite("sqlite");
var jwtKey = builder.AddParameter("jwt-key", secret:true);
var apiService = builder.AddProject<...>("apiservice")
    .WithReference(sqlite)
    .WithEnvironment("Jwt__Key", jwtKey)
    .WithHttpHealthCheck("/health");
```

## Pending — Phase 2
- `Endpoints/FoodEndpoints.cs` — GET /api/foods, POST /api/foods, GET /api/foods/{id}
- `Endpoints/EntryEndpoints.cs` — GET /api/entries, POST /api/entries, DELETE /api/entries/{id}
- `Services/BarcodeService.cs` — Open Food Facts API integration
- All protected with `.RequireAuthorization()`
