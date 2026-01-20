# Fantasy Premier League - DDD Architecture

## Project Structure

```
FantasyPremierLeague/
├── src/
│   ├── Domain/                 # Core domain logic
│   ├── Application/            # Application services & use cases
│   ├── Infrastructure/         # Data access, EF Core
│   └── WebApi/                 # API controllers & React app
└── README.md
```

## Technologies
- .NET 9 (latest stable, .NET 10 not yet released)
- Entity Framework Core (In-Memory Database)
- React 18
- Domain-Driven Design principles

## Setup Instructions

### Backend
```bash
cd src/WebApi
dotnet restore
dotnet run
```

### Frontend
```bash
cd src/WebApi/ClientApp
npm install
npm start
```

## Domain Model
- **Player**: Premier League players with stats
- **Team**: Fantasy teams owned by users
- **GameWeek**: Weekly competition periods
- **Transfer**: Player transfers between teams
