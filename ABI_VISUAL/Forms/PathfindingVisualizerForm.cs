using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ABI_VISUAL.Algorithms.Pathfinding;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Forms
{
    public enum PlacementMode { Wall, Start, End }

    public class PathfindingVisualizerForm : Form
    {
        private VisualizerSettings _settings;
        private bool[,]  _walls    = new bool[1, 1];
        private bool[,]  _visited  = new bool[1, 1];
        private bool[,]  _frontier = new bool[1, 1];
        private bool[,]  _path     = new bool[1, 1];

        private (int r, int c) _start = (-1, -1);
        private (int r, int c) _end   = (-1, -1);
        private List<PathStep> _steps = [];
        private int  _stepIndex   = 0;
        private bool _running     = false;
        private bool _done        = false;
        private bool _noPath      = false;
        private int  _visitedCount= 0;
        private int  _pathLength  = 0;

        private PlacementMode _mode      = PlacementMode.Wall;
        private bool          _mouseDown = false;

        private Panel   _gridPanel   = null!;
        private System.Windows.Forms.Timer _timer = new();
        private ComboBox _algoPicker = null!;
        private Button  _btnStart    = null!;
        private Button  _btnClear    = null!;
        private Button  _btnSettings = null!;
        private Button  _btnModeWall = null!;
        private Button  _btnModeStart= null!;
        private Button  _btnModeEnd  = null!;
        private Label   _lblVisited  = null!;
        private Label   _lblPathLen  = null!;
        private Label   _lblStatus   = null!;

        private static readonly string[] AlgoNames =
            ["BFS", "Dijkstra", "A* (A-Star)", "Greedy Best-First"];

        public PathfindingVisualizerForm(VisualizerSettings settings)
        {
            _settings = settings;
            InitializeComponents();
            ResetGrid();
        }

        private void InitializeComponents()
        {
            Theme.ApplyTo(this);
            Text          = "ABI VISUAL — Pathfinding Visualizer";
            Size          = new Size(1200, 750);
            MinimumSize   = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;

            _gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Panel };
            _gridPanel.Paint     += GridPanel_Paint;
            _gridPanel.MouseDown += GridPanel_MouseDown;
            _gridPanel.MouseMove += GridPanel_MouseMove;
            _gridPanel.MouseUp   += (s, e) => _mouseDown = false;

            var sidebar = new Panel { Dock = DockStyle.Right, Width = 235, BackColor = Theme.Surface };
            int y = 10;

            void Add(Control c) => sidebar.Controls.Add(c);

            Add(new Label { Text = "PATH", Font = new Font("Consolas", 22f, FontStyle.Bold),
                ForeColor = Theme.NeonPurple, Location = new Point(10, y), Size = new Size(215, 40) });
            y += 42;
            Add(new Label { Text = "VISUALIZER", Font = new Font("Consolas", 9f),
                ForeColor = Theme.TextSecondary, Location = new Point(12, y), AutoSize = true });
            y += 30;

            Add(new Label { Text = "Algorithm", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 8f), Location = new Point(10, y), AutoSize = true });
            y += 18;

            _algoPicker = new ComboBox {
                Location = new Point(10, y), Width = 210, DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Theme.SurfaceAlt, ForeColor = Theme.TextPrimary, Font = new Font("Consolas", 9f)
            };
            _algoPicker.Items.AddRange(AlgoNames);
            _algoPicker.SelectedIndex = 2;
            Add(_algoPicker); y += 36;

            Add(new Label { Text = "Placement Mode", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 8f), Location = new Point(10, y), AutoSize = true });
            y += 18;

            _btnModeWall  = Theme.MakeButton("WALL",  Theme.NeonOrange);
            _btnModeStart = Theme.MakeButton("START", Theme.NeonGreen);
            _btnModeEnd   = Theme.MakeButton("END",   Theme.NeonRed);

            _btnModeWall.Location  = new Point(10,  y); _btnModeWall.Width  = 63;
            _btnModeStart.Location = new Point(78,  y); _btnModeStart.Width = 63;
            _btnModeEnd.Location   = new Point(150, y); _btnModeEnd.Width   = 63;

            _btnModeWall.Click  += (s, e) => SetMode(PlacementMode.Wall);
            _btnModeStart.Click += (s, e) => SetMode(PlacementMode.Start);
            _btnModeEnd.Click   += (s, e) => SetMode(PlacementMode.End);

            Add(_btnModeWall); Add(_btnModeStart); Add(_btnModeEnd);
            y += 46;

            // How-to hint
            Add(new Label {
                Text = "Tip: Click grid to place tiles.\nDrag to draw walls.",
                ForeColor = Color.FromArgb(80, 80, 130),
                Font = new Font("Consolas", 7.5f),
                Location = new Point(10, y), Size = new Size(210, 34)
            }); y += 40;

            y = AddStatRow(sidebar, "Visited Nodes", out _lblVisited, y);
            y = AddStatRow(sidebar, "Path Length",   out _lblPathLen, y);
            y = AddStatRow(sidebar, "Status",        out _lblStatus,  y);
            y += 6;

            _btnStart = Theme.MakeButton("START", Theme.NeonGreen);
            _btnStart.Location = new Point(10, y); _btnStart.Width = 210;
            _btnStart.Click += BtnStart_Click;
            Add(_btnStart); y += 44;

            _btnClear = Theme.MakeButton("CLEAR GRID", Theme.NeonOrange);
            _btnClear.Location = new Point(10, y); _btnClear.Width = 210;
            _btnClear.Click += (s, e) => ClearGrid();
            Add(_btnClear); y += 44;

            _btnSettings = Theme.MakeButton("SETTINGS", Theme.NeonPurple);
            _btnSettings.Location = new Point(10, y); _btnSettings.Width = 210;
            _btnSettings.Click += BtnSettings_Click;
            Add(_btnSettings); y += 50;

            AddLegend(sidebar, y);

            Controls.Add(_gridPanel);
            Controls.Add(sidebar);

            _timer.Interval = _settings.PathSpeedMs;
            _timer.Tick    += Timer_Tick;

            SetMode(PlacementMode.Wall);
        }

        private int AddStatRow(Panel parent, string name, out Label valueLabel, int y)
        {
            parent.Controls.Add(new Label {
                Text = name + ":", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 7.5f), Location = new Point(10, y), AutoSize = true
            });
            valueLabel = new Label {
                Text = "0", ForeColor = Theme.NeonPurple,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Location = new Point(10, y + 14), AutoSize = true
            };
            parent.Controls.Add(valueLabel);
            return y + 36;
        }

        private void AddLegend(Panel parent, int startY)
        {
            var items = new (string, Color)[] {
                ("Unvisited", Theme.TileUnvisited),
                ("Wall",      Theme.TileWall),
                ("Start",     Theme.TileStart),
                ("End",       Theme.TileEnd),
                ("Frontier",  Theme.TileFrontier),
                ("Visited",   Theme.TileVisitedHL),
                ("Path",      Theme.TilePath),
            };
            int y = startY;
            parent.Controls.Add(new Label {
                Text = "LEGEND", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 7.5f, FontStyle.Bold),
                Location = new Point(10, y), AutoSize = true
            }); y += 18;
            foreach (var (name, color) in items)
            {
                parent.Controls.Add(new Panel {
                    Location = new Point(10, y + 2), Size = new Size(12, 12), BackColor = color
                });
                parent.Controls.Add(new Label {
                    Text = name, ForeColor = Theme.TextSecondary,
                    Font = new Font("Consolas", 7.5f), Location = new Point(28, y), AutoSize = true
                });
                y += 18;
            }
        }

        private void ResetGrid()
        {
            int r = _settings.GridRows, c = _settings.GridCols;
            _walls = new bool[r, c]; _visited = new bool[r, c];
            _frontier = new bool[r, c]; _path = new bool[r, c];
            _start = (-1, -1); _end = (-1, -1);
            _steps = []; _stepIndex = 0; _running = false;
            _done = false; _noPath = false;
            _visitedCount = 0; _pathLength = 0;
            ResetButtonStates(); UpdateStats(); _gridPanel.Invalidate();
        }

        private void ClearGrid() { _timer.Stop(); ResetGrid(); }

        private void ClearVisualization()
        {
            int r = _settings.GridRows, c = _settings.GridCols;
            _visited = new bool[r, c]; _frontier = new bool[r, c]; _path = new bool[r, c];
            _steps = []; _stepIndex = 0; _running = false; _done = false; _noPath = false;
            _visitedCount = 0; _pathLength = 0;
        }

        private void ResetButtonStates()
        {
            _btnStart.Text      = "START";
            _btnStart.ForeColor = Theme.NeonGreen;
            _btnStart.Enabled   = true;
            _algoPicker.Enabled = true;
            _btnModeWall.Enabled = _btnModeStart.Enabled = _btnModeEnd.Enabled = true;
        }

        private void SetMode(PlacementMode m)
        {
            _mode = m;
            _btnModeWall.FlatAppearance.BorderSize  = m == PlacementMode.Wall  ? 2 : 1;
            _btnModeStart.FlatAppearance.BorderSize = m == PlacementMode.Start ? 2 : 1;
            _btnModeEnd.FlatAppearance.BorderSize   = m == PlacementMode.End   ? 2 : 1;
        }

        private (int r, int c) PixelToCell(int px, int py)
        {
            if (_settings.GridRows == 0 || _settings.GridCols == 0) return (-1, -1);
            float cw = (float)_gridPanel.Width  / _settings.GridCols;
            float ch = (float)_gridPanel.Height / _settings.GridRows;
            int c = (int)(px / cw), r = (int)(py / ch);
            if (r < 0 || r >= _settings.GridRows || c < 0 || c >= _settings.GridCols) return (-1, -1);
            return (r, c);
        }

        private void GridPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_running || _done) return;
            _mouseDown = true;
            HandleCellClick(e.X, e.Y);
        }

        private void GridPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_mouseDown || _running || _done) return;
            if (_mode == PlacementMode.Wall) HandleCellClick(e.X, e.Y);
        }

        private void HandleCellClick(int px, int py)
        {
            var (r, c) = PixelToCell(px, py);
            if (r < 0) return;
            switch (_mode)
            {
                case PlacementMode.Wall:
                    if ((r, c) != _start && (r, c) != _end)
                    { _walls[r, c] = !_walls[r, c]; ClearVisualization(); _gridPanel.Invalidate(); }
                    break;
                case PlacementMode.Start:
                    if (!_walls[r, c] && (r, c) != _end)
                    { _start = (r, c); ClearVisualization(); _gridPanel.Invalidate(); }
                    break;
                case PlacementMode.End:
                    if (!_walls[r, c] && (r, c) != _start)
                    { _end = (r, c); ClearVisualization(); _gridPanel.Invalidate(); }
                    break;
            }
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (_done) return;
            if (!_running)
            {
                if (_start == (-1, -1) || _end == (-1, -1))
                {
                    MessageBox.Show(
                        "Please place both a START tile and an END tile before running.\n\n" +
                        "1. Click the green START button in Placement Mode\n" +
                        "2. Click any empty cell on the grid to place it\n" +
                        "3. Click the red END button and place it on another cell\n" +
                        "4. Then click START to run the algorithm",
                        "Missing Start/End Tile",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_steps.Count == 0) { ClearVisualization(); _steps = BuildSteps(); }
                _running = true;
                _timer.Interval = Math.Max(1, _settings.PathSpeedMs);
                _timer.Start();
                _btnStart.Text = "PAUSE"; _btnStart.ForeColor = Theme.NeonYellow;
                _algoPicker.Enabled = false;
                _btnModeWall.Enabled = _btnModeStart.Enabled = _btnModeEnd.Enabled = false;
            }
            else
            {
                _timer.Stop(); _running = false;
                _btnStart.Text = "RESUME"; _btnStart.ForeColor = Theme.NeonGreen;
            }
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            bool was = _running;
            if (_running) { _timer.Stop(); _running = false; }
            using var dlg = new SettingsForm(_settings, SettingsTarget.Pathfinding);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                bool sizeChanged = _settings.GridRows != dlg.Settings.GridRows ||
                                   _settings.GridCols != dlg.Settings.GridCols;
                _settings = dlg.Settings;
                _timer.Interval = _settings.PathSpeedMs;
                if (sizeChanged) { ClearGrid(); return; }
            }
            if (was) { _running = true; _timer.Start(); }
        }

        private List<PathStep> BuildSteps() =>
            _algoPicker.SelectedIndex switch {
                0 => BFS.GenerateSteps(_walls, _start, _end),
                1 => Dijkstra.GenerateSteps(_walls, _start, _end),
                2 => AStar.GenerateSteps(_walls, _start, _end),
                3 => GreedyBFS.GenerateSteps(_walls, _start, _end),
                _ => AStar.GenerateSteps(_walls, _start, _end),
            };

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_stepIndex >= _steps.Count)
            {
                _timer.Stop(); _running = false; _done = true;
                _btnStart.Text     = _noPath ? "NO PATH" : "DONE";
                _btnStart.ForeColor= _noPath ? Theme.NeonRed : Theme.NeonGreen;
                _btnStart.Enabled  = false;
                UpdateStats(); _gridPanel.Invalidate(); return;
            }
            ApplyStep(_steps[_stepIndex]); _stepIndex++;
            UpdateStats(); _gridPanel.Invalidate();
        }

        private void ApplyStep(PathStep step)
        {
            var (r, c) = step.Cell;
            switch (step.Type)
            {
                case PathStepType.Visit:
                    if (!_visited[r, c]) { _visited[r, c] = true; _visitedCount++; }
                    _frontier[r, c] = false; break;
                case PathStepType.Frontier:
                    if (!_visited[r, c]) _frontier[r, c] = true; break;
                case PathStepType.PathTrace:
                    if (step.Path != null) {
                        foreach (var (pr, pc) in step.Path) _path[pr, pc] = true;
                        _pathLength = step.Path.Count;
                    }
                    break;
                case PathStepType.NoPath:
                    _noPath = true; break;
            }
        }

        private void GridPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Theme.Panel);

            int rows = _settings.GridRows, cols = _settings.GridCols;
            float cw = (float)_gridPanel.Width / cols;
            float ch = (float)_gridPanel.Height / rows;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float x = c * cw, y = r * ch;
                    Color fill = Theme.TileUnvisited;

                    if      (_walls[r, c])    fill = Theme.TileWall;
                    else if ((r,c) == _start) fill = Theme.TileStart;
                    else if ((r,c) == _end)   fill = Theme.TileEnd;
                    else if (_path[r, c])     fill = Theme.TilePath;
                    else if (_frontier[r, c]) fill = Theme.TileFrontier;
                    else if (_visited[r, c])  fill = Theme.TileVisitedHL;

                    using var brush = new SolidBrush(fill);
                    g.FillRectangle(brush, x + 0.5f, y + 0.5f, cw - 1f, ch - 1f);
                }
            }

            if (cw >= 6 && ch >= 6)
            {
                using var gridPen = new Pen(Color.FromArgb(30, 30, 60), 0.5f);
                for (int r = 0; r <= rows; r++)
                    g.DrawLine(gridPen, 0, r * ch, _gridPanel.Width, r * ch);
                for (int c = 0; c <= cols; c++)
                    g.DrawLine(gridPen, c * cw, 0, c * cw, _gridPanel.Height);
            }

            DrawMarker(g, _start, cw, ch, Theme.TileStart, "S");
            DrawMarker(g, _end,   cw, ch, Theme.TileEnd,   "E");
        }

        private void DrawMarker(Graphics g, (int r, int c) cell, float cw, float ch, Color color, string letter)
        {
            if (cell == (-1, -1)) return;
            float x = cell.c * cw + cw / 2, y = cell.r * ch + ch / 2;
            float sz = Math.Min(cw, ch) * 0.6f;
            using var pen = new Pen(color, 1.5f);
            g.DrawEllipse(pen, x - sz / 2, y - sz / 2, sz, sz);
            if (sz > 8)
            {
                using var lbrush = new SolidBrush(Color.White);
                using var font   = new Font("Consolas", Math.Max(6, sz * 0.45f), FontStyle.Bold);
                var sf = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(letter, font, lbrush, x, y, sf);
            }
        }

        private void UpdateStats()
        {
            _lblVisited.Text = _visitedCount.ToString();
            _lblPathLen.Text = _pathLength > 0 ? _pathLength.ToString() : "—";
            _lblStatus.Text  = _noPath  ? "NO PATH" : _done ? "DONE" :
                               _running ? "RUNNING" : "READY";
            _lblStatus.ForeColor = _noPath  ? Theme.NeonRed  :
                                   _done    ? Theme.NeonGreen :
                                   _running ? Theme.NeonYellow : Theme.NeonPurple;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        { _timer.Stop(); base.OnFormClosed(e); }
    }
}
