FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for Central Package Management.
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files.
COPY ["code/WorldDomination.SimpleObservability.SampleServiceWebApi/WorldDomination.SimpleObservability.SampleServiceWebApi.csproj", "WorldDomination.SimpleObservability.SampleServiceWebApi/"]

# Restore dependencies.
RUN dotnet restore "WorldDomination.SimpleObservability.SampleServiceWebApi/WorldDomination.SimpleObservability.SampleServiceWebApi.csproj"

# Copy everything else.
COPY code/WorldDomination.SimpleObservability.SampleServiceWebApi/. WorldDomination.SimpleObservability.SampleServiceWebApi/

# Build.
WORKDIR /src/WorldDomination.SimpleObservability.SampleServiceWebApi
RUN dotnet build "WorldDomination.SimpleObservability.SampleServiceWebApi.csproj" -c Release --no-restore -o /app/build 

FROM build AS publish
RUN dotnet publish "WorldDomination.SimpleObservability.SampleServiceWebApi.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WorldDomination.SimpleObservability.SampleServiceWebApi.dll"]
