# Online Theater Kata

A programming kata focused on refactoring from an anemic domain model to a rich domain model using Domain-Driven Design (DDD) tactical patterns.

## Overview

This kata simulates an online theater system where customers can purchase movies with different licensing models. The initial implementation follows an anemic domain model pattern, and the goal is to refactor it into a rich domain model by applying DDD principles.

## Domain

The system includes:
- **Customers**: Can purchase movies and be promoted to different status levels
- **Movies**: Available with different licensing models (Two Days, Life Long)
- **Purchased Movies**: Track customer movie purchases with expiration dates

## Key Entities

- `Customer`: Regular customers who can purchase movies
- `Movie`: Movies with various licensing models
- `PurchasedMovie`: Represents a customer's movie purchase

## Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (In-Memory for tests)
- xUnit for testing

## Getting Started

1. Clone the repository
2. Build the solution
3. Run the acceptance tests to understand the current behavior
4. Begin refactoring the anemic domain model into a rich domain model

## Testing

The kata includes comprehensive acceptance tests that validate the API behavior. Run tests using:

```bash
dotnet test
```

## Credits

Based on work by Vladimir Khorikov, adapted as a refactoring kata for learning DDD tactical patterns.

## Goal

Transform the anemic domain model into a rich domain model by:
- Moving business logic from services into entities
- Implementing proper encapsulation
- Applying DDD patterns (Value Objects, Entities, Aggregates)
- Ensuring domain invariants are maintained within the domain layer