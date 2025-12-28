FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for Central Package Management.
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project file.
COPY ["code/WorldDomination.SimpleObservability.Dashboard/WorldDomination.SimpleObservability.Dashboard.csproj", "WorldDomination.SimpleObservability.Dashboard/"]

# Restore dependencies.
RUN dotnet restore "WorldDomination.SimpleObservability.Dashboard/WorldDomination.SimpleObservability.Dashboard.csproj"

# Copy everything else.
COPY code/WorldDomination.SimpleObservability.Dashboard/. WorldDomination.SimpleObservability.Dashboard/

# Build.
WORKDIR /src/WorldDomination.SimpleObservability.Dashboard
RUN dotnet build "WorldDomination.SimpleObservability.Dashboard.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "WorldDomination.SimpleObservability.Dashboard.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WorldDomination.SimpleObservability.Dashboard.dll"]
