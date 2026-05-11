# ABIVisualizer - Algorithm & Pathfinding Visualizer

ABIVisualizer is a modern interactive desktop application built with C# and .NET for visualizing pathfinding algorithms in real time.  
The application provides a grid-based visualization environment where users can place start/end nodes, create walls, and observe how different algorithms explore paths.

---

## Features

- Interactive grid-based pathfinding visualizer
- Real-time algorithm animation
- Multiple placement modes
- Start and End node selection
- Wall/Obstacle creation
- Visual representation of:
  - Visited nodes
  - Frontier nodes
  - Final shortest path
- Clean neon-inspired UI
- Settings panel support
- Grid clearing functionality

---

## Supported Algorithms

Currently implemented:

- A* (A-Star)

Planned algorithms:

- Dijkstra’s Algorithm
- Breadth First Search (BFS)
- Depth First Search (DFS)
- Greedy Best First Search

---

## Technologies Used

- C#
- .NET
- Windows Forms (WinForms)
- Object-Oriented Programming (OOP)

---

## Project Structure

```bash
ABIVisualizer/
│
├── Algorithms/        # Pathfinding algorithms
├── Forms/             # Application forms and UI
├── Models/            # Data models and grid structures
├── bin/               # Build files
├── obj/               # Temporary object files
├── Program.cs         # Application entry point
├── Theme.cs           # UI theme configuration
└── ABIVisualizer.csproj   # Project configuration
