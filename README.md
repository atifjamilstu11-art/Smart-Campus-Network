# Smart Campus Network Fault Detection & Recovery System

A full working project for a campus network troubleshooting assistant built with:
- ASP.NET Core Web API (C#)
- SQLite database
- C# graph algorithms (BFS, DFS, Dijkstra)
- Priority queue implementation
- HTML/CSS/JavaScript UI

## Features
- Device management
- Connection management
- Trigger fault analysis from a failed device
- Reachability analysis using BFS/DFS
- Alternative path detection using Dijkstra
- Incident tracking with priority scoring
- Dashboard with live statistics

## Run locally

### Prerequisites
- .NET 8 SDK

### 1. Restore and build
```bash
dotnet restore
 dotnet build
```

### 2. Run the API
```bash
dotnet run --project SmartCampusNetwork.API
```

### 3. Open the app
Open the browser at:
```text
http://localhost:5000
```

The app serves the frontend directly from the backend.

## Project structure
```text
Smart-Campus-Network/
├── SmartCampusNetwork.API/
│   ├── Algorithms/
│   ├── Models/
│   ├── Services/
│   ├── wwwroot/
│   ├── Program.cs
│   └── SmartCampusNetwork.API.csproj
├── README.md
└── .gitignore
```

## Suggested demo scenario
1. Open the dashboard.
2. View sample campus network devices.
3. Select a failing device such as Block B Switch.
4. Click Analyze Fault.
5. View affected devices, shortest routes, and generated incident.

## Academic mapping
- Graph + adjacency list: implemented in `Algorithms/Graph.cs`
- BFS: `Algorithms/BFS.cs`
- DFS: `Algorithms/DFS.cs`
- Dijkstra: `Algorithms/Dijkstra.cs`
- Priority queue: `Algorithms/PriorityQueue.cs`
- Web API + database: `Program.cs` and `Services/CampusNetworkService.cs`
