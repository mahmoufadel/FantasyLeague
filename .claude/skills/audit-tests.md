---
description: Audit all unit and integration test files for naming, attribute, and structure violations
context: fork
allowed-tools:
  - Read
  - Grep
  - Glob
---

# Skill: audit-tests

Scan every test file in `tests/UnitTests/` and `tests/IntegrationTests/` and report convention violations. Do not modify any file.

## Rules to enforce

### Naming convention
Every test method must follow `{MethodName}_When{Condition}_Should{ExpectedResult}`.
Flag any method that:
- Is missing the `_When` segment
- Is missing the `_Should` segment
- Uses a different separator style (e.g. camelCase, no underscores)

### Attribute rules

| Scenario | Required attribute | Violation if using |
|----------|-------------------|-------------------|
| Domain entity — single scenario | `[Fact]` | `[Theory]` without `[InlineData]` |
| Domain entity — data-driven | `[Theory]` + `[InlineData]` | `[Fact]` when multiple data cases exist |
| Service/controller test (has `Mock<T>` param) | `[Theory, AutoMockData]` | bare `[Fact]` or `[Theory]` without `AutoMockData` |
| Integration test | `[Fact]` | `[Theory, AutoMockData]` |

### Structure rules (unit tests — services & controllers)
- Must have `// Arrange`, `// Act`, `// Assert` comments
- Mocks injected as parameters, not constructor fields (`private readonly Mock<T> _mock`)
- Exception assertions use `.Invoking(...).Should().ThrowAsync<T>()`, not `try/catch`

### Structure rules (integration tests)
- Test class must implement `IAsyncLifetime`
- Must have `DisposeAsync` that wipes the relevant DbSet
- Must NOT have constructor-injected `Mock<T>` parameters

### Namespace rules
- Service/controller unit tests: `namespace UnitTests;`
- Domain entity unit tests: `namespace UnitTests.Domain.Entities;`
- Integration tests: `namespace IntegrationTests;`

### Scaffold debris
- Flag any file named `UnitTest1.cs` or `Class1.cs` — these are undeleted scaffold files

## Output format

Group findings by file:

```
=== audit-tests report ===

tests/UnitTests/PlayerServiceTests.cs
  FAIL  naming       GetAllPlayers_ReturnsMappedDtos — missing _When/_Should segments (line 15)
  FAIL  naming       GetPlayersByPosition_ReturnsEmpty_WhenNoPlayers — missing _Should segment (line 44)
  PASS  attributes
  FAIL  structure    GetAllPlayers_ReturnsMappedDtos — missing // Arrange / // Act / // Assert comments (line 15–28)

tests/UnitTests/GameWeekTests.cs
  PASS  naming
  PASS  attributes
  PASS  structure
  PASS  namespace

tests/UnitTests/UnitTest1.cs
  FAIL  scaffold     Undeleted scaffold file — should be removed

...

=== Summary ===
Files scanned: 10
Files fully compliant: 6
Files with violations: 4
Total violations: 7
```

Do not suggest rewrites or edits. Report only.
