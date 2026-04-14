# Database Seeder Usage Guide

## Overview

The DatabaseSeeder provides a comprehensive solution for initializing the Dfe.Complete database with all necessary data for development environments only.

**Important**: This is a class library. To run seeding commands, use the API project (`src/Api/Dfe.Complete.Api`) which references this Infrastructure project.

## Components

### 1. DatabaseSeeder.cs
Core seeding logic that handles all data needed for development:
- DAO revocation reasons
- Significant date history reasons  
- Local authorities (basic set)
- Default system users
- Project data

### 2. DatabaseSeederExtensions.cs
Extension methods for easy integration with ASP.NET Core applications:
- `SeedDatabaseAsync()` - Seeds database from IHost
- `AddDatabaseSeeder()` - Registers seeder services
- `SeedDatabaseAsync(IServiceProvider)` - Seeds using service provider

### 3. DatabaseSeederCli.cs
Command-line interface for development seeding operations. Called from the API project via Program.cs.

## Integration Examples

### In Program.cs (Development Auto-Seeding)

```csharp
// Add to service registration
builder.Services.AddDatabaseSeeder();

// Add after app.Build() but before app.Run()
var app = builder.Build();

// Handle command line seeding arguments
if (args.Length > 0 && args[0] == "seed-db")
{
    try
    {
        var exitCode = await DatabaseSeederCli.ExecuteAsync(args.Skip(1).ToArray(), app.Services);
        Environment.Exit(exitCode);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Command line seeding failed");
        Environment.Exit(1);
    }
}

// Auto-seed in development only  
if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync(force: false);
}

await app.RunAsync();
```

### Command Line Usage

**Run from the API project root (not Infrastructure project):**

```bash
# Navigate to API project directory
cd src/Api/Dfe.Complete.Api

# Basic seeding (development only)
dotnet run -- seed-db

# Force re-seed (development only)
dotnet run -- seed-db --force

# Show help
dotnet run -- seed-db --help
```

**Or from solution root:**

```bash
# Basic seeding
dotnet run --project src/Api/Dfe.Complete.Api -- seed-db

# Force re-seed  
dotnet run --project src/Api/Dfe.Complete.Api -- seed-db --force
```

### Programmatic Usage

```csharp
// Direct usage in a service or controller (development only)
public async Task SeedDatabase()
{
    await _serviceProvider.SeedDatabaseAsync(force: false);
}

// Or use the extension method approach
public async Task SeedDatabaseViaExtension(WebApplication app, string[] args)
{
    await app.ConfigureDatabaseSeedingAsync(args);
}
```

## Safety Features

- **Development only**: Only runs in Development environment
- **Idempotent**: Can be run multiple times safely
- **Comprehensive logging**: Tracks all seeding operations
- **Force protection**: Prevents accidental data loss unless explicitly requested
## Data Seeded

### All Development Data
- DAO revocation reasons (6 standard reasons)
- Significant date history reasons (7 standard reasons)  
- Local authorities (18 major UK local authorities)
- Default system users (4 regional delivery officers)
- Basic project groups for development

## Notes

- **Development only**: Seeding is completely disabled for non-development environments
- External dependencies (GIAS establishments, trusts by UKPRN/URN) are not included per requirements
- Local authorities use a representative sample of major UK authorities
- User emails use placeholder domains (@education.gov.uk)
- All data uses proper domain value objects and follows existing patterns
- All-or-nothing approach: no separate sample data option

## Quick Integration

Add to your **API project's Program.cs** (src/Api/Dfe.Complete.Api/Program.cs):

```csharp
using Dfe.Complete.Api.Extensions; // Add this using statement

// ... existing imports and builder configuration ...

// Add to service registration (with other builder.Services calls)
builder.Services.AddDatabaseSeeding();

var app = builder.Build();

// ... existing middleware configuration ...

// Add before app.RunAsync() - handles both CLI and auto-seeding
await app.ConfigureDatabaseSeedingAsync(args);

await app.RunAsync();
```

The extension method automatically handles:
- Command line seeding arguments (`seed-db`, `--force`, `--help`)
- Auto-seeding on startup (development environment only)
- Proper error handling and logging

## Command Line Usage
```bash
# From solution root - Basic development seeding
dotnet run --project src/Api/Dfe.Complete.Api -- seed-db

# From solution root - Force re-seed (development only) 
dotnet run --project src/Api/Dfe.Complete.Api -- seed-db --force
```