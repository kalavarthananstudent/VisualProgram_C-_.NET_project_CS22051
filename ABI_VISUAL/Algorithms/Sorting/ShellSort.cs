using System.Collections.Generic;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Algorithms.Sorting
{
    /// <summary>
    /// Shell Sort using Knuth gap sequence (3^k - 1)/2.
    /// Visualizes gap-phase comparisons and shifts clearly.
    /// </summary>
    public class ShellSort
    {
        public static List<SortStep> GenerateSteps(int[] input)
        {
            var steps = new List<SortStep>();
            var arr   = (int[])input.Clone();
            int n     = arr.Length;

            // Compute Knuth sequence
            int gap = 1;
            while (gap < n / 3) gap = gap * 3 + 1;

            while (gap >= 1)
            {
                for (int i = gap; i < n; i++)
                {
                    int temp = arr[i];
                    steps.Add(new SortStep(SortStepType.MarkPivot, i));
                    int j = i;

                    while (j >= gap)
                    {
                        steps.Add(new SortStep(SortStepType.Compare, j - gap, i));

                        if (arr[j - gap] > temp)
                        {
                            arr[j] = arr[j - gap];
                            steps.Add(new SortStep(SortStepType.Overwrite, j, -1, arr[j - gap], (int[])arr.Clone()));
                            j -= gap;
                        }
                        else break;
                    }

                    arr[j] = temp;
                    steps.Add(new SortStep(SortStepType.Overwrite, j, -1, temp, (int[])arr.Clone()));
                    steps.Add(new SortStep(SortStepType.ClearMarks, i));
                }

                gap /= 3;
            }

            for (int i = 0; i < n; i++)
                steps.Add(new SortStep(SortStepType.SetSorted, i));

            return steps;
        }
    }
}
