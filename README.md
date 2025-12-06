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

The Docker image ships with an empty configuration. You need to provide your own configuration by volume-mounting a file.

```bash
# Pull the image
docker pull simple-observatory/dashboard:latest

# Run with volume-mounted configuration
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/dashboard-config.json:/app/dashboard-config.json \
  --name observability-dashboard \
  simple-observatory/dashboard:latest
```

> **Note:** The Docker image does not include sample services. Sample services are only available when running locally in Development mode for testing purposes.

Access the dashboard at `http://localhost:8080`

### Running Locally

When running locally in Development mode, the dashboard includes sample services for testing:

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

> **Note:** Sample services are automatically loaded from `dashboardsettings.Development.json` when running in Development mode. This file is excluded from Docker builds.

Access the dashboard at `http://localhost:5000`

## Configuration

### Using the Settings UI

The easiest way to configure the dashboard is through the built-in settings interface:

1. Navigate to the dashboard homepage
2. Click the **⚙️ Settings** link in the header
3. Use the tab navigation to switch between:
   - **System Settings**: Configure system-wide settings and manage services
   - **Raw Configuration (Advanced)**: Bulk import/export via JSON editor

#### System Settings Tab

Configure system-wide settings and manage individual services:

1. **System Configuration**:
   - **Default Timeout**: Set the default timeout for health check requests (in seconds)
   - **Refresh Interval**: Configure how often the dashboard auto-refreshes
   - **Environment Display Order**: Define the order in which environments appear (drag and drop to reorder)

2. **Service Management**:
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

#### Raw Configuration (Advanced) Tab

For advanced users or bulk operations, switch to the **Raw Configuration (Advanced)** tab:

1. The JSON format matches the `dashboardsettings.json` file structure
2. You can:
   - View the entire configuration as formatted JSON
   - Copy the entire configuration to use in another environment
   - Paste in a pre-configured JSON to quickly set up multiple services
   - Make bulk changes more efficiently than through the UI forms
3. Click **Save JSON Configuration** to apply changes immediately
4. All changes update the in-memory configuration instantly

**Available Actions:**
- **💾 Save JSON Configuration**: Validates and saves changes to in-memory configuration
- **↻ Reload from Server**: Refreshes the editor with the current server configuration
- **📋 Copy to Clipboard**: One-click copy of the entire configuration

**Example JSON format:**
```json
{
  "Dashboard": {
    "services": [
      {
        "name": "My Service",
        "environment": "DEV",
        "healthCheckUrl": "http://localhost:5001/healthz",
        "description": "Development instance",
        "enabled": true
      }
    ],
    "refreshIntervalSeconds": 30,
    "timeoutSeconds": 5,
    "environmentOrder": ["PROD", "UAT", "DEV"]
  }
}
```

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
- 📝 **JSON Editor**: Bulk import/export configuration with raw JSON editing support

