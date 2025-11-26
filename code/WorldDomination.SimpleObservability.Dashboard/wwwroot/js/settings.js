/**
 * Settings page-specific JavaScript functionality.
 */

let currentConfig = null;
let editingService = null;
let environmentOrder = [];
let draggedElement = null;
let autoSaveTimeout = null;
let isSaving = false;

/**
 * Loads the configuration from the API.
 */
async function loadConfiguration() {
    try {
        const response = await fetch('/api/config');
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        currentConfig = await response.json();
        environmentOrder = currentConfig.environmentOrder || [];
        populateSettings();
        renderServiceList();
        renderEnvironmentOrder();
    } catch (error) {
        console.error('Error loading configuration:', error);
        showAlert('Failed to load configuration: ' + error.message, 'error');
    }
}

/**
 * Populates the settings form with current configuration values.
 */
function populateSettings() {
    const timeoutInput = document.getElementById('systemTimeout');
    const refreshInput = document.getElementById('refreshInterval');
    
    timeoutInput.value = currentConfig.timeoutSeconds || 5;
    refreshInput.value = currentConfig.refreshIntervalSeconds || 30;
    
    // Add change listeners for auto-save.
    timeoutInput.addEventListener('change', scheduleAutoSave);
    refreshInput.addEventListener('change', scheduleAutoSave);
}

/**
 * Schedules an auto-save with debouncing.
 */
function scheduleAutoSave() {
    if (autoSaveTimeout) {
        clearTimeout(autoSaveTimeout);
    }

    updateAutoSaveStatus('Saving...');

    autoSaveTimeout = setTimeout(async () => {
        await autoSaveSystemSettings();
    }, 2000); // Wait 2 seconds after last change.
}

/**
 * Updates the auto-save status message.
 * @param {string} message - The status message to display.
 */
function updateAutoSaveStatus(message) {
    const statusElement = document.getElementById('autoSaveStatus');
    statusElement.textContent = message;
    
    if (message.includes('saved')) {
        statusElement.style.color = '#10b981';
        setTimeout(() => {
            statusElement.textContent = '';
        }, 3000);
    }
}

/**
 * Saves the system settings to the API.
 */
async function autoSaveSystemSettings() {
    if (isSaving) return;

    const timeoutSeconds = parseInt(document.getElementById('systemTimeout').value);
    const refreshIntervalSeconds = parseInt(document.getElementById('refreshInterval').value);

    // Create a proper configuration object with all required fields.
    const updatedConfig = {
        services: currentConfig.services, // Preserve existing services.
        timeoutSeconds,
        refreshIntervalSeconds,
        environmentOrder: environmentOrder.length > 0 ? environmentOrder : null
    };

    console.log('Saving configuration:', updatedConfig);
    console.log('Environment order being saved:', environmentOrder);

    try {
        isSaving = true;
        const response = await fetch('/api/config', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedConfig)
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Failed to save settings');
        }

        const result = await response.json();
        console.log('Save result:', result);
        console.log('Returned config environmentOrder:', result.config.environmentOrder);
        
        // Update both currentConfig and environmentOrder from the response.
        currentConfig = result.config;
        
        // IMPORTANT: Use environmentOrder (the user-configured order), not environments (the computed property).
        environmentOrder = result.config.environmentOrder || [];
        console.log('Local environmentOrder updated to:', environmentOrder);
        
        updateAutoSaveStatus('✓ Auto-saved');
    } catch (error) {
        console.error('Error auto-saving settings:', error);
        updateAutoSaveStatus('❌ Save failed');
        showAlert('Failed to auto-save settings: ' + error.message, 'error');
    } finally {
        isSaving = false;
    }
}

/**
 * Renders the environment order list.
 */
function renderEnvironmentOrder() {
    const container = document.getElementById('environmentOrderList');
    
    if (environmentOrder.length === 0) {
        container.innerHTML = '<div class="order-hint">No environments defined. Add environments below or they will be automatically detected from your services.</div>';
        return;
    }

    container.innerHTML = environmentOrder.map((env, index) => `
        <span class="environment-tag" draggable="true" data-index="${index}">
            ${env}
            <button class="remove-btn" onclick="removeEnvironmentFromOrder(${index})" title="Remove">×</button>
        </span>
    `).join('');

    // Add drag and drop event listeners.
    const tags = container.querySelectorAll('.environment-tag');
    tags.forEach(tag => {
        tag.addEventListener('dragstart', handleDragStart);
        tag.addEventListener('dragover', handleDragOver);
        tag.addEventListener('drop', handleDrop);
        tag.addEventListener('dragend', handleDragEnd);
    });
}

