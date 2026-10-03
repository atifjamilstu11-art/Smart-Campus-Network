using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using SmartCampusNetwork.API.Algorithms;
using SmartCampusNetwork.API.Models;

namespace SmartCampusNetwork.API.Services;

public class CampusNetworkService
{
    private readonly DatabaseInitializer _databaseInitializer;

    public CampusNetworkService(DatabaseInitializer databaseInitializer)
    {
        _databaseInitializer = databaseInitializer;
    }

    public void Initialize()
    {
        _databaseInitializer.Initialize();
        _databaseInitializer.SeedSampleData();
    }

    public List<Device> GetDevices()
    {
        var devices = new List<Device>();

        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Type, IpAddress, MacAddress, Location, Status, X, Y FROM Devices ORDER BY Id";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            devices.Add(new Device
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = reader.GetString(2),
                IpAddress = reader.GetString(3),
                MacAddress = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Location = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Status = reader.GetString(6),
                X = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                Y = reader.IsDBNull(8) ? null : reader.GetInt32(8)
            });
        }

        return devices;
    }

    public List<Connection> GetConnections()
    {
        var connections = new List<Connection>();

        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, SourceDeviceId, TargetDeviceId, Cost, Status FROM Connections ORDER BY Id";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            connections.Add(new Connection
            {
                Id = reader.GetInt32(0),
                SourceDeviceId = reader.GetInt32(1),
                TargetDeviceId = reader.GetInt32(2),
                Cost = reader.GetInt32(3),
                Status = reader.GetString(4)
            });
        }

        return connections;
    }

    public Device CreateDevice(Device device)
    {
        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Devices (Name, Type, IpAddress, MacAddress, Location, Status, X, Y)
            VALUES (@Name, @Type, @IpAddress, @MacAddress, @Location, @Status, @X, @Y);
        ";

        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@Type", device.Type);
        command.Parameters.AddWithValue("@IpAddress", device.IpAddress);
        command.Parameters.AddWithValue("@MacAddress", device.MacAddress ?? string.Empty);
        command.Parameters.AddWithValue("@Location", device.Location ?? string.Empty);
        command.Parameters.AddWithValue("@Status", device.Status);
        command.Parameters.AddWithValue("@X", device.X ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Y", device.Y ?? (object)DBNull.Value);
        command.ExecuteNonQuery();

        command.CommandText = "SELECT last_insert_rowid()";
        device.Id = Convert.ToInt32(command.ExecuteScalar());
        return device;
    }

    public Connection CreateConnection(Connection connection)
    {
        using var connectionDb = new SqliteConnection(_databaseInitializer.ConnectionString);
        connectionDb.Open();

        using var command = connectionDb.CreateCommand();
        command.CommandText = @"
            INSERT INTO Connections (SourceDeviceId, TargetDeviceId, Cost, Status)
            VALUES (@SourceDeviceId, @TargetDeviceId, @Cost, @Status);
        ";

        command.Parameters.AddWithValue("@SourceDeviceId", connection.SourceDeviceId);
        command.Parameters.AddWithValue("@TargetDeviceId", connection.TargetDeviceId);
        command.Parameters.AddWithValue("@Cost", connection.Cost);
        command.Parameters.AddWithValue("@Status", connection.Status);
        command.ExecuteNonQuery();

        command.CommandText = "SELECT last_insert_rowid()";
        connection.Id = Convert.ToInt32(command.ExecuteScalar());
        return connection;
    }

    public List<Incident> GetIncidents()
    {
        var incidents = new List<Incident>();

        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Description, Severity, Status, DeviceId, AffectedDevices, Priority, CreatedAt, UpdatedAt FROM Incidents ORDER BY Priority DESC, CreatedAt DESC";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            incidents.Add(new Incident
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Severity = reader.GetString(3),
                Status = reader.GetString(4),
                DeviceId = reader.GetInt32(5),
                AffectedDevices = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Priority = reader.GetInt32(7),
                CreatedAt = DateTime.Parse(reader.GetString(8)),
                UpdatedAt = DateTime.Parse(reader.GetString(9))
            });
        }

        return incidents;
    }

    public Incident CreateIncident(Incident incident)
    {
        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Incidents (Title, Description, Severity, Status, DeviceId, AffectedDevices, Priority, CreatedAt, UpdatedAt)
            VALUES (@Title, @Description, @Severity, @Status, @DeviceId, @AffectedDevices, @Priority, @CreatedAt, @UpdatedAt);
        ";

        command.Parameters.AddWithValue("@Title", incident.Title);
        command.Parameters.AddWithValue("@Description", incident.Description);
        command.Parameters.AddWithValue("@Severity", incident.Severity);
        command.Parameters.AddWithValue("@Status", incident.Status);
        command.Parameters.AddWithValue("@DeviceId", incident.DeviceId);
        command.Parameters.AddWithValue("@AffectedDevices", incident.AffectedDevices ?? string.Empty);
        command.Parameters.AddWithValue("@Priority", incident.Priority);
        command.Parameters.AddWithValue("@CreatedAt", incident.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@UpdatedAt", incident.UpdatedAt.ToString("O"));
        command.ExecuteNonQuery();

        command.CommandText = "SELECT last_insert_rowid()";
        incident.Id = Convert.ToInt32(command.ExecuteScalar());
        return incident;
    }

    public Incident UpdateIncidentStatus(int incidentId, string newStatus)
    {
        using var connection = new SqliteConnection(_databaseInitializer.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Incidents
            SET Status = @Status, UpdatedAt = @UpdatedAt
            WHERE Id = @IncidentId;
        ";

        command.Parameters.AddWithValue("@Status", newStatus);
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("@IncidentId", incidentId);
        command.ExecuteNonQuery();

        return GetIncidents().FirstOrDefault(incident => incident.Id == incidentId) ?? new Incident();
    }

    public FaultAnalysisResult AnalyzeFault(int failedDeviceId, string? description = null)
    {
        var devices = GetDevices();
        var failedDevice = devices.FirstOrDefault(device => device.Id == failedDeviceId);

        if (failedDevice is null)
        {
            throw new InvalidOperationException("Device not found.");
        }

        var connections = GetConnections().Where(connection => connection.Status == "Active").ToList();
        var graph = new Graph();

        foreach (var connection in connections)
        {
            graph.AddEdge(connection.SourceDeviceId, connection.TargetDeviceId, connection.Cost);
            graph.AddEdge(connection.TargetDeviceId, connection.SourceDeviceId, connection.Cost);
        }

        var reachableNodes = BFS.Traverse(graph, failedDeviceId).ToHashSet();
        var unreachableDeviceIds = devices
            .Where(device => device.Id != failedDeviceId && !reachableNodes.Contains(device.Id))
            .Select(device => device.Id)
            .ToList();

        var deviceMap = devices.ToDictionary(device => device.Id, device => device);
        var alternativeRoutes = new List<string>();

        foreach (var device in devices.Where(device => device.Id != failedDeviceId))
        {
            var path = Dijkstra.ShortestPath(graph, failedDeviceId, device.Id);

            if (path.Count > 1)
            {
                alternativeRoutes.Add(string.Join(" -> ", path.Select(nodeId => deviceMap[nodeId].Name)));
            }
        }

        var priorityScore = unreachableDeviceIds.Count * 3 + (alternativeRoutes.Count == 0 ? 4 : 0) + (failedDevice.Type == "Router" ? 2 : 0);
        if (priorityScore >= 12)
        {
            priorityScore = 12;
        }

        var severity = priorityScore >= 9 ? "Critical" : priorityScore >= 6 ? "High" : priorityScore >= 3 ? "Medium" : "Low";

        var result = new FaultAnalysisResult
        {
            FailedDeviceId = failedDeviceId,
            FailedDeviceName = failedDevice.Name,
            Summary = $"Fault detected on {failedDevice.Name}. {unreachableDeviceIds.Count} devices are unreachable from the failed network point.",
            ReachableDeviceIds = devices.Where(device => device.Id != failedDeviceId && reachableNodes.Contains(device.Id)).Select(device => device.Id).ToList(),
            UnreachableDeviceIds = unreachableDeviceIds,
            AlternativeRoutes = alternativeRoutes,
            Severity = severity,
            Priority = priorityScore
        };

        var incident = new Incident
        {
            Title = $"Fault on {failedDevice.Name}",
            Description = description ?? $"The device {failedDevice.Name} has failed and network impact is being assessed.",
            Severity = severity,
            Status = "Open",
            DeviceId = failedDeviceId,
            AffectedDevices = string.Join(",", unreachableDeviceIds),
            Priority = priorityScore,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        CreateIncident(incident);
        return result;
    }

    public Incident GetHighestPriorityIncident()
    {
        var incidents = GetIncidents();
        if (incidents.Count == 0)
        {
            return new Incident();
        }

        var queue = new PriorityQueue<Incident>(Comparer<Incident>.Create((a, b) => b.Priority.CompareTo(a.Priority)));
        foreach (var incident in incidents)
        {
            queue.Enqueue(incident);
        }

        return queue.Dequeue();
    }
}
