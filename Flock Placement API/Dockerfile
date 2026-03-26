# -------------------------------------------------------
#    STAGE 1: Build
# -------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY "Flock Placement API/Flock Placement API.sln" ./
COPY "Flock Placement API/Application/" "Application/" ./
COPY "Flock Placement API/Domain/" "Domain/" ./
COPY "Flock Placement API/Flock Placement API/" "FlockPlacementAPI/" ./
COPY "Flock Placement API/Tests/" "Tests/" ./

RUN dotnet restore "Flock Placement API.sln"

# Build and publish
RUN dotnet publish "Flock Placement API.sln" -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Flock Placement API.dll"]