using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Sorting
{
    /// <summary>
    /// Merge Sort – faithfully animates divide and merge phases.
    /// Uses auxiliary array writes (Overwrite steps) so the animation
    /// reflects real merge-sort behaviour, not swaps.
    /// </summary>
    public class MergeSort
    {
        private static int[] _arr = [];
        private static List<SortStep> _steps = [];

        public static List<SortStep> GenerateSteps(int[] input)
        {
            _arr   = (int[])input.Clone();
            _steps = new List<SortStep>();
            MergeSortRecursive(0, _arr.Length - 1);

            // Mark all sorted at end
            for (int i = 0; i < _arr.Length; i++)
                _steps.Add(new SortStep(SortStepType.SetSorted, i));

            return _steps;
        }

        private static void MergeSortRecursive(int left, int right)
        {
            if (left >= right) return;

            int mid = (left + right) / 2;

            // Mark left and right halves for visual divide
            for (int i = left; i <= mid; i++)
                _steps.Add(new SortStep(SortStepType.MarkLeft, i));
            for (int i = mid + 1; i <= right; i++)
                _steps.Add(new SortStep(SortStepType.MarkRight, i));

            MergeSortRecursive(left, mid);
            MergeSortRecursive(mid + 1, right);
            Merge(left, mid, right);
        }

        private static void Merge(int left, int mid, int right)
        {
            int[] temp = new int[right - left + 1];
            int i = left, j = mid + 1, k = 0;

            while (i <= mid && j <= right)
            {
                _steps.Add(new SortStep(SortStepType.Compare, i, j));

                if (_arr[i] <= _arr[j])
                    temp[k++] = _arr[i++];
                else
                    temp[k++] = _arr[j++];
            }

            while (i <= mid)  temp[k++] = _arr[i++];
            while (j <= right) temp[k++] = _arr[j++];

            // Write merged result back, emitting Overwrite steps
            for (int x = 0; x < temp.Length; x++)
            {
                _arr[left + x] = temp[x];
                _steps.Add(new SortStep(SortStepType.Overwrite, left + x, -1, temp[x], (int[])_arr.Clone()));
            }
        }
    }
}
