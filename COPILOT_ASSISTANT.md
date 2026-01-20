# Copilot Assistant

This file was added by GitHub Copilot to the repository as requested.

Purpose:
- Marker file to indicate Copilot actions.
- Contains brief instructions for repository maintainers.

Instructions:
- Remove or edit this file as needed.
- Use Git to commit and push changes to your remote repository.

// CoPilot Assistant Instructions: Unit Test Generator
// Purpose: Guide GitHub CoPilot to create unit tests for .NET classes
// Packages included: xUnit, Moq, AutoFixture, FluentAssertions

/*
INSTRUCTIONS FOR CoPilot:

1. Always generate unit tests using xUnit framework.
2. Use Moq to mock dependencies.
3. Use AutoFixture with AutoMoq customization to auto-create test objects and inject mocks.
4. Use FluentAssertions for readable assertions.
5. Include examples of:
   - Normal behavior ("happy path") tests.
   - Parameterized tests using [Theory] and [InlineData].
   - Exception tests verifying correct exceptions are thrown.
6. Follow naming convention: {MethodName}_When{Condition}_Should{ExpectedResult}.
7. Freeze mocks for dependencies using _fixture.Freeze<Mock<DependencyType>>() to reuse across tests.
8. Arrange, Act, Assert pattern should always be clearly separated.
9. Provide default scaffolding for SUT (System Under Test) and interfaces, replaceable with actual code.
10. Include inline comments explaining the purpose of each step in the test.
11. Avoid writing actual implementation logic; focus on testing structure and mock setup.



