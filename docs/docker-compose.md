# Docker-compose

- [Docker compose](#docker-compose)
  - [Startup up the development stack](#startup-the-development-stack)
    - [Why?](#why)
    - [How?](#how)
  - [Startup the application](#startup-the-application)
      - [Known Issues](#known-issues)

## Startup the development stack

### Why?

For quickly brining up any dependencies the project has such as an SQL database and redis (if applicable)

### How?

Ensure you are in the root directory of the project and issue the command:

```bash
docker-compose -f Stack/docker-compose.yml up -d --build
```

This will 
```bash
- This will bring up the sql sever with a username and password set.
- Create an empty database.
- Apply migrations.
```

## Startup the application

When opening the solution in Visual Studio ensure the configuration for docker-compose is selected

![docker compose selected](docker-compose-config.png)

This will automatically warmup a container for the application.

You can now debug and run the application using docker compose