# Simple Observability Dashboard - Roadmap

This document outlines the planned features and enhancements for the Simple Observability Dashboard project. Items are organized by theme and priority.

## Legend
- ✅ Completed
- 🚧 In Progress
- 📋 Planned
- 💡 Idea/Future Consideration

---

## 🎨 User Experience & Interface

### Theme Support
**Status:** 📋 Planned  
**Priority:** High

Implement theme support to improve user experience across different preferences and environments.

**Features:**
- Light theme (default)
- Dark theme
- System preference detection (follows OS settings)
- Theme persistence across sessions
- Theme toggle in settings UI
- CSS custom properties for easy theme management
- Smooth transitions between themes

**Technical Considerations:**
- Use CSS variables for color schemes
- Store preference in localStorage/session
- Consider accessibility (WCAG contrast requirements)
- Apply theme to all pages (dashboard, settings, etc.)

---

## 🔔 Notification System

### Status Change Notifications
**Status:** 📋 Planned  
**Priority:** High

Alert users when a service transitions between health states (Healthy → Degraded → Unhealthy).

#### Notification Channels

##### Email Notifications
**Status:** 📋 Planned
- SMTP integration for email alerts
- Configurable recipients per service/environment
- Customizable email templates
- Rate limiting to prevent notification spam
- Aggregated digests (optional)

**Configuration:**
```json
{
  "notifications": {
    "email": {
      "enabled": true,
      "smtpServer": "smtp.example.com",
      "smtpPort": 587,
      "useSsl": true,
      "fromAddress": "observability@example.com",
      "recipients": ["ops-team@example.com"],
      "serviceSpecificRecipients": {
        "Payment API": ["payments-team@example.com"],
        "User Service": ["identity-team@example.com"]
      }
    }
  }
}
```

##### Webhook Notifications
**Status:** 📋 Planned  
**Priority:** High

Send HTTP POST requests to custom endpoints when health status changes.

**Features:**
- Configurable webhook URLs per service or globally
- Custom headers support (for authentication)
- Retry logic with exponential backoff
- Payload customization
- Signature validation (HMAC)

**Use Cases:**
- Integration with Slack, Microsoft Teams, Discord
- Custom alerting systems
- PagerDuty, Opsgenie integration
- Custom workflow automation

**Configuration:**
```json
{
  "notifications": {
    "webhooks": [
      {
        "name": "Slack Alerts",
        "url": "https://hooks.slack.com/services/YOUR/WEBHOOK/URL",
        "enabled": true,
        "headers": {
          "Content-Type": "application/json"
        },
        "filters": {
          "environments": ["PROD"],
          "statusChanges": ["Healthy -> Degraded", "* -> Unhealthy"]
        }
      },
      {
        "name": "Custom API",
        "url": "https://api.example.com/alerts",
        "enabled": true,
        "headers": {
          "Authorization": "Bearer YOUR_TOKEN",
          "X-Webhook-Secret": "your-secret"
        }
      }
    ]
  }
}
```

#### Notification Features
- Filter notifications by environment (e.g., only PROD alerts)
- Filter by status transition (e.g., only alert on degraded → unhealthy)
- Cooldown periods to prevent alert fatigue
- Notification history/audit log
- Test notification capability in settings UI

---

## 💾 Configuration Persistence

### Current State
**Status:** ✅ Completed
- JSON file configuration loading
- In-memory configuration updates via API
- Settings UI for configuration management

### File-Based Persistence
**Status:** 📋 Planned  
**Priority:** High

**Problem:** Configuration changes via the Settings UI are currently in-memory only and lost on restart.

**Solutions:**

#### Option 1: Direct File Write
Write configuration changes back to `dashboardsettings.json`.

**Pros:**
- Simple implementation
- Works in basic Docker scenarios with volume mounts
- No external dependencies

**Cons:**
- Not suitable for read-only filesystems
- Doesn't work well in container orchestration (Kubernetes, etc.)
- Challenging in Azure App Service with container support

#### Option 2: Cloud Storage Integration
Persist configuration to cloud storage providers.

##### Azure Blob Storage
**Status:** 💡 Idea
- Store configuration in Azure Blob Storage
- Read on startup, write on changes
- Works seamlessly with Azure App Service
- Support for different environments via container paths

**Configuration:**
```json
{
  "persistence": {
    "type": "AzureBlob",
    "connectionString": "DefaultEndpointsProtocol=https;...",
    "containerName": "observability-config",
    "blobName": "dashboard-config.json"
  }
}
```

##### AWS S3
**Status:** 💡 Idea
- Similar to Azure Blob approach
- Works with AWS ECS, EKS, App Runner

##### Generic HTTP Endpoint
**Status:** 💡 Idea
- Allow configuration to be fetched/updated via HTTP API
- Useful for custom configuration management systems

---

## 🔌 Plugin System

### Extensibility Architecture
**Status:** 💡 Idea  
**Priority:** Medium

Allow users to extend the dashboard functionality without modifying core code.

**Challenges:**
- .NET plugins require careful assembly loading
- Docker volume mounting of DLLs
- Type safety and versioning
- UI extensibility is complex

**Potential Plugin Types:**

#### 1. Persistence Plugins
Allow custom storage backends via plugin.

