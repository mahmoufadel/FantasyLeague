# Fantasy Premier League - Setup Guide

## Overview
This is a full-stack Fantasy Premier League application built with:
- **Backend**: .NET 9, ASP.NET Core Web API, Entity Framework Core (In-Memory Database)
- **Architecture**: Domain-Driven Design (DDD) with Clean Architecture
- **Frontend**: React 18
- **API Documentation**: Swagger/OpenAPI

## Project Structure

```
FantasyPremierLeague/
├── src/
│   ├── Domain/                     # Core business logic and entities
│   │   ├── Common/                 # Base classes (Entity, IDomainEvent)
│   │   └── Entities/               # Domain entities (Player, Team, GameWeek)
│   │
│   ├── Application/                # Application services and DTOs
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Interfaces/             # Repository interfaces
│   │   └── Services/               # Application services
│   │
│   ├── Infrastructure/             # Data access and external services
│   │   ├── Persistence/            # EF Core DbContext
│   │   └── Repositories/           # Repository implementations
│   │
│   └── WebApi/                     # API and React frontend
│       ├── Controllers/            # API controllers
│       ├── ClientApp/              # React application
│       └── Program.cs              # Application entry point
│
└── FantasyPremierLeague.sln       # Solution file
```

## Prerequisites

1. **.NET 9 SDK** (or .NET 8 if 9 is not available)
   - Download from: https://dotnet.microsoft.com/download

2. **Node.js** (v18 or higher)
   - Download from: https://nodejs.org/

3. **Code Editor** (recommended: Visual Studio 2022, VS Code, or Rider)

## Getting Started

### Step 1: Backend Setup

1. Navigate to the WebApi project:
```bash
cd src/WebApi
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the API:
```bash
dotnet run
```

The API will start on `http://localhost:5000` and `https://localhost:5001`

5. Open Swagger UI:
   - Navigate to `http://localhost:5000/swagger` in your browser
   - You can test all API endpoints here

### Step 2: Frontend Setup

1. Navigate to the React app:
```bash
cd src/WebApi/ClientApp
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

The React app will open at `http://localhost:3000`

## Domain Model Explained

### DDD Layers

1. **Domain Layer** (Core)
   - Contains business logic and rules
   - No dependencies on other layers
   - Entities: Player, Team, GameWeek
   - Value objects and domain events

2. **Application Layer**
   - Use cases and application services
   - DTOs for data transfer
   - Repository interfaces
   - Depends only on Domain layer

3. **Infrastructure Layer**
   - Data access with EF Core
   - Repository implementations
   - External service integrations
   - Depends on Application and Domain

4. **Presentation Layer** (WebApi)
   - API controllers
   - React frontend
   - Depends on Application layer

### Key Entities

#### Player (Aggregate Root)
- Represents a Premier League player
- Properties: Name, Position, Club, Price, Points, Stats
- Business rules: Price validation, stats updates

#### Team (Aggregate Root)
- Represents a fantasy team
- Properties: Name, Manager, Budget, Points, Players
- Business rules: Max 15 players, budget constraints
- Contains TeamPlayer value objects

#### GameWeek
- Represents a competition week
- Properties: Week number, dates, status
- Business rules: Activation and completion logic

## API Endpoints

### Players
- `GET /api/players` - Get all players
- `GET /api/players/{id}` - Get player by ID
- `GET /api/players/position/{position}` - Get players by position
- `POST /api/players` - Create new player
- `PUT /api/players/{id}/stats` - Update player stats
- `DELETE /api/players/{id}` - Delete player

### Teams
- `GET /api/teams` - Get all teams
- `GET /api/teams/{id}` - Get team by ID
- `POST /api/teams` - Create new team
- `POST /api/teams/add-player` - Add player to team
- `DELETE /api/teams/{teamId}/players/{playerId}` - Remove player
- `DELETE /api/teams/{id}` - Delete team

## Features

### Backend Features
- ✅ Clean Architecture with DDD
- ✅ Entity Framework Core with In-Memory Database
- ✅ Repository Pattern
- ✅ RESTful API design
- ✅ Swagger API documentation
- ✅ CORS enabled for React frontend
- ✅ Automatic data seeding

### Frontend Features
- ✅ Modern React with Hooks
- ✅ Player browsing with position filters
- ✅ Team creation and management
- ✅ Player transfers (add/remove)
- ✅ Budget tracking
- ✅ Responsive design
- ✅ Error handling and loading states

## Testing the Application

1. **Create a Team**:
   - Go to "My Teams" tab
   - Click "Create New Team"
   - Enter team name and manager name

2. **Add Players**:
   - Click "Add Players" on your team
   - Browse available players
   - Add players to your team (budget permitting)

3. **Manage Squad**:
   - View your team's budget and points
   - Remove players to free up budget
   - Maximum 15 players per team

## Database

The application uses EF Core In-Memory Database:
- Data persists only during application runtime
- Automatically seeded with 15 Premier League players
- Perfect for development and testing
- To use a persistent database (SQL Server, PostgreSQL), update `Program.cs`:

```csharp
builder.Services.AddDbContext<FantasyPremierLeagueDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Development Notes

### Adding New Features

1. **New Domain Entity**:
   - Add entity class in `Domain/Entities`
   - Add to DbContext in `Infrastructure/Persistence`
   - Create repository interface in `Application/Interfaces`
   - Implement repository in `Infrastructure/Repositories`

2. **New API Endpoint**:
   - Create service method in `Application/Services`
   - Add controller action in `WebApi/Controllers`
   - Update React components to consume the endpoint

### Architecture Benefits

- **Testability**: Each layer can be tested independently
- **Maintainability**: Clear separation of concerns
- **Flexibility**: Easy to change infrastructure (database, etc.)
- **Domain Focus**: Business logic is isolated and protected

## Troubleshooting

### Port Already in Use
If port 5000 or 3000 is in use:
```bash
# Backend: Edit Properties/launchSettings.json to change ports
# Frontend: Set PORT environment variable
PORT=3001 npm start
```

### CORS Issues
Ensure the API URL in `ClientApp/src/services/api.js` matches your backend URL.

### Database Reset
To reset the database, just restart the application (in-memory database is cleared).

## Next Steps

Potential enhancements:
- Add user authentication
- Implement gameweek scoring
- Add player statistics charts
- Create leaderboards
- Add transfer history
- Implement team validation rules
- Add more detailed player information

## License

This is a sample project for educational purposes.

---

Built with ❤️ using .NET 9, Entity Framework Core, and React
