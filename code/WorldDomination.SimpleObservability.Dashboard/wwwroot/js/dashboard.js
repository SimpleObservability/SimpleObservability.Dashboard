/**
 * Dashboard-specific JavaScript functionality.
 */

let refreshInterval;
let countdownInterval;
let intervalSeconds = 30;
let countdownValue = 30;

/**
 * Loads the dashboard health data from the API.
 */
async function loadDashboard() {
    try {
        // Update countdown to show "Refreshing...".
        updateCountdownDisplay('...');

        const response = await fetch('/api/health');
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const data = await response.json();
        renderDashboard(data);
        updateLastUpdated();
        
        // Update refresh interval if it changed.
        intervalSeconds = data.refreshIntervalSeconds || 30;
        
        // Reset countdown to the full interval.
        countdownValue = intervalSeconds;
        updateCountdownDisplay(countdownValue);
        
        document.getElementById('loading').style.display = 'none';
        document.getElementById('dashboardTable').style.display = 'table';
    } catch (error) {
        console.error('Error loading dashboard:', error);
        document.getElementById('loading').innerHTML = `
            <div style="color: #ef4444;">
                <p style="font-weight: 600; margin-bottom: 10px;">⚠️ Error Loading Dashboard</p>
                <p>${error.message}</p>
            </div>
        `;
        
        // Reset countdown even on error.
        countdownValue = intervalSeconds;
        updateCountdownDisplay(countdownValue);
    }
}

/**
 * Renders the dashboard table with health data.
 * @param {Object} data - The health data from the API.
 */
function renderDashboard(data) {
    const { environments, services, results } = data;

    // Build header with service names.
    const header = document.getElementById('tableHeader');
    header.innerHTML = '<th>Environment</th>';
    
    // Get unique service names for columns.
    const serviceNames = [...new Set(services.map(s => s.name))];
    serviceNames.forEach(name => {
        const th = document.createElement('th');
        th.textContent = name;
        header.appendChild(th);
    });

    // Build rows for each environment.
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '';

    environments.forEach(env => {
        const row = document.createElement('tr');
        
        // Environment cell.
        const envCell = document.createElement('td');
        envCell.className = 'environment-cell';
        envCell.textContent = env;
        row.appendChild(envCell);

        // Service cells.
        serviceNames.forEach(serviceName => {
            const cell = document.createElement('td');
            const service = services.find(s => s.name === serviceName && s.environment === env);
            
            if (service) {
                // Use composite key: name + environment to match backend.
                const resultKey = `${serviceName}|${env}`;
                const result = results[resultKey];
                if (result && result.isSuccess) {
                    cell.innerHTML = renderServiceCard(result);
                } else if (result) {
                    // Check if service is disabled vs. has an error.
                    if (result.isDisabled) {
                        cell.innerHTML = renderDisabledCard(serviceName, result);
                    } else {
                        cell.innerHTML = renderErrorCard(serviceName, result);
                    }
                } else {
                    cell.innerHTML = '<div class="empty-cell">No data</div>';
                }
            } else {
                cell.innerHTML = '<div class="empty-cell">-</div>';
            }
            
            row.appendChild(cell);
        });

        tbody.appendChild(row);
    });
}

/**
 * Renders a healthy service card.
 * @param {Object} result - The service health result.
 * @returns {string} HTML string for the service card.
 */
function renderServiceCard(result) {
    const metadata = result.healthMetadata;
    
    // Defensive check: ensure metadata exists and has required properties.
    if (!metadata || typeof metadata !== 'object') {
        return renderErrorCard(result.serviceEndpoint?.name || 'Unknown', { 
            errorMessage: 'No metadata returned' 
        });
    }

    const statusClass = getStatusClass(metadata.status ?? 0);
    const statusLabel = getStatusLabel(metadata.status ?? 0);
    const serviceName = metadata.serviceName || result.serviceEndpoint?.name || 'Unknown Service';
    const version = metadata.version || 'N/A';

    let html = `
        <div class="service-card ${statusClass}">
            <div class="service-name">${serviceName}</div>
            <div class="service-version">v${version}</div>
            <span class="service-status status-${statusClass}">${statusLabel}</span>
    `;

    if (metadata.description) {
        html += `<div class="metadata-info">${metadata.description}</div>`;
    }

    if (metadata.hostName) {
        html += `<div class="metadata-info">Host: ${metadata.hostName}</div>`;
    }

    // Add tooltip with additional metadata.
    html += renderTooltip(metadata);

    html += '</div>';
    return html;
}

/**
 * Renders a tooltip with additional metadata information.
 * @param {Object} metadata - The health metadata object.
 * @returns {string} HTML string for the tooltip.
 */
