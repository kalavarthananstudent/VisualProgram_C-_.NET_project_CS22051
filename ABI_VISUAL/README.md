# AlgoViz — Algorithm Visualizer
### CSCI 22042 Visual Programming · University of Kelaniya · 2024/2025 Semester II

A multi-form WinForms desktop application that visually animates sorting and pathfinding algorithms step-by-step using GDI+ and System.Windows.Forms.Timer.

---

## Requirements

| Tool | Version |
|------|---------|
| Visual Studio | 2022 (any edition) |
| .NET SDK | 8.0 or later |
| OS | Windows 10 / 11 |

---

## Setup & Run

1. **Clone / unzip** the repository into a local folder.
2. Open **`AlgoViz.sln`** in Visual Studio 2022.
3. In the Solution Explorer, right-click the project → **Set as Startup Project**.
4. Press **F5** (Debug) or **Ctrl+F5** (Run without debugging).

Alternatively, via the command line:
```bash
cd AlgoViz
dotnet run
```

---

## Project Structure

```
AlgoViz/
├── AlgoViz.csproj
├── Program.cs                  # Entry point
├── Theme.cs                    # Centralized dark neon colour palette + UI helpers
├── Models/
│   ├── SortStep.cs             # Atomic sorting step model
│   ├── PathStep.cs             # Atomic pathfinding step model
│   └── VisualizerSettings.cs   # Shared settings (array size, speed, grid size)
├── Algorithms/
│   ├── Sorting/
│   │   ├── InsertionSort.cs
│   │   ├── MergeSort.cs        # Divide & merge phases animated separately
│   │   ├── QuickSort.cs        # Lomuto partition, pivot highlighted
│   │   ├── ShellSort.cs        # Knuth gap sequence
│   │   └── HeapSort.cs         # Heapify + extraction phases
│   └── Pathfinding/
│       ├── BFS.cs
│       ├── Dijkstra.cs
│       ├── AStar.cs            # Manhattan heuristic
│       └── GreedyBFS.cs
└── Forms/
    ├── MainForm.cs             # Launch screen / navigation
    ├── SortingVisualizerForm.cs
    ├── PathfindingVisualizerForm.cs
    └── SettingsForm.cs         # Modal settings dialog (opened from both visualizers)
```

---

## Features

### Sorting Visualizer
- **Algorithms:** Insertion Sort, Merge Sort, Quick Sort, Shell Sort, Heap Sort
- Bar chart drawn on a `Panel` via GDI+ (`Paint` event)
- Color-coded bar states: Default · Comparing · Swap/Write · Pivot · Sorted
- Live comparison counter
- Generate new random array at any time
- Start / Pause / Resume / Reset controls
- Array size and animation speed configurable via Settings

### Pathfinding Visualizer
- **Algorithms:** BFS, Dijkstra, A* (Manhattan heuristic), Greedy Best-First Search
- Interactive grid drawn on a `Panel` via GDI+
- Click/drag to place: Walls · Start tile · End tile
- Animated traversal with Frontier (purple) and Visited (blue) states
- Final path highlighted in gold
- Error message if Start or End tile is missing
- Grid size and animation speed configurable via Settings

### Technical Highlights
- All rendering: `System.Drawing` / GDI+ via `Panel.Paint`
- All animation: `System.Windows.Forms.Timer` (no `Thread.Sleep`, no busy-wait)
- Each algorithm encapsulated in its own class (OOP requirement)
- All user inputs validated; no unhandled exceptions under normal use
- Timer stops cleanly on form close or reset

---

## External References

- Microsoft Docs – [System.Windows.Forms](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms)
- Microsoft Docs – [System.Drawing (GDI+)](https://learn.microsoft.com/en-us/dotnet/api/system.drawing)
- Algorithm pseudocode referenced from: Introduction to Algorithms (CLRS), 4th Edition
- A* heuristic: Manhattan distance (standard for 4-directional grids)

*All code was written from scratch. No third-party charting or visualization libraries were used.*
