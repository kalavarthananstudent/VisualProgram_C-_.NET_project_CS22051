using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Sorting
{
    /// <summary>
    /// Quick Sort (Lomuto partition, last element pivot).
    /// Animates comparisons, swaps, and pivot placement.
    /// </summary>
    public class QuickSort
    {
        private static int[] _arr = [];
        private static List<SortStep> _steps = [];

        public static List<SortStep> GenerateSteps(int[] input)
        {
            _arr   = (int[])input.Clone();
            _steps = new List<SortStep>();
            QuickSortRecursive(0, _arr.Length - 1);

            for (int i = 0; i < _arr.Length; i++)
                _steps.Add(new SortStep(SortStepType.SetSorted, i));

            return _steps;
        }

        private static void QuickSortRecursive(int low, int high)
        {
            if (low >= high) return;
            int pi = Partition(low, high);
            QuickSortRecursive(low, pi - 1);
            QuickSortRecursive(pi + 1, high);
        }

        private static int Partition(int low, int high)
        {
            int pivot = _arr[high];
            _steps.Add(new SortStep(SortStepType.MarkPivot, high));

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                _steps.Add(new SortStep(SortStepType.Compare, j, high));

                if (_arr[j] <= pivot)
                {
                    i++;
                    (_arr[i], _arr[j]) = (_arr[j], _arr[i]);
                    _steps.Add(new SortStep(SortStepType.Swap, i, j));
                }
            }

            // Place pivot
            (_arr[i + 1], _arr[high]) = (_arr[high], _arr[i + 1]);
            _steps.Add(new SortStep(SortStepType.Swap, i + 1, high));
            _steps.Add(new SortStep(SortStepType.SetSorted, i + 1));
            _steps.Add(new SortStep(SortStepType.ClearMarks, high));

            return i + 1;
        }
    }
}