/**
 * Adds a new environment to the order list.
 */
function addEnvironmentToOrder() {
    const input = document.getElementById('newEnvironmentName');
    const envName = input.value.trim().toUpperCase();

    if (!envName) {
        showAlert('Please enter an environment name.', 'error');
        return;
    }

    if (environmentOrder.includes(envName)) {
        showAlert(`Environment "${envName}" is already in the list.`, 'error');
        return;
    }

    environmentOrder.push(envName);
    input.value = '';
    renderEnvironmentOrder();
    scheduleAutoSave();
}

/**
 * Removes an environment from the order list.
 * @param {number} index - The index of the environment to remove.
 */
function removeEnvironmentFromOrder(index) {
    environmentOrder.splice(index, 1);
    renderEnvironmentOrder();
    scheduleAutoSave();
}

/**
 * Handles the dragstart event for environment tags.
 * @param {DragEvent} e - The drag event.
 */
function handleDragStart(e) {
    draggedElement = e.target;
    e.target.classList.add('dragging');
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', e.target.innerHTML);
}

/**
 * Handles the dragover event for environment tags.
 * @param {DragEvent} e - The drag event.
 * @returns {boolean} False to prevent default behavior.
 */
function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault();
    }
    e.dataTransfer.dropEffect = 'move';
    return false;
}

/**
 * Handles the drop event for environment tags.
 * @param {DragEvent} e - The drag event.
 * @returns {boolean} False to prevent default behavior.
 */
function handleDrop(e) {
    if (e.stopPropagation) {
        e.stopPropagation();
    }

    if (draggedElement !== e.target && e.target.classList.contains('environment-tag')) {
        const draggedIndex = parseInt(draggedElement.dataset.index);
        const targetIndex = parseInt(e.target.dataset.index);

        console.log('Drag drop - Before reorder:', [...environmentOrder]);
        console.log(`Moving from index ${draggedIndex} to ${targetIndex}`);

        // Reorder the array.
        const item = environmentOrder[draggedIndex];
        environmentOrder.splice(draggedIndex, 1);
        environmentOrder.splice(targetIndex, 0, item);

        console.log('Drag drop - After reorder:', [...environmentOrder]);

        renderEnvironmentOrder();
        scheduleAutoSave();
    }

    return false;
}

/**
 * Handles the dragend event for environment tags.
 * @param {DragEvent} e - The drag event.
 */
function handleDragEnd(e) {
    e.target.classList.remove('dragging');
}

/**
 * Renders the list of services.
 */
function renderServiceList() {
    const serviceList = document.getElementById('serviceList');
    
    if (!currentConfig.services || currentConfig.services.length === 0) {
        serviceList.innerHTML = '<p style="text-align: center; color: #6b7280; padding: 20px;">No services configured yet.</p>';
        return;
    }

    // Sort services by name, then environment.
    const sortedServices = [...currentConfig.services].sort((a, b) => {
        const nameCompare = a.name.localeCompare(b.name);
        if (nameCompare !== 0) return nameCompare;
        return a.environment.localeCompare(b.environment);
    });

    serviceList.innerHTML = sortedServices.map(service => `
        <div class="service-item">
            <div class="service-header">
                <div class="service-title">
                    ${service.name} - ${service.environment}
                    ${!service.enabled ? ' <span style="color: #9ca3af;">(Disabled)</span>' : ''}
                </div>
                <div class="service-actions">
                    <button class="button button-secondary" onclick='editService(${JSON.stringify(service).replace(/'/g, "&#39;")})'>
                        Edit
                    </button>
                    <button class="button button-danger" onclick='deleteService("${service.name.replace(/"/g, '&quot;')}", "${service.environment.replace(/"/g, '&quot;')}")'>
                        Delete
                    </button>
                </div>
            </div>
            <div class="service-details">
                <div class="service-detail-item">
                    <span class="service-detail-label">URL:</span>
                    <span>${service.healthCheckUrl}</span>
                </div>
                ${service.description ? `
                <div class="service-detail-item">
                    <span class="service-detail-label">Description:</span>
                    <span>${service.description}</span>
                </div>
                ` : ''}
                ${service.timeoutSeconds ? `
                <div class="service-detail-item">
                    <span class="service-detail-label">Timeout:</span>
                    <span>${service.timeoutSeconds}s (overrides system default)</span>
                </div>
                ` : ''}
            </div>
        </div>
    `).join('');
}

