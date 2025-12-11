FROM mcr.microsoft.com/dotnet/runtime:8.0@sha256:40e24d28590d355e21a9923a9304a38da1a255d32edf7a5b3d54ec43b3e20637 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:a87db299d259cec53210df406cd5e51f900a0c6d938fe56d5f392629c0505d75 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in correct order (dependencies first)
COPY ["src/Api/Dfe.Complete.Api.Client/Dfe.Complete.Api.Client.csproj", "src/Dfe.Complete.Api.Client/"]
COPY ["src/Core/Dfe.Complete.Application/Dfe.Complete.Application.csproj", "src/Dfe.Complete.Application/"]
COPY ["src/Core/Dfe.Complete.Domain/Dfe.Complete.Domain.csproj", "src/Dfe.Complete.Domain/"]
COPY ["src/Core/Dfe.Complete.Utils/Dfe.Complete.Utils.csproj", "src/Dfe.Complete.Utils/"]
COPY ["src/Core/Dfe.Complete.Infrastructure/Dfe.Complete.Infrastructure.csproj", "Dfe.Complete.Infrastructure/"]
RUN dotnet restore "Dfe.Complete.Infrastructure/Dfe.Complete.Infrastructure.csproj" -p:TargetFramework=net8.0
COPY . .
WORKDIR "/src/Dfe.Complete.Infrastructure"

## Run migrations
RUN dotnet tool install --version 8.0.21 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet-ef database update --connection "$CONNECTION_STRING"
