# Quick API Scaffold

Generate a complete CRUD API endpoint following DDD principles.

## Arguments
- `EntityName` (required): PascalCase entity name (e.g., "Transfer", "GameWeek")
- `Properties` (optional): Comma-separated property list (e.g., "Name:string,Budget:decimal")

## Output
Creates these files with full CRUD implementation:
1. `src/Domain/Entities/{EntityName}.cs` - Domain entity with validation
2. `src/Application/DTOs/{EntityName}Dtos.cs` - Request/response DTOs
3. `src/Application/Interfaces/I{EntityName}Repository.cs` - Repository interface
4. `src/Application/Services/{EntityName}Service.cs` - Service with business logic
5. `src/Infrastructure/Repositories/{EntityName}Repository.cs` - EF Core repository
6. `src/WebApi/Controllers/{EntityName}sController.cs` - REST controller
7. `tests/UnitTests/Application/Services/{EntityName}ServiceTests.cs` - Unit tests
8. `tests/UnitTests/Domain/Entities/{EntityName}Tests.cs` - Domain tests

## Updates
- Registers DI services in `Program.cs`
- Adds `DbSet<{EntityName}>` to `DbContext`

## Example Usage
```
Create API endpoint for Transfer with properties: FromTeamId:Guid,ToTeamId:Guid,PlayerId:Guid,TransferDate:DateTime,Fee:decimal
```

## Coding Standards
- Entity validates in constructor (no nulls, positive values)
- DTOs are records (immutable)
- Service returns DTOs, never domain entities
- Controller uses standard REST routes
- Tests use xUnit, Moq, FluentAssertions, [Theory]
- Repository pattern with async/await throughout
