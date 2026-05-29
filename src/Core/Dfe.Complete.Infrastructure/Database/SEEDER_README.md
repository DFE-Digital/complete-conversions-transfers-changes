# Quickstart

- **Set your connection string in user secrets:**
    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
    ```

- **Apply migrations:**
    ```bash
    dotnet ef database update -p src/Core/Dfe.Complete.Infrastructure -s src/Api/Dfe.Complete.Api
    ```

- **Run the seeder:**
    ```bash
    dotnet run --project src/Api/Dfe.Complete.Api -- seed-db
    ```
    - To reset all seed data before seeding:
        ```bash
        dotnet run --project src/Api/Dfe.Complete.Api -- seed-db --force
        ```

- **Local developer users:**  
    To add your own test users, set the following secret:
    ```bash
    dotnet user-secrets set "LocalSeed:UserEmails" "alice.smith@education.gov.uk;bob.jones@education.gov.uk"
    ```
# Database Seeder Usage Guide

## Overview

The DatabaseSeeder provides a comprehensive solution for initializing the Dfe.Complete database with all necessary data for development environments only. It is:
- **CLI-only** (run via the API project)
- **Idempotent** (safe to run multiple times)
- **Development/local DB only** (will not run on production or remote DBs)
- **Supports local developer user seeding via user secrets**

**Important:** To run seeding commands, use the API project (`src/Api/Dfe.Complete.Api`) which references this Infrastructure project.

## What Gets Seeded

- DAO revocation reasons (6 standard reasons)
- Significant date history reasons (7 standard reasons)
- Local authorities (18 major UK local authorities)
- Default system users (4 regional delivery officers)
- Local developer users (from user secrets, see below)
- Basic project groups for development

## How to Run the Seeder

### Command Line Usage

Run from the API project root (not Infrastructure project):

```bash
cd src/Api/Dfe.Complete.Api

dotnet run -- seed-db

dotnet run -- seed-db --help
```

Or from the solution root:

```bash
dotnet run --project src/Api/Dfe.Complete.Api -- seed-db
```

### Integration in Program.cs

Add to your **API project's Program.cs** (src/Api/Dfe.Complete.Api/Program.cs):

```csharp
// Add to service registration
builder.Services.AddDatabaseSeeder();

var app = builder.Build();

// Handle command line seeding arguments
if (args.Length > 0 && args[0] == "seed-db")
{
    var exitCode = await DatabaseSeederCli.ExecuteAsync(args.Skip(1).ToArray(), app.Services);
    Environment.Exit(exitCode);
}

await app.RunAsync();
```

## Local Developer User Seeding

You can seed custom local developer users using user secrets. This is only allowed against a local database (connection string must contain `localhost`, `127.0.0.1`, or `localdb`).

**Set your local developer emails (semicolon-separated):**

```bash
dotnet user-secrets set "LocalSeed:UserEmails" "alice.smith@education.gov.uk;bob.jones@education.gov.uk"
```

- Only emails in the format `firstname.lastname@education.gov.uk` are accepted.
- First and last names are derived from the email.
- Duplicates and invalid formats are ignored.
- **Never commit real user emails to source control.**

When you run the seeder, these users will be added if they do not already exist.

## Safety Features

- **Development only:** Only runs in Development environment
- **Idempotent:** Can be run multiple times safely
- **Comprehensive logging:** Tracks all seeding operations
- **Local DB guard:** Local developer user seeding only runs on local DBs

## Notes

- External dependencies (GIAS establishments, trusts by UKPRN/URN) are not included per requirements
- Local authorities use a representative sample of major UK authorities
- User emails use placeholder domains (@education.gov.uk)
- All data uses proper domain value objects and follows existing patterns
- All-or-nothing approach: no separate sample data option