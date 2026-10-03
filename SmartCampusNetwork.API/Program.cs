using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SmartCampusNetwork.API.Algorithms;
using SmartCampusNetwork.API.Models;
using SmartCampusNetwork.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<CampusNetworkService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");

var networkService = app.Services.GetRequiredService<CampusNetworkService>();
networkService.Initialize();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.MapGet("/api/devices", () => Results.Ok(networkService.GetDevices()));
app.MapPost("/api/devices", (Device device) => Results.Ok(networkService.CreateDevice(device)));

app.MapGet("/api/connections", () => Results.Ok(networkService.GetConnections()));
app.MapPost("/api/connections", (Connection connection) => Results.Ok(networkService.CreateConnection(connection)));

app.MapGet("/api/incidents", () => Results.Ok(networkService.GetIncidents()));
app.MapPost("/api/incidents", (Incident incident) => Results.Ok(networkService.CreateIncident(incident)));
app.MapPut("/api/incidents/{incidentId}/status", (int incidentId, [FromBody] StatusUpdateRequest update) =>
{
    var result = networkService.UpdateIncidentStatus(incidentId, update.Status);
    return Results.Ok(result);
});

app.MapPost("/api/fault-analysis", (FaultRequest request) =>
{
    var result = networkService.AnalyzeFault(request.DeviceId, request.Description);
    return Results.Ok(result);
});

app.MapGet("/api/highest-priority-incident", () => Results.Ok(networkService.GetHighestPriorityIncident()));

app.Run();

public record FaultRequest(int DeviceId, string? Description = null);
public record StatusUpdateRequest(string Status);
