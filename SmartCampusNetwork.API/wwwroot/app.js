const apiBase = '/api';

async function fetchJson(url, options = {}) {
    const response = await fetch(url, {
        headers: {
            'Content-Type': 'application/json',
            ...(options.headers || {})
        },
        ...options
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Request failed');
    }

    return response.json();
}

async function loadNetworkData() {
    const [devices, connections, incidents] = await Promise.all([
        fetchJson(`${apiBase}/devices`),
        fetchJson(`${apiBase}/connections`),
        fetchJson(`${apiBase}/incidents`)
    ]);

    renderDevices(devices);
    renderDeviceOptions(devices);
    renderIncidents(incidents);
    updateStats(devices, incidents);
    renderPriorityIncident(incidents);
    return { devices, connections, incidents };
}

function renderDevices(devices) {
    const tbody = document.getElementById('deviceTableBody');
    tbody.innerHTML = devices.map(device => `
        <tr>
            <td>${device.name}</td>
            <td>${device.type}</td>
            <td>${device.ipAddress}</td>
            <td>${device.location}</td>
            <td>${device.status}</td>
        </tr>
    `).join('');
}

function renderDeviceOptions(devices) {
    const sourceDevice = document.getElementById('sourceDevice');
    const targetDevice = document.getElementById('targetDevice');
    const faultDevice = document.getElementById('faultDevice');

    const options = devices.map(device => `<option value="${device.id}">${device.name}</option>`).join('');

    sourceDevice.innerHTML = options;
    targetDevice.innerHTML = options;
    faultDevice.innerHTML = options;
}

function renderIncidents(incidents) {
    const container = document.getElementById('incidentList');

    if (!incidents.length) {
        container.innerHTML = '<p>No incidents recorded.</p>';
        return;
    }

    container.innerHTML = incidents.map(incident => `
        <div class="incident-item">
            <h4>${incident.title}</h4>
            <p><strong>Severity:</strong> ${incident.severity}</p>
            <p><strong>Status:</strong> ${incident.status}</p>
            <p><strong>Priority:</strong> ${incident.priority}</p>
            <p>${incident.description}</p>
        </div>
    `).join('');
}

function updateStats(devices, incidents) {
    document.getElementById('deviceCount').textContent = devices.length;
    document.getElementById('incidentCount').textContent = incidents.length;
    document.getElementById('onlineCount').textContent = devices.filter(device => device.status === 'Online').length;
}

function renderPriorityIncident(incidents) {
    const target = document.getElementById('priorityIncident');

    if (!incidents.length) {
        target.textContent = 'No incidents yet.';
        return;
    }

    const topIncident = [...incidents].sort((a, b) => b.priority - a.priority)[0];
    target.textContent = `${topIncident.title} (${topIncident.severity}, Priority: ${topIncident.priority})`;
}

async function handleDeviceSubmit(event) {
    event.preventDefault();

    const device = {
        name: document.getElementById('deviceName').value,
        type: document.getElementById('deviceType').value,
        ipAddress: document.getElementById('deviceIp').value,
        macAddress: document.getElementById('deviceMac').value,
        location: document.getElementById('deviceLocation').value,
        status: 'Online'
    };

    await fetchJson(`${apiBase}/devices`, {
        method: 'POST',
        body: JSON.stringify(device)
    });

    event.target.reset();
    await loadNetworkData();
}

async function handleConnectionSubmit(event) {
    event.preventDefault();

    const connection = {
        sourceDeviceId: Number(document.getElementById('sourceDevice').value),
        targetDeviceId: Number(document.getElementById('targetDevice').value),
        cost: Number(document.getElementById('connectionCost').value),
        status: 'Active'
    };

    if (connection.sourceDeviceId === connection.targetDeviceId) {
        alert('A device cannot be connected to itself.');
        return;
    }

    await fetchJson(`${apiBase}/connections`, {
        method: 'POST',
        body: JSON.stringify(connection)
    });

    event.target.reset();
    await loadNetworkData();
}

async function handleFaultSubmit(event) {
    event.preventDefault();

    const deviceId = Number(document.getElementById('faultDevice').value);
    const description = document.getElementById('faultDescription').value;

    const result = await fetchJson(`${apiBase}/fault-analysis`, {
        method: 'POST',
        body: JSON.stringify({ deviceId, description })
    });

    document.getElementById('faultResult').innerHTML = `
        <strong>Failed device:</strong> ${result.failedDeviceName}<br>
        <strong>Severity:</strong> ${result.severity}<br>
        <strong>Priority:</strong> ${result.priority}<br>
        <strong>Summary:</strong> ${result.summary}<br>
        <strong>Unreachable devices:</strong> ${result.unreachableDeviceIds.length || 0}<br>
        <strong>Alternative routes:</strong><br>
        ${result.alternativeRoutes.length ? result.alternativeRoutes.map(route => `• ${route}<br>`).join('') : 'No alternative path available.'}
    `;

    await loadNetworkData();
}

document.getElementById('deviceForm').addEventListener('submit', handleDeviceSubmit);
document.getElementById('connectionForm').addEventListener('submit', handleConnectionSubmit);
document.getElementById('faultForm').addEventListener('submit', handleFaultSubmit);
document.getElementById('refreshBtn').addEventListener('click', loadNetworkData);

loadNetworkData().catch(error => {
    document.getElementById('faultResult').textContent = `Could not load network data: ${error.message}`;
    console.error(error);
});
