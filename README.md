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
