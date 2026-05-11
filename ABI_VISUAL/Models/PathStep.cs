using System.Collections.Generic;

namespace ABI_VISUAL.Models
{
    public enum PathStepType { Visit, Frontier, PathTrace, Done, NoPath }

    public class PathStep
    {
        public PathStepType       Type      { get; }
        public (int r, int c)     Cell      { get; }
        public List<(int,int)>?   Path      { get; }

        public PathStep(PathStepType type, (int,int) cell, List<(int,int)>? path = null)
        {
            Type = type; Cell = cell; Path = path;
        }
    }
}
