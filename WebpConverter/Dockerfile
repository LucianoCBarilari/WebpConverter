# Final runtime image — Alpine for minimal size
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
# Required for .NET on Alpine — disables full ICU globalization (not needed for image processing)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
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