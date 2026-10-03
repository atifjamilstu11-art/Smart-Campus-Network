using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCampusNetwork.API.Algorithms;

public class Graph
{
    private readonly Dictionary<int, List<GraphEdge>> _adjacency = new();

    public void AddEdge(int source, int target, double cost)
    {
        if (!_adjacency.ContainsKey(source))
        {
            _adjacency[source] = new List<GraphEdge>();
        }

        _adjacency[source].Add(new GraphEdge(target, cost));

        if (!_adjacency.ContainsKey(target))
        {
            _adjacency[target] = new List<GraphEdge>();
        }
    }

    public IEnumerable<int> GetNodes()
    {
        return _adjacency.Keys.Union(_adjacency.Values.SelectMany(v => v.Select(edge => edge.NodeId)));
    }

    public IEnumerable<GraphEdge> GetNeighbors(int node)
    {
        return _adjacency.TryGetValue(node, out var edges) ? edges : Enumerable.Empty<GraphEdge>();
    }
}

public class GraphEdge
{
    public GraphEdge(int nodeId, double cost)
    {
        NodeId = nodeId;
        Cost = cost;
    }

    public int NodeId { get; }
    public double Cost { get; }
}
