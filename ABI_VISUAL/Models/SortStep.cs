namespace ABI_VISUAL.Models
{
    public enum SortStepType
    {
        Compare,
        Swap,
        Overwrite,   // for merge sort auxiliary writes
        SetSorted,
        MarkPivot,
        MarkLeft,
        MarkRight,
        ClearMarks,
    }

    /// <summary>
    /// A single atomic step produced by a sorting algorithm iterator.
    /// </summary>
    public class SortStep
    {
        public SortStepType Type    { get; }
        public int          IndexA  { get; }
        public int          IndexB  { get; }
        public int          Value   { get; }   // used for Overwrite
        public int[]?       Array   { get; }   // snapshot (optional, for overwrite)

        public SortStep(SortStepType type, int a, int b = -1, int value = 0, int[]? arr = null)
        {
            Type   = type;
            IndexA = a;
            IndexB = b;
            Value  = value;
            Array  = arr;
        }
    }
}
