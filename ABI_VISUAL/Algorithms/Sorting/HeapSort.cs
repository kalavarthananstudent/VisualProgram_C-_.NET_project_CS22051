using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Sorting
{
    /// <summary>
    /// Heap Sort – animates heapify and extraction phases.
    /// </summary>
    public class HeapSort
    {
        private static int[] _arr = [];
        private static List<SortStep> _steps = [];

        public static List<SortStep> GenerateSteps(int[] input)
        {
            _arr   = (int[])input.Clone();
            _steps = new List<SortStep>();
            int n  = _arr.Length;

            // Build max-heap
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(n, i);

            // Extract elements from heap one by one
            for (int i = n - 1; i > 0; i--)
            {
                (_arr[0], _arr[i]) = (_arr[i], _arr[0]);
                _steps.Add(new SortStep(SortStepType.Swap, 0, i));
                _steps.Add(new SortStep(SortStepType.SetSorted, i));
                Heapify(i, 0);
            }
            _steps.Add(new SortStep(SortStepType.SetSorted, 0));

            return _steps;
        }

        private static void Heapify(int n, int root)
        {
            int largest = root;
            int left    = 2 * root + 1;
            int right   = 2 * root + 2;

            if (left < n)
            {
                _steps.Add(new SortStep(SortStepType.Compare, left, largest));
                if (_arr[left] > _arr[largest]) largest = left;
            }

            if (right < n)
            {
                _steps.Add(new SortStep(SortStepType.Compare, right, largest));
                if (_arr[right] > _arr[largest]) largest = right;
            }

            if (largest != root)
            {
                (_arr[root], _arr[largest]) = (_arr[largest], _arr[root]);
                _steps.Add(new SortStep(SortStepType.Swap, root, largest));
                Heapify(n, largest);
            }
        }
    }
}