/**
 * Opens the modal for adding a new service.
 */
function openAddServiceModal() {
    editingService = null;
    document.getElementById('modalTitle').textContent = 'Add Service';
    document.getElementById('serviceForm').reset();
    document.getElementById('originalServiceName').value = '';
    document.getElementById('originalEnvironment').value = '';
    document.getElementById('serviceEnabled').checked = true;
    document.getElementById('serviceModal').classList.add('active');
}

/**
 * Opens the modal for editing an existing service.
 * @param {Object} service - The service object to edit.
 */
function editService(service) {
    editingService = service;
    document.getElementById('modalTitle').textContent = 'Edit Service';
    document.getElementById('originalServiceName').value = service.name;
    document.getElementById('originalEnvironment').value = service.environment;
    document.getElementById('serviceName').value = service.name;
    document.getElementById('serviceEnvironment').value = service.environment;
    document.getElementById('serviceUrl').value = service.healthCheckUrl;
    document.getElementById('serviceDescription').value = service.description || '';
    document.getElementById('serviceTimeout').value = service.timeoutSeconds || '';
    document.getElementById('serviceEnabled').checked = service.enabled !== false;
    document.getElementById('serviceModal').classList.add('active');
}

/**
 * Closes the service modal.
 */
function closeServiceModal() {
    document.getElementById('serviceModal').classList.remove('active');
    editingService = null;
}

/**
 * Saves a service (add or update).
 * @param {Event} event - The form submit event.
 */
async function saveService(event) {
    event.preventDefault();

    const serviceData = {
        name: document.getElementById('serviceName').value.trim(),
        environment: document.getElementById('serviceEnvironment').value.trim(),
        healthCheckUrl: document.getElementById('serviceUrl').value.trim(),
        description: document.getElementById('serviceDescription').value.trim() || null,
        enabled: document.getElementById('serviceEnabled').checked
    };

    // Add timeout if specified.
    const timeoutValue = document.getElementById('serviceTimeout').value;
    if (timeoutValue) {
        serviceData.timeoutSeconds = parseInt(timeoutValue);
    }

    try {
        let response;
        
        if (editingService) {
            // Update existing service.
            const originalName = document.getElementById('originalServiceName').value;
            const originalEnv = document.getElementById('originalEnvironment').value;
            
            response = await fetch(`/api/config/services/${encodeURIComponent(originalName)}/${encodeURIComponent(originalEnv)}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(serviceData)
            });
        } else {
            // Add new service.
            response = await fetch('/api/config/services', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(serviceData)
            });
        }

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Failed to save service');
        }

        closeServiceModal();
        await loadConfiguration();
        showAlert(`Service "${serviceData.name}" ${editingService ? 'updated' : 'added'} successfully!`, 'success');
    } catch (error) {
        console.error('Error saving service:', error);
        showAlert('Failed to save service: ' + error.message, 'error');
    }
}

/**
 * Deletes a service.
 * @param {string} serviceName - The name of the service to delete.
 * @param {string} environment - The environment of the service to delete.
 */
async function deleteService(serviceName, environment) {
    if (!confirm(`Are you sure you want to delete "${serviceName}" in ${environment}?`)) {
        return;
    }

    try {
        const response = await fetch(`/api/config/services/${encodeURIComponent(serviceName)}/${encodeURIComponent(environment)}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Failed to delete service');
        }

        await loadConfiguration();
        showAlert(`Service "${serviceName}" deleted successfully!`, 'success');
    } catch (error) {
        console.error('Error deleting service:', error);
        showAlert('Failed to delete service: ' + error.message, 'error');
    }
}

/**
 * Initializes the settings page.
 */
function initializeSettings() {
    // Close modal when clicking outside.
    document.getElementById('serviceModal').addEventListener('click', (e) => {
        if (e.target.id === 'serviceModal') {
            closeServiceModal();
        }
    });

    // Initial load.
    loadConfiguration();
}

// Initialize when DOM is ready.
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeSettings);
} else {
    initializeSettings();
}
