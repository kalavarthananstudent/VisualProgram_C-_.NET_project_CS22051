using System;
using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Pathfinding
{
    /// <summary>
    /// A* pathfinding using Manhattan distance heuristic.
    /// </summary>
    public class AStar
    {
        private static int Heuristic((int r, int c) a, (int r, int c) b)
            => Math.Abs(a.r - b.r) + Math.Abs(a.c - b.c);

        public static List<PathStep> GenerateSteps(bool[,] walls, (int r, int c) start, (int r, int c) end)
        {
            int rows    = walls.GetLength(0);
            int cols    = walls.GetLength(1);
            var steps   = new List<PathStep>();
            var gScore  = new Dictionary<(int,int), int>();
            var fScore  = new Dictionary<(int,int), int>();
            var parent  = new Dictionary<(int,int),(int,int)>();
            var closed  = new HashSet<(int,int)>();

            // Open set as sorted set on fScore
            var open = new SortedSet<(int f, int g, int r, int c)>(
                Comparer<(int,int,int,int)>.Create((a, b) => {
                    if (a.Item1 != b.Item1) return a.Item1.CompareTo(b.Item1);
                    if (a.Item2 != b.Item2) return a.Item2.CompareTo(b.Item2);
                    if (a.Item3 != b.Item3) return a.Item3.CompareTo(b.Item3);
                    return a.Item4.CompareTo(b.Item4);
                }));

            gScore[start] = 0;
            fScore[start] = Heuristic(start, end);
            open.Add((fScore[start], 0, start.r, start.c));

            int[][] dirs = [[-1,0],[1,0],[0,-1],[0,1]];

            while (open.Count > 0)
            {
                var (f, g, r, c) = open.Min;
                open.Remove(open.Min);
                var cell = (r, c);

                if (closed.Contains(cell)) continue;
                closed.Add(cell);
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
                    if (walls[nr, nc] || closed.Contains(next)) continue;

                    int tentativeG = g + 1;
                    if (!gScore.ContainsKey(next) || tentativeG < gScore[next])
                    {
                        parent[next] = cell;
                        gScore[next] = tentativeG;
                        fScore[next] = tentativeG + Heuristic(next, end);
                        open.Add((fScore[next], gScore[next], nr, nc));
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
