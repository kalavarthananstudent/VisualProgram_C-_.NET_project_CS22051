using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Pathfinding
{
    public class BFS
    {
        public static List<PathStep> GenerateSteps(bool[,] walls, (int r, int c) start, (int r, int c) end)
        {
            int rows   = walls.GetLength(0);
            int cols   = walls.GetLength(1);
            var steps  = new List<PathStep>();
            var parent = new Dictionary<(int, int), (int, int)>();
            var visited= new HashSet<(int, int)>();
            var queue  = new Queue<(int, int)>();

            queue.Enqueue(start);
            visited.Add(start);

            int[][] dirs = new int[][] {
                new int[] {-1, 0}, new int[] {1, 0},
                new int[] {0, -1}, new int[] {0, 1}
            };

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                int cr = cell.Item1, cc = cell.Item2;
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
                    int nr = cr + d[0];
                    int nc = cc + d[1];
                    var next = (nr, nc);

                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols
                        && !walls[nr, nc] && !visited.Contains(next))
                    {
                        visited.Add(next);
                        parent[next] = cell;
                        queue.Enqueue(next);
                        steps.Add(new PathStep(PathStepType.Frontier, next));
                    }
                }
            }

            steps.Add(new PathStep(PathStepType.NoPath, start));
            return steps;
        }

        private static List<(int, int)> Reconstruct(
            Dictionary<(int, int), (int, int)> parent,
            (int, int) start, (int, int) end)
        {
            var path = new List<(int, int)>();
            var cur  = end;
            while (cur != start) { path.Add(cur); cur = parent[cur]; }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
