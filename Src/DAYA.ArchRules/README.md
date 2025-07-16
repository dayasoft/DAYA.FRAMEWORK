# DAYA ArchRules Framework

## Overview

The DAYA ArchRules Framework is a powerful architecture validation tool designed to enforce consistent architectural patterns and best practices in .NET applications. It helps ensure that your code adheres to clean architecture principles by validating architectural rules during build or test processes.

## Purpose

The primary purpose of this framework is to:

1. **Enforce Architectural Boundaries**: Ensure that different layers (Domain, Application, Infrastructure, API) respect their dependencies.
2. **Maintain Coding Standards**: Enforce consistent naming conventions and implementation patterns.
3. **Prevent Architecture Erosion**: Catch architectural violations early in the development cycle.
4. **Automate Architecture Validation**: Integrate validation into CI/CD pipelines as part of the build process.

## Key Components

### RuleEngine

The core component that loads and executes all architecture rules. It takes assemblies from different layers as input:

- Domain Assembly
- Application Assembly
- Infrastructure Assembly
- API Assembly
- Integration Events Assembly

### ArchRule

The base class for all architecture rules. Each rule implements the `Check()` method that validates a specific architectural constraint.

### Rule Categories

The framework includes several categories of rules:

1. **Layer Rules**: Enforce proper dependencies between architectural layers
   - Domain layer should not depend on Infrastructure or Application layers
   - Application layer should not depend on Infrastructure layer

2. **Naming Rules**: Enforce consistent naming conventions
   - Interfaces should be prefixed with 'I'
   - Async methods should end with 'Async'
   - Repository implementations should have 'Repository' suffix

3. **Integration Event Rules**: Ensure proper integration event implementation
   - Events should be immutable
   - Events should have appropriate postfixes
   - Properties should be primitives and public

4. **General Rules**: Enforce broader coding standards
   - Async methods should not return void
   - Proper namespace usage

## How to Use

### 1. Setup in Test Project

```csharp
public void Architecture_Rules_Test()
{
    var domainAssembly = Assembly.Load("YourProject.Domain");
    var applicationAssembly = Assembly.Load("YourProject.Application");
    var infrastructureAssembly = Assembly.Load("YourProject.Infrastructure");
    var apiAssembly = Assembly.Load("YourProject.API");
    var integrationEventsAssembly = Assembly.Load("YourProject.IntegrationEvents");

    var engine = new RuleEngine(
        domainAssembly, 
        applicationAssembly, 
        infrastructureAssembly,
        apiAssembly, 
        integrationEventsAssembly);

    engine.Check(); // Will throw an exception if rules are violated
}
```

### 2. Configure Rule Exclusions

For specific cases where rule exceptions are needed:

```csharp
// Before running the rule engine
RuleEngine.IgnoredRules.Add(("SomeClass", "SomeRuleType"));
```

## Extending the Framework

To create custom architecture rules:

1. Create a new class inheriting from `ArchRule`
2. Implement the `Check()` method
3. Throw an `ArchitectureException` when the rule is violated

Example:
```csharp
internal class MyCustomRule : ArchRule
{
    internal override string Message => "Custom rule violation message";

    internal override void Check()
    {
        var result = Types.InAssembly(Data.DomainAssembly)
            .That()
            .HaveNameStartingWith("MyPrefix")
            .Should()
            .ResideInNamespace("MyNamespace")
            .GetResult();
            
        if (!result.IsSuccessful)
        {
            throw new ArchitectureException(Message);
        }
    }
}
```

## Benefits

- **Maintainability**: Ensures codebase remains clean and follows architectural patterns
- **Onboarding**: Helps new developers understand and adhere to architectural standards
- **Quality Assurance**: Catches architectural violations early in development
- **Scalability**: Supports growing codebases by maintaining architectural integrity

## Example Implementation

See the sample project in the Example folder for a complete implementation example.
