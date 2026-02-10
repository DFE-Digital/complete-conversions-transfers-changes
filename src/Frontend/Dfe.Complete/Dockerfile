# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0@sha256:93396843879b7dda0da6ed92407e5862279e34aab51296442781ff69c51a6c93 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0@sha256:fbc91d6a310530d42475f3264f2412e361cf73cbbf6b0aa05f22eca91423571c AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Frontend/Dfe.Complete/Dfe.Complete.csproj", "src/Frontend/Dfe.Complete/"]
COPY ["src/Core/Dfe.Complete.Application/Dfe.Complete.Application.csproj", "src/Core/Dfe.Complete.Application/"]
COPY ["src/Core/Dfe.Complete.Domain/Dfe.Complete.Domain.csproj", "src/Core/Dfe.Complete.Domain/"]
COPY ["src/Core/Dfe.Complete.Utils/Dfe.Complete.Utils.csproj", "src/Core/Dfe.Complete.Utils/"]
COPY ["src/Core/Dfe.Complete.Infrastructure/Dfe.Complete.Infrastructure.csproj", "src/Core/Dfe.Complete.Infrastructure/"]
COPY ["src/Frontend/Dfe.Complete.Logging/Dfe.Complete.Logging.csproj", "src/Frontend/Dfe.Complete.Logging/"]
COPY ["src/Frontend/Dfe.Complete.UserContext/Dfe.Complete.UserContext.csproj", "src/Frontend/Dfe.Complete.UserContext/"]
RUN dotnet restore "./src/Frontend/Dfe.Complete/Dfe.Complete.csproj"
COPY . .
WORKDIR "/src/src/Frontend/Dfe.Complete"
RUN dotnet build "./Dfe.Complete.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dfe.Complete.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dfe.Complete.dll"]