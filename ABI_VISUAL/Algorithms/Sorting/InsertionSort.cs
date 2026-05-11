using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Sorting
{
    /// <summary>
    /// Insertion Sort – produces a flat list of SortStep objects representing
    /// each atomic comparison and shift/overwrite. O(n²) time.
    /// </summary>
    public class InsertionSort
    {
        public static List<SortStep> GenerateSteps(int[] input)
        {
            var steps = new List<SortStep>();
            var arr   = (int[])input.Clone();
            int n     = arr.Length;

            for (int i = 1; i < n; i++)
            {
                int key = arr[i];
                int j   = i - 1;

                // Mark key element
                steps.Add(new SortStep(SortStepType.MarkPivot, i));

                while (j >= 0)
                {
                    steps.Add(new SortStep(SortStepType.Compare, j, i));

                    if (arr[j] > key)
                    {
                        arr[j + 1] = arr[j];
                        steps.Add(new SortStep(SortStepType.Overwrite, j + 1, -1, arr[j], (int[])arr.Clone()));
                        j--;
                    }
                    else break;
                }

                arr[j + 1] = key;
                steps.Add(new SortStep(SortStepType.Overwrite, j + 1, -1, key, (int[])arr.Clone()));
                steps.Add(new SortStep(SortStepType.ClearMarks, i));
            }

            // Mark everything sorted
            for (int i = 0; i < n; i++)
                steps.Add(new SortStep(SortStepType.SetSorted, i));

            return steps;
        }
    }
}
