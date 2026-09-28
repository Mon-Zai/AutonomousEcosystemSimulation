using System.Collections.Generic;
using AI.Pathfinding;
using UnityEngine;

public static class Pathfinding
{
    public static List<Node> CalculateAStar(Node start, Node goal)
    {
        PriorityQueue frontier = new PriorityQueue();
        frontier.Enqueue(start, 0);
        Dictionary<Node, Node> cameFrom = new();
        cameFrom.Add(start, null);
        Dictionary<Node, int> costSoFar = new();
        costSoFar.Add(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                List<Node> path = new List<Node>();

                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Add(current);
                path.Reverse();

                return path;
            }

            foreach (var next in current.Neighbors)
            {
                var newCost = costSoFar[current] + next.Cost;

                if (!cameFrom.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost + Vector3.Distance(next.transform.position, goal.transform.position);
                    frontier.Enqueue(next, priority);
                    if (cameFrom.ContainsKey(next))
                    {
                        cameFrom[next] = current;
                    }
                    else cameFrom.Add(next, current);
                }
            }
        }

        return new List<Node>();
    }
}
