# Sample Service WebApi

A sample web API service that demonstrates the Simple Observability health check schema. This service exposes configurable health metadata that can be modified at runtime, making it useful for testing and development of the Simple Observability Dashboard.

## Overview

This service provides:

- A standard health check endpoint (`/healthz`) that returns health metadata in the Simple Observability format.
- A management endpoint (`PUT /health`) to dynamically update health metadata at runtime.
- A simple home endpoint (`/`) for basic service information.

## Running the Service

```bash
# From the project directory
dotnet run
```

By default, the service runs on `http://localhost:5000` (or the port configured in `launchSettings.json`).

## API Endpoints

### GET / - Hello World

Returns basic service information.

**Response:**

```json
{
  "message": "Hello from Sample Service!",
  "service": "Sample Service",
  "version": "1.0.0",
  "environment": "Development",
  "currentStatus": "Healthy"
}
```

**curl:**

```bash
curl http://localhost:5000/
```

---

### GET /healthz - Health Check

Returns the current health metadata in the Simple Observability standard format.

**Response:**

```json
{
  "serviceName": "Sample Service",
  "version": "1.0.0",
  "environment": "Development",
  "status": "Healthy",
  "timestamp": "2025-01-15T10:30:00.000Z",
  "uptime": "00:15:30.1234567",
  "description": "Sample service for testing Simple Observability",
  "hostName": "my-hostname",
  "additionalMetadata": {
    "Runtime": ".NET 10",
    "MachineName": "my-machine",
    "ProcessorCount": "8"
  }
}
```

**curl:**

```bash
curl http://localhost:5000/healthz
```

---

### PUT /health - Update Health Metadata

Updates the health metadata. All fields are optional - only the provided fields will be updated.

**Request Body:**

| Field         | Type   | Description                                              |
| ------------- | ------ | -------------------------------------------------------- |
| `status`      | string | Health status: `"Healthy"`, `"Degraded"`, or `"Unhealthy"` |
| `serviceName` | string | The service name                                         |
| `version`     | string | The service version                                      |
| `environment` | string | The environment name (e.g., Development, Production)     |
| `description` | string | A description of the service                             |
| `hostName`    | string | The hostname                                             |

**Response:**

```json
{
  "message": "Health metadata updated: Status=Degraded",
  "currentMetadata": {
    "status": "Degraded",
    "serviceName": "Sample Service",
    "version": "1.0.0",
    "environment": "Development",
    "description": "Sample service for testing Simple Observability",
    "hostName": "my-hostname"
  }
}
```

## curl Examples

### Set Status to Healthy

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"status": "Healthy"}'
```

### Set Status to Degraded

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"status": "Degraded"}'
```

### Set Status to Unhealthy

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"status": "Unhealthy"}'
```

### Update Service Name

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"serviceName": "My Custom Service"}'
```

### Update Version

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"version": "2.0.0"}'
```

### Update Environment

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"environment": "Production"}'
```

### Update Description

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"description": "This service is currently under maintenance"}'
```

### Update Hostname

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{"hostName": "production-server-01"}'
```

### Update Multiple Fields

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Degraded",
    "description": "Database connection pool exhausted",
    "environment": "Staging"
  }'
```

### Reset to Defaults

```bash
curl -X PUT http://localhost:5000/health \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Healthy",
    "serviceName": "Sample Service",
    "version": "1.0.0",
    "environment": "Development",
    "description": "Sample service for testing Simple Observability"
  }'
```

## PowerShell Examples

### Get Health Status

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/healthz" -Method Get
```

### Set Status to Unhealthy

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Put -ContentType "application/json" -Body '{"status": "Unhealthy"}'
```

### Update Multiple Fields

```powershell
$body = @{
    status = "Degraded"
    description = "High memory usage detected"
    version = "1.2.3"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Put -ContentType "application/json" -Body $body
```

## Use Cases

This sample service is useful for:

1. **Dashboard Development** - Test how the Simple Observability Dashboard handles different health states.
2. **Integration Testing** - Simulate various health scenarios in your CI/CD pipeline.
3. **Demo Purposes** - Quickly demonstrate the Simple Observability health check format.
4. **Learning** - Understand the expected health metadata schema.

## Health Status Values

| Status      | Description                                                           |
| ----------- | --------------------------------------------------------------------- |
| `Healthy`   | The service is healthy and operating normally.                        |
| `Degraded`  | The service is operational but experiencing degraded performance.     |
| `Unhealthy` | The service is unhealthy and not operating correctly.                 |