function renderTooltip(metadata) {
    const additionalMetadata = metadata.additionalMetadata;
    const hasAdditionalData = additionalMetadata && Object.keys(additionalMetadata).length > 0;
    const hasUptime = metadata.uptime;
    const hasTimestamp = metadata.timestamp;

    // Only show tooltip if there's something to display.
    if (!hasAdditionalData && !hasUptime && !hasTimestamp) {
        return '';
    }

    let tooltipContent = '<div class="tooltip-content">';

    // Add timestamp if available.
    if (hasTimestamp) {
        const timestamp = new Date(metadata.timestamp);
        tooltipContent += `
            <div class="tooltip-row">
                <span class="tooltip-key">Checked:</span>
                <span class="tooltip-value">${timestamp.toLocaleString()}</span>
            </div>
        `;
    }

    // Add uptime if available.
    if (hasUptime) {
        tooltipContent += `
            <div class="tooltip-row">
                <span class="tooltip-key">Uptime:</span>
                <span class="tooltip-value">${formatUptime(metadata.uptime)}</span>
            </div>
        `;
    }

    // Add additional metadata key/value pairs.
    if (hasAdditionalData) {
        Object.entries(additionalMetadata).forEach(([key, value]) => {
            // Escape HTML to prevent XSS.
            const safeKey = escapeHtml(key);
            const safeValue = escapeHtml(value);
            
            tooltipContent += `
                <div class="tooltip-row">
                    <span class="tooltip-key">${safeKey}:</span>
                    <span class="tooltip-value">${safeValue}</span>
                </div>
            `;
        });
    }

    tooltipContent += '</div>';

    return `
        <div class="tooltip">
            <div class="tooltip-title">Additional Information</div>
            ${tooltipContent}
        </div>
    `;
}

/**
 * Formats a TimeSpan string into a human-readable format.
 * @param {string} uptime - TimeSpan string (e.g., "1.23:45:67.890").
 * @returns {string} Formatted uptime string.
 */
function formatUptime(uptime) {
    if (!uptime) return 'N/A';
    
    // Parse TimeSpan format (days.hours:minutes:seconds.milliseconds).
    const parts = uptime.split('.');
    const timeParts = (parts[0].includes(':') ? parts[0] : parts[1] || '').split(':');
    
    const days = parts.length > 2 ? parseInt(parts[0]) : 0;
    const hours = parseInt(timeParts[0] || 0);
    const minutes = parseInt(timeParts[1] || 0);
    
    const result = [];
    if (days > 0) result.push(`${days}d`);
    if (hours > 0) result.push(`${hours}h`);
    if (minutes > 0) result.push(`${minutes}m`);
    
    return result.length > 0 ? result.join(' ') : '< 1m';
}

/**
 * Escapes HTML special characters to prevent XSS attacks.
 * @param {string} text - Text to escape.
 * @returns {string} Escaped text.
 */
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Renders an error service card.
 * @param {string} serviceName - The name of the service.
 * @param {Object} result - The service health result.
 * @returns {string} HTML string for the error card.
 */
function renderErrorCard(serviceName, result) {
    return `
        <div class="service-card error">
            <div class="service-name">${serviceName}</div>
            <div class="error-message">⚠️ ${result.errorMessage || 'Unknown error'}</div>
        </div>
    `;
}

/**
 * Renders a disabled service card.
 * @param {string} serviceName - The name of the service.
 * @param {Object} result - The service health result.
 * @returns {string} HTML string for the disabled card.
 */
function renderDisabledCard(serviceName, result) {
    return `
        <div class="service-card disabled">
            <div class="service-name">${serviceName}</div>
            <div class="error-message" style="color: #6b7280;">Service is disabled</div>
        </div>
    `;
}

/**
 * Gets the CSS class for a health status.
 * @param {number} status - The health status enum value.
 * @returns {string} The CSS class name.
 */
function getStatusClass(status) {
    switch(status) {
        case 0: return 'healthy';
        case 1: return 'degraded';
        case 2: return 'unhealthy';
        default: return 'healthy';
    }
}

/**
 * Gets the label for a health status.
 * @param {number} status - The health status enum value.
 * @returns {string} The status label.
 */
function getStatusLabel(status) {
    switch(status) {
        case 0: return 'Healthy';
        case 1: return 'Degraded';
        case 2: return 'Unhealthy';
        default: return 'Unknown';
    }
}

/**
 * Updates the last updated timestamp display.
 */
function updateLastUpdated() {
    const now = new Date();
    document.getElementById('lastUpdated').textContent = now.toLocaleTimeString();
}

/**
 * Updates the countdown display.
 * @param {string|number} value - The countdown value to display.
 */
function updateCountdownDisplay(value) {
    const countdownElement = document.getElementById('countdown');
    countdownElement.textContent = value;
}

/**
 * Starts the countdown timer.
 */
function startCountdown() {
    // Clear any existing countdown interval.
    if (countdownInterval) {
        clearInterval(countdownInterval);
    }

    // Start countdown that decrements every second.
    countdownInterval = setInterval(() => {
        countdownValue--;
        
        if (countdownValue <= 0) {
            updateCountdownDisplay('...');
        } else {
            updateCountdownDisplay(countdownValue);
        }
    }, 1000);
}

/**
 * Starts the auto-refresh interval.
 */
function startAutoRefresh() {
    if (refreshInterval) {
        clearInterval(refreshInterval);
    }
    
    refreshInterval = setInterval(() => {
        loadDashboard();
    }, intervalSeconds * 1000);
    
    // Start the countdown timer.
    startCountdown();
}

/**
 * Initializes the dashboard.
 */
function initializeDashboard() {
    // Initial load.
    loadDashboard();

    // Start auto-refresh and countdown after initial load.
    setTimeout(() => {
        startAutoRefresh();
    }, 1000);
}

// Initialize when DOM is ready.
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDashboard);
} else {
    initializeDashboard();
}
