# Claude Command: Create New API Endpoint

## Usage

```
@create-api {EntityName} {Action}
```

**Examples:**
- `@create-api Transfer Create`
- `@create-api GameWeek Activate`
- `@create-api Player UpdatePrice`

---

## Scaffolding Template

When asked to create a new API endpoint, generate the following files following this exact structure:

### 1. Domain Entity (if new entity)

**File:** `src/Domain/Entities/{EntityName}.cs`

```csharp
using Domain.Common;

namespace Domain.Entities;

public class {EntityName} : Entity
{{
    // Properties

    private {EntityName}() {{ }} // EF Core

    public {EntityName}(/* required params */)
    {{
        // Validation and initialization
    }}

    // Domain methods for business rules
}}
```

---

### 2. DTOs

**File:** `src/Application/DTOs/{EntityName}Dtos.cs`

```csharp
namespace Application.DTOs;

// Request DTOs
public record Create{EntityName}Request(/* properties */);
public record Update{EntityName}Request(/* properties */);

// Response DTOs
public record {EntityName}Dto(
    Guid Id,
    /* other properties */
);
```

---

### 3. Repository Interface

**File:** `src/Application/Interfaces/I{EntityName}Repository.cs`

```csharp
using Domain.Entities;

namespace Application.Interfaces;

public interface I{EntityName}Repository
{{
    Task<{EntityName}?> GetByIdAsync(Guid id);
    Task<IEnumerable<{EntityName}>> GetAllAsync();
    Task<{EntityName}> AddAsync({EntityName} entity);
    Task UpdateAsync({EntityName} entity);
    Task DeleteAsync({EntityName} entity);
}}
```

---

### 4. Application Service

**File:** `src/Application/Services/{EntityName}Service.cs`

```csharp
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public interface I{EntityName}Service
{{
    Task<{EntityName}Dto> CreateAsync(Create{EntityName}Request request);
    Task<{EntityName}Dto?> GetByIdAsync(Guid id);
    Task<IEnumerable<{EntityName}Dto>> GetAllAsync();
    Task<{EntityName}Dto?> UpdateAsync(Guid id, Update{EntityName}Request request);
    Task<bool> DeleteAsync(Guid id);
}}

public class {EntityName}Service : I{EntityName}Service
{{
    private readonly I{EntityName}Repository _repository;

    public {EntityName}Service(I{EntityName}Repository repository)
    {{
        _repository = repository;
    }}

    public async Task<{EntityName}Dto> CreateAsync(Create{EntityName}Request request)
    {{
        // Implementation
    }}

    public async Task<{EntityName}Dto?> GetByIdAsync(Guid id)
    {{
        // Implementation
    }}

    public async Task<IEnumerable<{EntityName}Dto>> GetAllAsync()
    {{
        // Implementation
    }}

    public async Task<{EntityName}Dto?> UpdateAsync(Guid id, Update{EntityName}Request request)
    {{
        // Implementation
    }}

    public async Task<bool> DeleteAsync(Guid id)
    {{
        // Implementation
    }}
}}
```

---

### 5. Repository Implementation

**File:** `src/Infrastructure/Repositories/{EntityName}Repository.cs`

```csharp
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class {EntityName}Repository : I{EntityName}Repository
{{
    private readonly FantasyPremierLeagueDbContext _context;

    public {EntityName}Repository(FantasyPremierLeagueDbContext context)
    {{
        _context = context;
    }}

    public async Task<{EntityName}?> GetByIdAsync(Guid id)
    {{
        return await _context.{EntityName}s.FindAsync(id);
    }}

    public async Task<IEnumerable<{EntityName}>> GetAllAsync()
    {{
        return await _context.{EntityName}s.ToListAsync();
    }}

    public async Task<{EntityName}> AddAsync({EntityName} entity)
    {{
        _context.{EntityName}s.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }}

    public async Task UpdateAsync({EntityName} entity)
    {{
        _context.{EntityName}s.Update(entity);
        await _context.SaveChangesAsync();
    }}

    public async Task DeleteAsync({EntityName} entity)
    {{
        _context.{EntityName}s.Remove(entity);
        await _context.SaveChangesAsync();
    }}
}}
```

---

### 6. API Controller

**File:** `src/WebApi/Controllers/{EntityName}sController.cs`

