## GitHub Copilot Instructions

---

## Coding

### General Coding Rules

* Open the exact file to be edited before generating code.
* Use inline suggestions (`Tab` / `Right Arrow`) for small changes.
* Use Copilot Chat for multi-file or complex implementations.
* Always reference target files by full path.
* Specify the project, target file, and method or interface to implement.
* Keep prompts small, focused, and limited to a single responsibility.
* Avoid generating unrelated changes outside the requested scope.

---

### Coding — DDD & CQRS (New Services & Controllers)

#### Architectural Principles

* Follow **Domain-Driven Design (DDD)** with clear separation between:

  * **Domain**: Entities, Value Objects, Aggregates, Domain Events
  * **Application**: CQRS (Commands, Queries, Handlers, DTOs, Interfaces)
  * **Infrastructure**: Persistence, external services, implementations
  * **API**: Controllers (𝐌𝐢𝐧𝐢𝐦𝐚𝐥 𝐀𝐏𝐈𝐬), request/response contracts
* Enforce dependency direction:

  ```
  API → Application → Domain
               ↑
        Infrastructure
  ```

---

#### Service Creation Rules

* Each business capability is implemented as a **use case**, not a generic service.
* Avoid “fat” services:

  * Business rules belong in the **Domain**
  * Orchestration belongs in **Handlers**
* Application services must be **stateless**.

---

#### CQRS Rules

* Use **Commands** for write operations.
* Use **Queries** for read operations.
* Never mix reads and writes in the same handler.

**Command Handlers**

* Modify domain state
* Persist changes
* Publish domain events when needed

**Query Handlers**

* Must be read-only
* Must not depend on domain entities when DTOs or projections exist

---

#### Commands (Write Path)

* Commands:

  * Represent **intent**
  * Are immutable
  * Contain only required data
* Handlers:

  * Validate command
  * Load aggregate from repository
  * Call aggregate methods
  * Persist changes
  * Return minimal result (Id, Status)

**Rules**

* No business logic in controllers
* No persistence logic in the domain
* No framework dependencies in the domain layer

---

#### Queries (Read Path)

* Queries return **DTOs**, never domain entities.
* Query handlers:

  * Access read models or projections
  * Never modify state
* Optimize queries for read performance.

---

#### Controller Rules

* Controllers must be **thin**:

  * Accept HTTP request
  * Map request → Command / Query
  * Send to mediator/dispatcher
  * Map result → HTTP response
* One endpoint = one command or one query.
* No business logic in controllers.

---

#### Naming Conventions

* Commands: `CreateXCommand`, `UpdateXCommand`, `DeleteXCommand`
* Command Handlers: `CreateXCommandHandler`
* Queries: `GetXByIdQuery`, `SearchXQuery`
* Controllers: `XController`

---

## Testing

### Mandatory Testing Rules

* Always generate **unit tests using xUnit**.
* Always use **[Theory]**, never **[Fact]**.
* Use **Moq** to mock dependencies.
* Use **AutoFixture** with **AutoMoqCustomization**.
* Use **FluentAssertions** for assertions.

---

### AutoMockDataAttribute

* Create or use `AutoMockDataAttribute` inheriting from `AutoDataAttribute`.
* Initialize shared customizations in the constructor:

  * `StringCustomization`
  * `DateTimeCustomization`
  * Other common primitives as needed
* Use this attribute for **all tests**.

---

### Mocking Rules

* Freeze shared mocks using:

  ```
  _fixture.Freeze<Mock<DependencyType>>()
  ```
* Reuse frozen mocks across the test.

---

### Test Structure Rules

* Follow naming convention:

  ```
  {MethodName}_When{Condition}_Should{ExpectedResult}
  ```
* Always use **Arrange / Act / Assert** with clear separation.
* Include inline comments explaining each step.
* Do **not** write implementation logic inside tests.
* Focus only on:

  * Test structure
  * Mock setup
  * Expectations

---

### Required Test Coverage Examples

Include examples of:

* Happy path tests
* Parameterized tests using `[Theory]` and `[InlineData]`
* Exception tests validating exception type and message

---

### Test Scaffolding

* Provide default scaffolding for:

  * System Under Test (SUT)
  * Interfaces and dependencies
* Scaffolding must be easily replaceable with real code.

---

### After Generating Tests

Run:

```
dotnet build
dotnet test
```

Fix failures immediately 



Angular application with the following requirements:


## General Rules
- Use **Standalone Components** by default (NO NgModules unless explicitly required).
- Prefer **Signals** over RxJS for local and UI state.
- Avoid unnecessary RxJS complexity.
- Assume **zoneless change detection** (no reliance on Zone.js).
- Optimize for **performance, readability, and simplicity**.

---

## Reactivity & State Management
- Use `signal()` for mutable state.
- Use `computed()` for derived state.
- Use `effect()` for side effects.
- Do NOT use `BehaviorSubject` or `Subject` for local component state.
- Use Observables ONLY for:
  - HTTP streams
  - WebSockets
  - External event streams

Example:
```ts
count = signal(0);
doubleCount = computed(() => this.count() * 2);



1. Use the latest stable Angular version
2. Use standalone components (no NgModules)
3. Use SCSS for styling
4. Enable strict mode
5. Use routing
6. Follow clean architecture and best practices
7. Use environment-based configuration
8. Prepare the project for enterprise-scale development

Project structure:
- Core folder for singleton services (auth, api, interceptors)
- Shared folder for reusable components, pipes, directives
- Feature-based structure under /features
- Use lazy loading for features
- Use Angular signals where appropriate
- Use HttpClient with interceptors
- Add example service, component, and route

Include:
- Example feature module (e.g. Dashboard)
- Example API service with mock data
- Example route guard
- Clean, well-commented code

Do not include unnecessary boilerplate.
Explain key decisions briefly.

