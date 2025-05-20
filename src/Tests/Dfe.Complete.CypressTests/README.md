## Cypress Testing

### Test Setup

#### Environment variables

The Cypress tests are designed to run against the front-end of the application. To set up the tests, you need to provide
a configuration file named `cypress.env.json` with the following information:

```json
{
  "url": "<enter frontend URL>",
  "username": "<enter the user you want to run the tests with>",
  "api": "<enter backend URL>",
  "authKey": "<enter key set for the CypressTestSecret>",
  "tenantId": "<enter Id from Id Provider for the tenant>",
  "clientId": "<enter Id from Id Provider for the client used for test app registration>",
  "clientSecret": "<enter a client secret Id Provider for the client used for test app registration>",
  "completeApiClientId": "<enter Id from Id Provider for the complete api app registration>"
}
```

While it is possible to pass these configurations through commands, it is easier to store them in the configuration
file.

#### Authentication

The authentication is invoked in every test using the `login()` command. This defaults to the cypress test user, but you
can override this be passing in the TestUser user.

```javascript
beforeEach(() => {
    cy.login();
    // OR
    cy.login(businessSupportUser);
});
```

Intercepts all browser requests and adds a special auth header using the `authKey`. Make sure you set the
`CypressTestSecret` in your app, and it matches the `authKey` in the `cypress.env.json` file.

#### Database

The following users will also need to be added for the tests to run correctly.
Users to add:

- Cypress test user (default)
- Regional delivery officer London
- Regional Casework Services
- Regional Delivery Officer Team Leader
- Regional Casework Services Team Leader
- Business Support
- Data Consumer
- Service Support

```sql
-- Cypress Test User
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.testuser@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 1
               , 'cypress'
               , 'testuser'
               , 'TEST-AD-ID'
               , 1
               , 0
               , null
               , 'london'
               , null
               , 0
               , 0
               , null)
    END
-- Regional Delivery Officer London
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-RDO' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.rdo-london@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 1
               , 'cypress'
               , 'rdo-london'
               , 'TEST-AD-ID-RDO'
               , 1
               , 0
               , null
               , 'london'
               , null
               , 0
               , 0
               , null)
    END
-- Regional Casework Services
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-RCS' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.regional-casework-services@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 1
               , 'cypress'
               , 'regional-casework-services'
               , 'TEST-AD-ID-RCS'
               , 1
               , 0
               , null
               , 'regional_casework_services'
               , null
               , 0
               , 0
               , null)
    END
-- Regional Delivery Officer Team Leader
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-RDO-TL' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.rdo-team-leader@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 1
               , 1
               , 'cypress'
               , 'rdo-team-leader'
               , 'TEST-AD-ID-RDO-TL'
               , 1
               , 0
               , null
               , 'london'
               , null
               , 0
               , 0
               , null)
    END
-- Regional Casework Services Team Leader
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-RCS-TL' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.rcs-team-leader@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 1
               , 0
               , 'cypress'
               , 'rcs-team-leader'
               , 'TEST-AD-ID-RCS-TL'
               , 0
               , 0
               , null
               , 'regional_casework_services'
               , null
               , 0
               , 0
               , null)
    END
-- Business Support
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-BS' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.business-support@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 0
               , 'cypress'
               , 'business-support'
               , 'TEST-AD-ID-BS'
               , 0
               , 0
               , null
               , 'business_support'
               , null
               , 0
               , 0
               , null)
    END
-- Data Consumers
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-DC' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.data-consumers@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 0
               , 'cypress'
               , 'data-consumers'
               , 'TEST-AD-ID-DC'
               , 0
               , 0
               , null
               , 'data_consumers'
               , null
               , 0
               , 0
               , null)
    END
-- Service Support
IF NOT EXISTS (SELECT *
               FROM [complete].[users]
               WHERE 'TEST-AD-ID-SS' = active_directory_user_id)
    BEGIN
        INSERT INTO [complete].[users]
        VALUES ( NEWID()
               , 'cypress.service-support@education.gov.uk'
               , GETDATE()
               , GETDATE()
               , 0
               , 0
               , 'cypress'
               , 'service-support'
               , 'TEST-AD-ID-SS'
               , 0
               , 1
               , null
               , 'service_support'
               , null
               , 1
               , 1
               , null)
    END
```

### Test Execution

If you have a `cypress.env.json` file, the `cy:open` and `cy:run` commands will automatically pick up the configuration.

Navigate to the `Dfe.Complete.CypressTests` directory:

```
cd Dfe.Complete.CypressTests/
```

To open the Cypress Test Runner, run the following command:

```
npm run cy:open
```

To execute the tests in headless mode, use the following command (the output will log to the console):

```
npm run cy:run
```

### Test linting

We have set up [eslint](https://eslint.org) and [prettier](https://prettier.io/) on the Cypress tests to encourage code
quality. This can be run by using the script `npm run lint`

- Prettier will format all code files
- Eslint checks will run

All the default rules have been setup

### Security testing with ZAP

The Cypress tests can also be run, proxied via [OWASP ZAP](https://zaproxy.org) for passive security scanning of the
application.

These can be run using the configured `docker-compose.yml`, which will spin up containers for the ZAP daemon and the
Cypress tests, including all networking required. You will need to update any config in the file before running

Create a `.env` file for docker, this file needs to include

- all of your required cypress configuration
- HTTP_PROXY e.g. http://zap:8080
- ZAP_API_KEY, can be any random guid

Example env:

```
URL=<Enter URL>
USERNAME=<Enter username>
API=<Enter API>
API_KEY=<Enter API key>
AUTH_KEY=<Enter auth key>
HTTP_PROXY=http://zap:8080
ZAP_API_KEY=<Enter random guid>

```

**Note**: You might have trouble running this locally because of docker thinking localhost is the container and not your
machine

To run docker compose use:

`docker-compose -f docker-compose.yml --exit-code-from cypress`

**Note**: `--exit-code-from cypress` tells the container to quit when cypress finishes

You can also exclude URLs from being intercepted by using the NO_PROXY setting

e.g. NO_PROXY=google.com,yahoo.co.uk

Alternatively, you can run the Cypress tests against an existing ZAP proxy by setting the environment configuration

```
HTTP_PROXY="<zap-daemon-url>"
NO_PROXY="<list-of-urls-to-ignore>"
```

and setting the runtime variables

`zapReport=true,zapApiKey=<zap-api-key>,zapUrl="<zap-daemon-url>"`

### Accessibility Testing

The `executeAccessibilityTests` command is implemented in Cypress and is used to perform accessibility tests on a web
application. It utilises the Axe accessibility testing library to check for accessibility issues based on the specified
criteria.

#### Usage

To use this command, simply call `executeAccessibilityTests()` in your Cypress test code. Here's an example:

```javascript
it("should perform accessibility tests", () => {
    // Perform actions and assertions on your web application
    // ...

    // Execute accessibility tests
    cy.executeAccessibilityTests();

    // Continue with other test logic
    // ...
});
```

#### Command Details

The `executeAccessibilityTests` command under "support/commands.ts"

This will run all accessibility rules provided by the framework

### Troubleshooting Cypress Binary Issues

If you are installing Cypress from behind a proxy you can often hit an issue where the binary is not able to
download.Download a version of the cypress binary and in the .npmrc file set the path to it below.

```
CYPRESS_INSTALL_BINARY=<Path to binary>
```
