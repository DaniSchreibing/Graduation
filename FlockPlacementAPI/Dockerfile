# ----------------------------------------
#   STAGE 1 — BUILD
# ----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution
COPY FlockPlacementAPI.slnx ./

# Copy project folders
COPY Application/ ./Application/
COPY Domain/ ./Domain/
COPY FlockPlacementAPI/ ./FlockPlacementAPI/
COPY Tests/ ./Tests/

# Restore
RUN dotnet restore "FlockPlacementAPI.slnx"

# Publish
RUN dotnet publish "FlockPlacementAPI/FlockPlacementAPI.csproj" -c Release -o /app/publish

# ----------------------------------------
#   STAGE 2 — RUNTIME
# ----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FlockPlacementAPI.dll"]