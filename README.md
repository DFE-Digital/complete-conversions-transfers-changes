# complete-conversions-transfers-changes

Complete application and API (for external services) to help the process of schools converting to academies, transferring between academy trusts or changing their academy status.

## Setup

### Pre requisites

NodeJS and NPM installed
It is recommended you request access to the complete development database by making a service request. You can then use that to connect your local build to a db without installing one locally.

### Frontend setup

- In the DfE.Complete project navigate to the directory wwwroot
- Run the commands `npm i` then `npm run build`
- Setup db either by running the local migrations or connecting to the dev db
  - To use a Local db simply install MSSQl (You can use a Docker container or local Db, recommened way is MSSQL express)
  - Navigate to the Dfe.Complete.Infrastructure and run `dotnet ef database update`
- Populate the User secrets file. You will need to use the correct connection string, either your local db or db enviroment of your choice

### API setup

- Populate the User secrets file

## Database Migrations

### Creating a Migration

When you make changes to the domain models in `Dfe.Complete.Domain.Entities`, you need to create a new migration to update the database schema.

Run the following command from the root of the repository:

```powershell
dotnet ef migrations add <MigrationName> -p src/Core/Dfe.Complete.Infrastructure -s src/Api/Dfe.Complete.Api
```

- `-p`: Specifies the project where the DbContext and migrations are located.
- `-s`: Specifies the startup project (the API project is recommended as it has the necessary design-time tools).

### Applying Migrations

To apply migrations to your local database:

```powershell
# From the root of the repository
dotnet ef database update -p src/Core/Dfe.Complete.Infrastructure -s src/Api/Dfe.Complete.Api

# OR navigate to the Infrastructure project
cd src/Core/Dfe.Complete.Infrastructure
dotnet ef database update
```

### Design-Time Factory

A `GenericDbContextFactory` (and `CompleteContextFactory`) exists in `Dfe.Complete.Infrastructure` to facilitate migrations at design-time. It defaults to the `Development` environment and attempts to load the connection string from `src/Api/Dfe.Complete.Api/appsettings.Development.json`.

---

## Docker compose for local development

Using docker compose for local development can be [found here](docs/docker-compose.md)

### Authorisation

#### Frontend  
List of all of our policies can be [found here](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Core/Dfe.Complete.Domain/Constants/UserPolicyConstants.cs). If there's not an auth constant that matches your requirements, you can add it here.

Policies themselves are defined in one of two places:
- for simple policies, pop them in [appsettings.json](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Frontend/Dfe.Complete/appsettings.json#L63-L65) - the two available operators are 
  - `And` - every role must be present on the user
  - `Or` - at least one of the roles must be present on the user

Currently the policies are used in two different ways:
- directly on a [model](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Frontend/Dfe.Complete/Pages/Projects/Team/YourTeamProjectsModel.cs#L9) - the permissions are inherited so now that everything extending `YourTeamProjectsModel` will also have `CanViewTeamProjects` auth policy check
- using the new `asp-policy` [tag helper](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Frontend/Dfe.Complete/Pages/Shared/_Header.cshtml#L32C43-L32C52). This will hide any html element that isn't authorised

Alternatively in the pages you can just use the authorizationService directly.

#### API
Basically the same setup but simpler.

End points authorised [like so](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Frontend/Dfe.Complete/Pages/Shared/_Header.cshtml#L32C43-L32C52).

Testing
If you add a protected route, please add the `"Route"` and `"ExpectedSecurity"` to the objects in [ExpectedSecurityConfig.json](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Tests/Dfe.Complete.Tests/SecurityTests/ExpectedSecurityConfig.json) - the rest will handle itself.

If you add a new custom policy, please update the inline data in [the tests](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/src/Tests/Dfe.Complete.Tests/SecurityTests/CustomPoliciesTest.cs).
