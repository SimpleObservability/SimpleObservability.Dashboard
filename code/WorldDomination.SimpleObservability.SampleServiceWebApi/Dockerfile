FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for Central Package Management.
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files.
COPY ["code/Core/WorldDomination.SimpleObservability/WorldDomination.SimpleObservability.csproj", "Core/WorldDomination.SimpleObservability/"]
COPY ["code/WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi/WorldDomination.SimpleObservability.SampleServiceWebApi.csproj", "WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi/"]

# Restore dependencies.
RUN dotnet restore "WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi/WorldDomination.SimpleObservability.SampleServiceWebApi.csproj"

# Copy everything else.
COPY code/Core/WorldDomination.SimpleObservability/. Core/WorldDomination.SimpleObservability/
COPY code/WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi/. WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi/

# Build.
WORKDIR "/src/WebApi/WorldDomination.SimpleObservability.SampleServiceWebApi"
RUN dotnet build "WorldDomination.SimpleObservability.SampleServiceWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorldDomination.SimpleObservability.SampleServiceWebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WorldDomination.SimpleObservability.SampleServiceWebApi.dll"]
