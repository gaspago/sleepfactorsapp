# Sleep Factors Tracker

A modern Blazor Web App (SSR + interactive server) to track your child's daily factors and identify patterns affecting sleep quality.

## Features

✅ **Account Management** – Register, login, logout with ASP.NET Core Identity  
✅ **Factor Tracking** – Meals (pranzo, merenda, cena, with ingredients), naps, conflicts, bath/doccia, incidents, custom factors  
✅ **Sleep Quality** – Record Bad / So-so / Good for each day  
✅ **Composite Factors** – Meals contain ingredient children for detailed analysis  
✅ **Single-Factor Analysis** – Rank individual factors by risk score (bad/soSo/good ratio)  
✅ **Combined Analysis** – Find factor pairs (interactions) that affect sleep  
✅ **Real-time Updates** – SignalR push from server to browser when new data is saved  
✅ **SQLite Persistence** – Lightweight database, no setup required  
✅ **OOP Patterns** – Base Class + Composite + Strategy pattern comments in code  
✅ **Mobile-friendly** – Bootstrap UI responsive on all devices

## Tech Stack

- **.NET 10.0** with Blazor Web (Server, SSR)
- **Entity Framework Core** with SQLite
- **ASP.NET Core Identity** for auth
- **SignalR** for real-time browser updates
- **Bootstrap 5** for styling

## Local Development

### Prerequisites
- .NET 10 SDK ([download](https://dotnet.microsoft.com/download/dotnet))

### Run Locally

```bash
cd SleepFactorsApp
dotnet run
```

Open `https://localhost:7175` in your browser. Register for a new account.

## Deploy to Render (Free)

See [DEPLOY_RENDER.md](DEPLOY_RENDER.md) for step-by-step instructions.

## Usage

1. **Register** – Create an account
2. **Add Daily Factors** – For each day, select sleep quality and add factors (meals, incidents, etc.)
3. **Results & Ranking** – View risk rankings and excluded factors
4. **Analyze** – Look for patterns: which factors appear most with bad sleep?

## Project Structure

```
SleepFactorsApp/
├── Domain/               # Models with base class + composite pattern
├── Data/                 # EF Core DbContext
├── Services/             # Analysis strategies (Strategy pattern)
├── Hubs/                 # SignalR hub
├── Components/
│   ├── Pages/
│   │   ├── Home.razor    # Landing + auth-aware welcome
│   │   ├── Login.razor   # Login form
│   │   ├── Register.razor # Register form
│   │   ├── Track.razor   # [Authorize] Data entry
│   │   └── Results.razor # [Authorize] Analysis + ranking
│   └── Layout/           # Nav + auth status
├── wwwroot/
│   └── js/sleepFactorsRealtime.js  # SignalR client
├── Dockerfile            # Container image for Render
└── Program.cs            # DI + middleware setup
```

## Design Patterns

- **Factor Base Class** – Unified handling of SimpleFactor, CompositeFactor, MealFactor
- **Composite Pattern** – MealFactor contains Ingredients as children
- **Strategy Pattern** – SingleFactorAnalysisStrategy, CombinedFactorAnalysisStrategy
- **Repository (EF)** – DailyLogService wraps DbContext queries
- **Hub Pattern** – SleepFactorsHub broadcasts data-saved events

## Extra Ideas

- 1-day lag analysis: Factor on day N → sleep quality on night N+1
- Export reports to CSV
- Visual charts: factor frequency vs. sleep quality
- Mobile app (MAUI)

## License

MIT
