/**
 * Common utility functions for displaying alerts.
 */

/**
 * Shows an alert message on the page.
 * @param {string} message - The message to display.
 * @param {string} type - The alert type: 'success', 'error', or 'info'.
 * @param {string} containerId - The ID of the container element (default: 'alertContainer').
 */
function showAlert(message, type, containerId = 'alertContainer') {
    const alertContainer = document.getElementById(containerId);
    if (!alertContainer) {
        console.error(`Alert container with ID '${containerId}' not found.`);
        return;
    }

    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.textContent = message;
    alertContainer.appendChild(alert);

    setTimeout(() => {
        alert.remove();
    }, 5000);
}
