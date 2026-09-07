### 1. Plan Mode Default

- Enter plan mode for ANY not-trivial task (3+ steps or architectural decisions)
- Use plan mode for verification steps, not just building
- Write detailed specs upfront to reduce ambiguity

### 2. Self-Improvement Loop

- After ANY correction from the user: update `tasks/lessons.md` with the pattern
- Write rules for yourself that prevent the same mistake
- Ruthlessly iterate on these lessons until the mistake rate drops
- Review lessons at session start for a project

### 3. Verification Before Done

- Never mark a task complete without proving it works
- Diff behavior between main and your changes when relevant
- Ask yourself: "Would a staff engineer approve this?"
- Run tests, check logs, demonstrate correctness
- Stop every service/process you started for verification (`dotnet run`, `ng serve`, background API instances, etc.) as soon as you're done with it - a leftover instance locks build output (`bin`/`obj`) and blocks the next `dotnet build`. When backgrounding via `nohup`, kill the whole process chain (shell + child process), not just the top-level PID.

### 4. Demand Elegance (Balanced)

- For non-trivial changes: pause and ask "is there a more elegant way?"
- If a fix feels hacky: "Knowing everything I know now, implement the elegant solution"
- Skip this for simple, obvious fixes. Don't overengineer
- Challenge your own work before presenting it

### 5. Skills usage

- Use skills for any task that requires a capability
- Load with `.claude/setting.json`
- Invoke skills with natural language
- Each skill is one independent capability

## Core Principles

- **Simplicity First**: Make every change as simple as possible. Impact minimal code
- **No Laziness**: Find root causes. No temporary fixes. Senior developer standards

## Project General Instructions

- .NET 10, Csharp 14, AzureSQL, Aspnet Identity, Azure blob storage
- Prefer records for immutable DTOs.
- Keep controllers thin.
- Business logic belongs in services.
- Always use the latest versions of dependencies.
- Minimize the amount of code generated.

## Test
- Library: xUnit, Shoudly, Moq
- Project has unit tests (*.UnitTests) and integration tests (*.IntegrationTests).
- Always create test cases for the generated code both positive and negative.
- Use Shouldly for test assertions, not xUnit's Assert.
