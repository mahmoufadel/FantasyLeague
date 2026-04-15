---
description: Audit a domain entity file for DDD compliance and report violations
context: fork
allowed-tools:
  - Read
  - Grep
  - Glob
---

# Skill: review-entity

Review the domain entity at `$ARGUMENTS` (a path like `src/Domain/Entities/Player.cs`) for compliance with this project's DDD rules. If no path is given, scan all files under `src/Domain/Entities/`.

## What to check

For each entity file, verify every rule below and report PASS or FAIL with line numbers.

### 1. Inherits from `Entity`
- The class declaration must include `: Entity`

### 2. Private setters on all properties
- Every `public` property must use `{ get; private set; }` — not `{ get; set; }`

### 3. EF Core private constructor
- Must contain a `private {ClassName}() { }` parameterless constructor

### 4. No framework imports
- No `using Microsoft.*` or `using EntityFrameworkCore.*`

### 5. Validation in constructor
- Every non-nullable, non-default parameter must be validated (null check or range check) before assignment

### 6. Validation in mutating methods
- Every `public void` or `public` mutating method must validate its parameters before modifying state

### 7. No async methods
- Domain entities must not have `async` methods

### 8. No DTO references
- No `using Application.DTOs` or any reference to DTO types

### 9. Exception types
- `ArgumentNullException` for null arguments
- `ArgumentException` for invalid values
- `InvalidOperationException` for invalid state transitions

## Output format

Print a report in this exact format:

```
=== review-entity: src/Domain/Entities/Player.cs ===

PASS  [1] Inherits from Entity
PASS  [2] Private setters
FAIL  [3] EF Core private constructor — no private parameterless constructor found
PASS  [4] No framework imports
PASS  [5] Constructor validation
PASS  [6] Mutating method validation
PASS  [7] No async methods
PASS  [8] No DTO references
PASS  [9] Correct exception types

Summary: 8 passed, 1 failed

Violations:
  [3] Line 8: missing private {ClassName}() { } constructor
```

If everything passes:
```
Summary: 9 passed, 0 failed — entity is DDD-compliant ✓
```

Do not suggest fixes or rewrite code. Report only.
