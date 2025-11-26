FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for Central Package Management.
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project file.
COPY ["code/WorldDomination.SimpleObservability.Dashboard/WorldDomination.SimpleObservability.Dashboard.csproj", "code/WorldDomination.SimpleObservability.Dashboard/"]

# Restore dependencies.
RUN dotnet restore "code/WorldDomination.SimpleObservability.Dashboard/WorldDomination.SimpleObservability.Dashboard.csproj"

# Copy everything else.
COPY code/WorldDomination.SimpleObservability.Dashboard/. code/WorldDomination.SimpleObservability.Dashboard/

# Build.
WORKDIR "/src/code/WorldDomination.SimpleObservability.Dashboard"
RUN dotnet build "WorldDomination.SimpleObservability.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorldDomination.SimpleObservability.Dashboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WorldDomination.SimpleObservability.Dashboard.dll"]
