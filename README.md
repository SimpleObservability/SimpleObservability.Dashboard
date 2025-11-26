<h1 align="center">Simple: Observability Dashboard</h1>

<div align="center">
  <i>A lightweight, Docker-ready dashboard for monitoring microservices health across multiple environments.</i>
</div>

<p align="center">

# Summary

Observe your services easily with this Website (via a docker image) and simple customization of your own services which you wish to observe.
Update your "health" endpoints to return a specific schema which can then showcase the health AND state of the various services in your organisation.

How is this different to all the many other 'health' dashboards like NewRelic, DataDog, Splunk, App Insights, etc?  
This exposes your own service meta-data in an easy to read dashboard - not just if it's 'healthy' or 'unresponsive'.

<img width="1496" height="722" alt="image" src="https://github.com/user-attachments/assets/5ab46b14-e8c2-4aee-8dea-0ef3d4160d65" />

</p>


## Quick Start

### Using Docker (Recommended)

```bash
# Pull the image
docker pull purekrome/simple-observability:latest

# Run with volume-mounted configuration
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/dashboard-config.json:/app/dashboard-config.json \
  --name observability-dashboard \
  purekrome/simple-observability:latest
```

Access the dashboard at `http://localhost:8080`

### Running Locally

```bash
# Clone the repository
git clone https://github.com/PureKrome/SimpleObservability.git
cd SimpleObservability

# (Optional - run sample service for testing)
docker compose up

# Run the WebAPI
cd code/WebApi/WorldDomination.SimpleObservability.Dashboard
dotnet run
```

Access the dashboard at `http://localhost:5000`

## Configuration

### Using the Settings UI

The easiest way to configure the dashboard is through the built-in settings interface:

1. Navigate to the dashboard homepage
2. Click the **⚙️ Settings** link in the header
3. Configure system-wide settings:
   - **Default Timeout**: Set the default timeout for health check requests (in seconds)
   - **Refresh Interval**: Configure how often the dashboard auto-refreshes
   - **Environment Display Order**: Define the order in which environments appear (drag and drop to reorder)
4. Manage services:
   - **Add New Service**: Click "+ Add New Service" to add a service
   - **Edit Service**: Click "Edit" on any service to modify its settings
   - **Delete Service**: Click "Delete" to remove a service
   - **Service-Specific Timeout**: Optionally override the system timeout for individual services

**Note:** Currently, configuration changes are stored in-memory only. Persisting changes to file will be implemented in a future update.

#### Environment Ordering

You can define the display order of environments in the dashboard:
- Add environments to the order list (e.g., DEV, UAT, PROD)
- Drag and drop to reorder them
- Environments not in the list will appear last, sorted alphabetically
- This is useful for ensuring production environments always appear in a consistent position

### Manual Configuration

Create a `dashboard-config.json` file:

```json
{
  "services": [
    {
      "name": "Payment API",
      "environment": "PROD",
      "healthCheckUrl": "https://payment-api.example.com/healthz",
      "description": "Handles payment processing",
      "enabled": true,
      "timeoutSeconds": 10
    },
    {
      "name": "User Service",
      "environment": "PROD",
      "healthCheckUrl": "https://user-service.example.com/healthz",
      "description": "User authentication and management",
      "enabled": true
    }
  ],
  "refreshIntervalSeconds": 30,
  "timeoutSeconds": 5,
  "environmentOrder": ["DEV", "UAT", "PROD"]
}
```

### Configuration Options

| Field | Type | Description | Default |
|-------|------|-------------|---------|
| `services` | array | Service endpoints to monitor | `[]` |
| `refreshIntervalSeconds` | number | Dashboard auto-refresh interval | `30` |
| `timeoutSeconds` | number | Default timeout for health check requests | `5` |
| `environmentOrder` | array | Optional ordered list of environment names | `null` |

**Note:** The `environments` field is automatically computed from the unique environment values in the services list and ordered according to `environmentOrder` if specified.

### Service Configuration

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `name` | string | ✅ | Display name for the service |
| `environment` | string | ✅ | Which environment row this service appears in |
| `healthCheckUrl` | string | ✅ | Full URL to the health check endpoint |
| `description` | string | ❌ | Optional description |
| `enabled` | boolean | ❌ | Whether to monitor this service (default: true) |
| `timeoutSeconds` | number | ❌ | Service-specific timeout override (uses system default if not set) |

## Implementing Health Checks in Your Services

To integrate your services with Simple Observability, implement a health check endpoint that returns the expected JSON schema.

### Language-Specific Client Libraries

| Language | Repository | Status |
|----------|------------|--------|
| C# / .NET | [SimpleObservability.Client.DotNet](https://github.com/SimpleObservability/SimpleObservability.Client.DotNet) | ✅ Available |
| Node.js | Coming soon | 🚧 Planned |
| Python | Coming soon | 🚧 Planned |
| Go | Coming soon | 🚧 Planned |
| Java | Coming soon | 🚧 Planned |

### JSON Schema

Implement an endpoint (e.g. `GET /healthz`) that returns JSON using **camelCase** property names:

```json
{
  "serviceName": "My Service",
  "version": "1.0.0",
  "environment": "Production",
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

See [SCHEMA.md](docs/SCHEMA.md) for complete schema documentation.


## ⚖️ Dual License - Free for Qualified Users

**Simple Observability uses a dual-license model:**

✅ **FREE** for open source projects, students, non-profits, charities, and small businesses (<250 employees or <$1M revenue)  
💼 **Commercial License** required for larger organisations

📄 **[View License Details](LICENSE.md)** | 💰 **[View Pricing](docs/PRICING.md)**

## Features

- 🎯 **Simple Setup**: Just configure your service endpoints and go
- 🐳 **Docker Ready**: Designed to run as a container with volume-mounted configuration
- 🌍 **Multi-Environment**: Display services across DEV, UAT, PROD, or custom environments
- 📊 **Real-time Monitoring**: Auto-refreshing dashboard shows service health at a glance
- 🔧 **Flexible Schema**: Standard health check format that works with any technology stack
- 📦 **NuGet Package**: Easy-to-use library for .NET services (other languages supported via JSON)
- ⚙️ **Settings UI**: Manage configuration directly from the web interface

