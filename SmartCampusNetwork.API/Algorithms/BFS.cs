using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCampusNetwork.API.Algorithms;

public static class BFS
{
    public static List<int> Traverse(Graph graph, int startNode)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        var results = new List<int>();

        visited.Add(startNode);
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            results.Add(current);

            foreach (var edge in graph.GetNeighbors(current))
            {
                if (visited.Contains(edge.NodeId))
                {
                    continue;
                }

                visited.Add(edge.NodeId);
                queue.Enqueue(edge.NodeId);
            }
        }

        return results;
    }
}
