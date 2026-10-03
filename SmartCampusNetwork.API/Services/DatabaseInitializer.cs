using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SmartCampusNetwork.API.Services;

public class DatabaseInitializer
{
    private readonly string _databasePath;

    public DatabaseInitializer()
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(dataDirectory);
        _databasePath = Path.Combine(dataDirectory, "campus.db");
    }

    public string ConnectionString => $"Data Source={_databasePath}";

    public void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Devices (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                IpAddress TEXT NOT NULL,
                MacAddress TEXT,
                Location TEXT,
                Status TEXT NOT NULL,
                X INTEGER,
                Y INTEGER
            );

            CREATE TABLE IF NOT EXISTS Connections (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SourceDeviceId INTEGER NOT NULL,
                TargetDeviceId INTEGER NOT NULL,
                Cost INTEGER NOT NULL,
                Status TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Incidents (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                Severity TEXT NOT NULL,
                Status TEXT NOT NULL,
                DeviceId INTEGER NOT NULL,
                AffectedDevices TEXT,
                Priority INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
        ";

        command.ExecuteNonQuery();
    }

    public void SeedSampleData()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var deviceCheck = connection.CreateCommand();
        deviceCheck.CommandText = "SELECT COUNT(*) FROM Devices";
        var count = Convert.ToInt32(deviceCheck.ExecuteScalar());

        if (count > 0)
        {
            return;
        }

        var insertDevices = @"
            INSERT INTO Devices (Name, Type, IpAddress, MacAddress, Location, Status, X, Y)
            VALUES
                ('Internet Router', 'Router', '10.0.0.1', '00:11:22:33:44:01', 'Main Gate', 'Online', 120, 80),
                ('Core Switch', 'Switch', '10.0.0.2', '00:11:22:33:44:02', 'Administration Block', 'Online', 300, 80),
                ('Block A Switch', 'Switch', '10.0.0.3', '00:11:22:33:44:03', 'Block A', 'Online', 220, 220),
                ('Block B Switch', 'Switch', '10.0.0.4', '00:11:22:33:44:04', 'Block B', 'Online', 420, 220),
                ('Library AP', 'Access Point', '10.0.0.5', '00:11:22:33:44:05', 'Library', 'Online', 300, 340),
                ('Server Room', 'Server', '10.0.0.6', '00:11:22:33:44:06', 'Server Room', 'Online', 520, 80),
                ('PC-101', 'End Device', '10.0.0.11', '00:11:22:33:44:11', 'Block A Lab', 'Online', 160, 320),
                ('PC-205', 'End Device', '10.0.0.12', '00:11:22:33:44:12', 'Block B Lab', 'Online', 520, 340);
        ";

        using (var command = connection.CreateCommand())
        {
            command.CommandText = insertDevices;
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
                INSERT INTO Connections (SourceDeviceId, TargetDeviceId, Cost, Status)
                VALUES
                    (1, 2, 3, 'Active'),
                    (2, 3, 2, 'Active'),
                    (2, 4, 2, 'Active'),
                    (2, 5, 3, 'Active'),
                    (2, 6, 1, 'Active'),
                    (3, 7, 1, 'Active'),
                    (4, 8, 1, 'Active'),
                    (3, 5, 2, 'Active'),
                    (4, 5, 2, 'Active');
            ";
            command.ExecuteNonQuery();
        }
    }
}
