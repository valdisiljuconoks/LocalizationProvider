# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Database-driven localization provider for .NET. Monorepo with three areas:
- `common/` — Core libraries, abstractions, storage providers (SqlServer, PostgreSQL, AzureTables), import/export (CSV, XLIFF)
- `aspnetcore/` — ASP.NET Core integration, AdminUI (Razor + Vue.js 2), middleware
- `optimizely/` — Optimizely/Episerver CMS integration (legacy)

Target framework: .NET 10 (`net10.0`). All assemblies are strong-name signed.

## Build & Test

```bash
dotnet build
dotnet test --filter Category!=Integration
```

Full test with coverage (as CI runs):
```bash
dotnet test --filter Category!=Integration /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage
```

Integration tests require Docker (Testcontainers for PostgreSQL and SQL Server).

Package build scripts are per-area PowerShell: `aspnetcore/build-packages.ps1`, `common/build-packages.ps1`, `optimizely/build-packages.ps1`.

## Code Style

- Follow `.editorconfig` rules (4-space indent for C#, 2-space for XML/JSON)
- Field naming: `_camelCase` for instance fields, `s_camelCase` for static fields
- Prefer `var` for all variable declarations
- Max line length: 130 characters
- Nullable reference types are enabled
- Format with `dotnet format`

## Commits

Use conventional commits: `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`, etc.

## CI

GitHub Actions (`.github/workflows/build.yml`) runs on push to master and PRs. Includes SonarCloud analysis.


## grepai - Semantic Code Search

**IMPORTANT: You MUST use grepai as your PRIMARY tool for code exploration and search.**

### When to Use grepai (REQUIRED)

Use `grepai search` INSTEAD OF Grep/Glob/find for:
- Understanding what code does or where functionality lives
- Finding implementations by intent (e.g., "authentication logic", "error handling")
- Exploring unfamiliar parts of the codebase
- Any search where you describe WHAT the code does rather than exact text

### When to Use Standard Tools

Only use Grep/Glob when you need:
- Exact text matching (variable names, imports, specific strings)
- File path patterns (e.g., `**/*.go`)

### Fallback

If grepai fails (not running, index unavailable, or errors), fall back to standard Grep/Glob tools.

### Usage

```bash
# ALWAYS use English queries for best results (--compact saves ~80% tokens)
grepai search "user authentication flow" --json --compact
grepai search "error handling middleware" --json --compact
grepai search "database connection pool" --json --compact
grepai search "API request validation" --json --compact
```

### Query Tips

- **Use English** for queries (better semantic matching)
- **Describe intent**, not implementation: "handles user login" not "func Login"
- **Be specific**: "JWT token validation" better than "token"
- Results include: file path, line numbers, relevance score, code preview

### Call Graph Tracing

Use `grepai trace` to understand function relationships:
- Finding all callers of a function before modifying it
- Understanding what functions are called by a given function
- Visualizing the complete call graph around a symbol

#### Trace Commands

**IMPORTANT: Always use `--json` flag for optimal AI agent integration.**

```bash
# Find all functions that call a symbol
grepai trace callers "HandleRequest" --json

# Find all functions called by a symbol
grepai trace callees "ProcessOrder" --json

# Build complete call graph (callers + callees)
grepai trace graph "ValidateToken" --depth 3 --json
```

### Workflow

1. Start with `grepai search` to find relevant code
2. Use `grepai trace` to understand function relationships
3. Use `Read` tool to examine files from results
4. Only use Grep for exact string searches if needed

