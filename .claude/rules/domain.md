---
description: DDD domain entity conventions for the Domain layer
globs: ["src/Domain/**/*.cs"]
alwaysApply: false
---

# Domain Layer Rules

The Domain layer is the core of the system. It must remain pure C# with **zero** framework or infrastructure dependencies.

## Entity Structure

- All entities inherit from `Entity` (from `Domain.Common`) which provides the `Id` (Guid) property
- Properties must have **private setters** — state changes only through domain methods
- Always include a `private EntityName() { }` parameterless constructor for EF Core
- Business rules live here, not in services or controllers

```csharp
// Correct pattern
public class Player : Entity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    private Player() { } // EF Core

    public Player(string name, decimal price)
    {
        if (price <= 0) throw new ArgumentException("Price must be greater than zero", nameof(price));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Price = price;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentException("Price must be greater than zero", nameof(newPrice));
        Price = newPrice;
    }
}
```

## Validation Rules

- Validate in the **constructor** and in **every mutating method** — never trust callers
- Throw `ArgumentException` for invalid inputs, `ArgumentNullException` for null arguments
- Throw `InvalidOperationException` for invalid state transitions (e.g. completing an inactive game week)
- Exception messages must describe the rule violation clearly (they are asserted in tests)

## Forbidden in Domain

- No `using Microsoft.*` or `using EntityFrameworkCore.*`
- No async methods — domain logic is synchronous
- No DTOs — domain entities never reference `Application.DTOs`
- No static factory methods unless the construction logic is genuinely complex

## Scoring Formula (Player)

```
Points += (goalsScored × 5) + (assists × 3) + (cleanSheets × 4)
```

Stats are **accumulative** — `UpdateStats` adds to existing totals, it does not replace them.

## GameWeek State Machine

```
Upcoming → Active (via Activate()) → Completed (via Complete())
```

- `Activate()` on a Completed game week throws `InvalidOperationException`
- `Complete()` on a non-Active game week throws `InvalidOperationException`
- Calling `Activate()` on an already-Active game week is idempotent (no-op)
