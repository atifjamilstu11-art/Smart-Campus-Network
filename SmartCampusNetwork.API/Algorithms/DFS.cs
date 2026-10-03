using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCampusNetwork.API.Algorithms;

public static class DFS
{
    public static List<int> Traverse(Graph graph, int startNode)
    {
        var visited = new HashSet<int>();
        var result = new List<int>();

        void Visit(int node)
        {
            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);
            result.Add(node);

            foreach (var edge in graph.GetNeighbors(node))
            {
                Visit(edge.NodeId);
            }
        }

        Visit(startNode);
        return result;
    }
}
