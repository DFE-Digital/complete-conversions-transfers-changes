# Set the major version of dotnet
ARG DOTNET_VERSION=8.0
# Set the major version of nodejs
ARG NODEJS_VERSION_MAJOR=22

# Build assets
FROM "node:${NODEJS_VERSION_MAJOR}-bullseye-slim" AS assets
WORKDIR /app
COPY ./src/Frontend/Dfe.Complete/wwwroot /app
RUN npm install
RUN npm run build

# Build the app using the dotnet SDK
FROM "mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-azurelinux3.0" AS build
WORKDIR /build
COPY ./Dfe.Complete.sln /build
COPY ./Directory.Build.props /build
COPY ./src/ /build/src
COPY ./script/docker-entrypoint.sh /app/docker-entrypoint.sh

# Mount GitHub Token as a Docker secret so that NuGet Feed can be accessed
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github "https://nuget.pkg.github.com/DFE-Digital/index.json"

RUN ["dotnet", "restore", "Dfe.Complete.sln"]
WORKDIR /build/src/Frontend/Dfe.Complete/
RUN ["dotnet", "build", "--no-restore", "-c", "Release"]
RUN ["dotnet", "publish", "--no-build", "-o", "/app"]

# Build a runtime environment
FROM "mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-azurelinux3.0" AS base
WORKDIR /app
LABEL org.opencontainers.image.source="https://github.com/DFE-Digital/complete-api"

COPY --from=build /app /app
COPY --from=assets /app /app/wwwroot
RUN ["chmod", "+x", "./docker-entrypoint.sh"]

USER $APP_UID
