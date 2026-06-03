---
name: verify
description: Build the solution and run unit tests to verify changes are correct. Use after implementing features or fixes.
---

Run the following commands in sequence to verify the codebase compiles and tests pass:

1. **Build**: `dotnet build`
2. **Test**: `dotnet test --filter Category!=Integration`

Report results clearly. If there are failures, analyze them and suggest fixes.

If the user asks for a full verification including integration tests, run:
```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage
```
Note: Integration tests require Docker to be running.