```csharp
public interface IPersistencePlugin
{
    Task<DashboardConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken);
    Task SaveConfigurationAsync(DashboardConfiguration config, CancellationToken cancellationToken);
}
```

**Docker Usage:**
```bash
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/plugins:/app/plugins \
  -e PLUGIN_PERSISTENCE=/app/plugins/MyCustomStorage.dll \
  simple-observatory/dashboard:latest
```

#### 2. Notification Plugins
Custom notification channels.

```csharp
public interface INotificationPlugin
{
    Task SendNotificationAsync(NotificationContext context, CancellationToken cancellationToken);
}
```

#### 3. Health Check Plugins
Custom health check logic/transformations.

```csharp
public interface IHealthCheckPlugin
{
    Task<HealthResult> ProcessHealthCheckAsync(HealthMetadata metadata, CancellationToken cancellationToken);
}
```

**UI Considerations:**
- Plugin settings could be JSON-based (no custom UI initially)
- Future: Consider Blazor Server components for UI extensibility
- Documentation templates for plugin developers

---

## 📊 Dashboard Enhancements

### Enhanced Visualizations
**Status:** 📋 Planned  
**Priority:** Medium

- Historical health data (uptime graphs)
- Response time trends
- Service dependency mapping
- Multi-dashboard support (different views for different teams)

### Service Grouping
**Status:** 📋 Planned  
**Priority:** Medium

- Group services by tags/categories
- Collapsible service groups
- Custom views per user/role

### Advanced Filtering
**Status:** 📋 Planned  
**Priority:** Low

- Filter services by status
- Search services by name
- Filter by environment
- Saved filter presets

---

## 🔐 Authentication & Authorization

**Status:** 💡 Idea  
**Priority:** Low

Currently, the dashboard has no authentication. This is intentional for simplicity, but may be needed for certain deployments.

**Options:**
- Basic authentication
- OAuth/OIDC integration
- Azure AD integration
- Read-only vs. admin roles
- Service-level permissions

**Note:** Many users deploy this behind a VPN or use reverse proxy authentication, so built-in auth may not be high priority.

---

## 🔧 Technical Improvements

### Health Check Enhancements
**Status:** 📋 Planned  
**Priority:** Medium

- Retry logic for transient failures
- Circuit breaker pattern
- Parallel health check execution with throttling
- Health check result caching (with TTL)

### Performance Optimizations
**Status:** 📋 Planned  
**Priority:** Low

- Response caching
- Compression (gzip/brotli)
- WebSocket support for real-time updates (instead of polling)
- Pagination for large service lists

### Observability for the Dashboard Itself
**Status:** 💡 Idea  
**Priority:** Low

- Export its own health endpoint
- Metrics endpoint (Prometheus format)
- Structured logging improvements
- Distributed tracing support

---

## 📦 Deployment & Operations

### Docker Improvements
**Status:** 📋 Planned  
**Priority:** Medium

- Multi-architecture images (ARM support)
- Smaller image size (Alpine base, trimmed runtime)
- Health check in Dockerfile
- Better environment variable support

### Kubernetes Deployment
**Status:** 📋 Planned  
**Priority:** Medium

- Helm chart
- ConfigMap/Secret integration
- Horizontal Pod Autoscaling support
- Ingress examples

### Configuration Management
**Status:** 📋 Planned  
**Priority:** Medium

- Environment variable overrides for all settings
- Secrets management integration (Azure Key Vault, AWS Secrets Manager)
- Configuration validation on startup
- Configuration hot-reload (without restart)

---

## 📖 Documentation

### User Documentation
**Status:** 🚧 In Progress

- ✅ Basic setup guide
- ✅ Configuration reference
- 📋 Troubleshooting guide
- 📋 Best practices guide
- 📋 Video tutorials

### Developer Documentation
**Status:** 📋 Planned

- Plugin development guide
- API documentation (OpenAPI/Swagger)
- Architecture decision records (ADRs)
- Contribution guidelines

---

## 🌍 Internationalization

**Status:** 💡 Idea  
**Priority:** Low

- Multi-language support
- Localized date/time formats
- Timezone handling

---

## 🧪 Testing & Quality

**Status:** 🚧 In Progress

- ✅ Basic unit tests
- 📋 Integration tests
- 📋 End-to-end tests (Playwright/Selenium)
- 📋 Performance tests
- 📋 Load tests for large service counts

---

## Release Milestones

### v1.0.0 (Current)
- ✅ Core dashboard functionality
- ✅ Settings UI
- ✅ JSON configuration
- ✅ Docker support

### v1.1.0 (Next)
- Theme support (light/dark/system)
- Configuration file persistence
- Webhook notifications
- Enhanced error handling

### v1.2.0
- Email notifications
- Azure Blob Storage persistence
- Historical data tracking
- Response time monitoring

### v2.0.0 (Future)
- Plugin system architecture
- Authentication/authorization
- WebSocket real-time updates
- Advanced visualizations

---

## Contributing

We welcome contributions! If you'd like to work on any of these features, please:
1. Open an issue to discuss the feature
2. Reference this roadmap in your PR
3. Follow our [contribution guidelines](.github/CONTRIBUTING.md)

---

## Feedback

Have ideas not on this roadmap? Open an issue with the `enhancement` label to discuss!
