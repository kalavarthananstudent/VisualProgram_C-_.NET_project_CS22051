using System;
using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Pathfinding
{
    /// <summary>
    /// Dijkstra's Algorithm on a uniform-cost 2D grid.
    /// All edges have weight 1, so this is equivalent to BFS but
    /// uses a priority queue to demonstrate the algorithm correctly.
    /// </summary>
    public class Dijkstra
    {
        public static List<PathStep> GenerateSteps(bool[,] walls, (int r, int c) start, (int r, int c) end)
        {
            int rows    = walls.GetLength(0);
            int cols    = walls.GetLength(1);
            var steps   = new List<PathStep>();
            var dist    = new Dictionary<(int,int), int>();
            var parent  = new Dictionary<(int,int),(int,int)>();
            var visited = new HashSet<(int,int)>();

            // Priority queue: (cost, cell)
            var pq = new SortedSet<(int cost, int r, int c)>(
                Comparer<(int,int,int)>.Create((a, b) =>
                    a.Item1 != b.Item1 ? a.Item1.CompareTo(b.Item1) :
                    a.Item2 != b.Item2 ? a.Item2.CompareTo(b.Item2) :
                    a.Item3.CompareTo(b.Item3)));

            dist[start] = 0;
            pq.Add((0, start.r, start.c));

            int[][] dirs = [[-1,0],[1,0],[0,-1],[0,1]];

            while (pq.Count > 0)
            {
                var (cost, r, c) = pq.Min;
                pq.Remove(pq.Min);
                var cell = (r, c);

                if (visited.Contains(cell)) continue;
                visited.Add(cell);
                steps.Add(new PathStep(PathStepType.Visit, cell));

                if (cell == end)
                {
                    var path = Reconstruct(parent, start, end);
                    steps.Add(new PathStep(PathStepType.PathTrace, end, path));
                    steps.Add(new PathStep(PathStepType.Done, end));
                    return steps;
                }

                foreach (var d in dirs)
                {
                    int nr = r + d[0], nc = c + d[1];
                    var next = (nr, nc);
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (walls[nr, nc] || visited.Contains(next)) continue;

                    int newDist = cost + 1;
                    if (!dist.ContainsKey(next) || newDist < dist[next])
                    {
                        dist[next]   = newDist;
                        parent[next] = cell;
                        pq.Add((newDist, nr, nc));
                        steps.Add(new PathStep(PathStepType.Frontier, next));
                    }
                }
            }

            steps.Add(new PathStep(PathStepType.NoPath, start));
            return steps;
        }

        private static List<(int,int)> Reconstruct(
            Dictionary<(int,int),(int,int)> parent, (int,int) start, (int,int) end)
        {
            var path = new List<(int,int)>();
            var cur  = end;
            while (cur != start) { path.Add(cur); cur = parent[cur]; }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