```csharp
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class {EntityName}sController : ControllerBase
{{
    private readonly I{EntityName}Service _service;

    public {EntityName}sController(I{EntityName}Service service)
    {{
        _service = service;
    }}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<{EntityName}Dto>>> GetAll()
    {{
        var result = await _service.GetAllAsync();
        return Ok(result);
    }}

    [HttpGet("{{id}}")]
    public async Task<ActionResult<{EntityName}Dto>> GetById(Guid id)
    {{
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }}

    [HttpPost]
    public async Task<ActionResult<{EntityName}Dto>> Create(Create{EntityName}Request request)
    {{
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new {{ id = result.Id }}, result);
    }}

    [HttpPut("{{id}}")]
    public async Task<ActionResult<{EntityName}Dto>> Update(Guid id, Update{EntityName}Request request)
    {{
        var result = await _service.UpdateAsync(id, request);
        if (result == null)
            return NotFound();
        return Ok(result);
    }}

    [HttpDelete("{{id}}")]
    public async Task<IActionResult> Delete(Guid id)
    {{
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }}
}}
```

---

### 7. DbContext Update

**Update:** `src/Infrastructure/Persistence/FantasyPremierLeagueDbContext.cs`

Add to the DbContext class:
```csharp
public DbSet<{EntityName}> {EntityName}s {{ get; set; }} = null!;
```

---

### 8. Dependency Injection Registration

**Update:** `src/WebApi/Program.cs`

Add to the DI container:
```csharp
builder.Services.AddScoped<I{EntityName}Repository, {EntityName}Repository>();
builder.Services.AddScoped<I{EntityName}Service, {EntityName}Service>();
```

---

### 9. Unit Tests

**File:** `tests/UnitTests/Application/Services/{EntityName}ServiceTests.cs`

```csharp
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace UnitTests.Application.Services;

public class {EntityName}ServiceTests
{{
    private readonly Fixture _fixture;
    private readonly Mock<I{EntityName}Repository> _repositoryMock;
    private readonly {EntityName}Service _sut;

    public {EntityName}ServiceTests()
    {{
        _fixture = new Fixture();
        _repositoryMock = new Mock<I{EntityName}Repository>();
        _sut = new {EntityName}Service(_repositoryMock.Object);
    }}

    [Theory, AutoData]
    public async Task CreateAsync_WhenValidRequest_ShouldReturnDto(Create{EntityName}Request request)
    {{
        // Arrange
        var entity = new {EntityName}(/*...*/);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<{EntityName}>())).ReturnsAsync(entity);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<{EntityName}>()), Times.Once);
    }}

    [Theory, AutoData]
    public async Task GetById_WhenExists_ShouldReturnDto(Guid id)
    {{
        // Arrange
        var entity = new {EntityName}(/*...*/);
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
    }}

    [Theory, AutoData]
    public async Task GetById_WhenNotExists_ShouldReturnNull(Guid id)
    {{
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(({EntityName}?)null);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }}
}}
```

**File:** `tests/UnitTests/Domain/Entities/{EntityName}Tests.cs`

```csharp
using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Domain.Entities;

public class {EntityName}Tests
{{
    [Theory]
    [InlineData("valid data")]
    public void Constructor_WhenValidData_ShouldCreateEntity(string data)
    {{
        // Arrange & Act
        var entity = new {EntityName}(/*...*/);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
    }}

    [Theory]
    [InlineData(null)]
    public void Constructor_WhenInvalidData_ShouldThrowException(string? invalidData)
    {{
        // Arrange & Act
        Action act = () => new {EntityName}(invalidData!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }}
}}
```

---

## Post-Creation Steps

After scaffolding, run:

```bash
dotnet build
dotnet test
```

Fix any compilation errors and ensure tests pass.

---

## Custom Action Endpoints

For custom actions (not CRUD), add to controller:

```csharp
[HttpPost("{{id}}/action-name")]
public async Task<ActionResult> ActionName(Guid id, ActionRequest request)
{{
    var result = await _service.ActionNameAsync(id, request);
    return Ok(result);
}}
```

Use `POST` for actions that modify state, `GET` for read-only operations.

---

## Rules

1. **Never modify existing working code** - Only add new files
2. **Follow existing patterns** - Match code style of Player/Team
3. **All entities must inherit from Entity base class**
4. **Use record types for DTOs**
5. **Repository interfaces go in Application layer**
6. **Repository implementations go in Infrastructure layer**
7. **Controllers must be thin** - Delegate to services
8. **Always include unit tests** - Use [Theory] not [Fact]
9. **Run build and test after** - Verify everything compiles
