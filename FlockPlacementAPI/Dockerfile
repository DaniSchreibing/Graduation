# -------------------------------------------------------
#    STAGE 1: Build
# -------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY "FlockPlacementAPI/FlockPlacementAPI.slnx" ./
COPY "FlockPlacementAPI/Application/" "Application/" ./
COPY "FlockPlacementAPI/Domain/" "Domain/" ./
COPY "FlockPlacementAPI/FlockPlacementAPI/" "FlockPlacementAPI/" ./
COPY "FlockPlacementAPI/Tests/" "Tests/" ./

RUN dotnet restore "FlockPlacementAPI.slnx"

# Build and publish
RUN dotnet publish "FlockPlacementAPI.slnx" -c Release -o /app/publish

# -------------------------------------------------------
#    STAGE 2: Runtime Image
# -------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose your API port (adjust if needed)
EXPOSE 8080

# Start API
ENTRYPOINT ["dotnet", "FlockPlacementAPI.dll"]