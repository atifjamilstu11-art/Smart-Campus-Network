using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCampusNetwork.API.Algorithms;

public static class Dijkstra
{
    public static List<int> ShortestPath(Graph graph, int startNode, int targetNode)
    {
        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int>();
        var priorityQueue = new PriorityQueue<(int Node, double Distance)>(Comparer<(int Node, double Distance)>.Create((a, b) =>
        {
            var comparison = a.Distance.CompareTo(b.Distance);
            return comparison != 0 ? comparison : a.Node.CompareTo(b.Node);
        }));

        priorityQueue.Enqueue((startNode, 0));
        distances[startNode] = 0;

        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Dequeue();

            if (current.Node == targetNode)
            {
                break;
            }

            foreach (var edge in graph.GetNeighbors(current.Node))
            {
                var candidateDistance = current.Distance + edge.Cost;

                if (!distances.TryGetValue(edge.NodeId, out var bestDistance) || candidateDistance < bestDistance)
                {
                    distances[edge.NodeId] = candidateDistance;
                    previous[edge.NodeId] = current.Node;
                    priorityQueue.Enqueue((edge.NodeId, candidateDistance));
                }
            }
        }

        if (!distances.ContainsKey(targetNode))
        {
            return new List<int>();
        }

        var path = new List<int>();
        var currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            if (!previous.TryGetValue(currentNode, out var previousNode))
            {
                return new List<int>();
            }

            currentNode = previousNode;
        }

        path.Add(startNode);
        path.Reverse();
        return path;
    }
}
