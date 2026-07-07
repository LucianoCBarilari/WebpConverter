# Final runtime image — Azure Linux Distroless (Microsoft hardened, minimal CVEs)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-azurelinux3.0-distroless AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebpConverter/WebpConverter.csproj", "WebpConverter/"]
RUN dotnet restore "./WebpConverter/WebpConverter.csproj"
COPY . .
WORKDIR "/src/WebpConverter"
RUN dotnet build "./WebpConverter.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebpConverter.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
USER $APP_UID
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebpConverter.dll"]