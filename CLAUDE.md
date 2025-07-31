# CLAUDE.md - DAYA Cloud Framework V2

This is a .NET cloud framework project structured as a multi-layer architecture with Clean Architecture principles. 

## Project Structure

- **Src/**: Core framework libraries
  - `DAYA.Cloud.Framework.V2` - Main framework library
  - `DAYA.Cloud.Framework.V2.Authentication` - Authentication components
  - `DAYA.Cloud.Framework.V2.Cosmos.Migration` - Cosmos DB migration utilities
  - `DAYA.Cloud.Framework.V2.SymmetricEncryption` - Encryption utilities
  - `DAYA.ArchRules` - Architecture validation rules
  - `DAYA.Cloud.CodeGenerator` - Code generation tools

- **Example/**: Sample implementation showing framework usage
  - `Daya.Sample.API` - Web API layer
  - `Daya.Sample.Application` - Application layer with CQRS
  - `Daya.Sample.Domain` - Domain layer with entities and business rules
  - `Daya.Sample.Infrastructure` - Infrastructure layer
  - `Daya.Sample.IntegrationEvents` - Integration events
  - `Daya.Sample.IntegrationTests` - Integration tests
  - `Daya.Sample.TestHelpers` - Test utilities
  - `Daya.Sample.Migration.Data` - Data migration utilities

## Architecture Patterns

This framework follows:
- **Clean Architecture** with clear layer separation
- **CQRS** (Command Query Responsibility Segregation)
- **Domain-Driven Design** (DDD) with aggregates, entities, and value objects
- **Event-Driven Architecture** with domain events and integration events
- **Outbox Pattern** for reliable event publishing

## Key Technologies

- .NET Framework/Core
- Azure Cosmos DB
- Azure Service Bus
- Azure Search
- MediatR for CQRS
- FluentValidation

## Commands for Development

When working with this project:
- Use `dotnet build` to build the solution
- Use `dotnet test` to run tests
- Architecture rules are enforced via `DAYA.ArchRules` project

## Code Conventions

- Follow the established patterns in the Example project
- Use typed IDs for aggregate identifiers
- Implement proper validation using FluentValidation
- Use MediatR for command/query handling
- Follow the established naming conventions for commands, queries, and handlers
- Implement proper domain events for business logic
- Use the repository pattern for data access
- Follow the outbox pattern for publishing integration events

## Testing

- Use the TestHelpers project for test utilities
- Integration tests are in the IntegrationTests project
- Architecture rules are tested via ArchRuleTests project

## Development Notes

- This is an enterprise-grade cloud framework for Azure
- Security and performance are key considerations
- Follow the established architectural patterns and rules
- Use the code generator tools when available for consistency