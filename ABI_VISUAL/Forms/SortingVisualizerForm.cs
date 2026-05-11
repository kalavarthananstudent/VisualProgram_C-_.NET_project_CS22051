using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ABI_VISUAL.Algorithms.Sorting;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Forms
{
    public class SortingVisualizerForm : Form
    {
        private VisualizerSettings _settings;
        private int[]          _array       = [];
        private List<SortStep> _steps       = [];
        private int            _stepIndex   = 0;
        private int            _comparisons = 0;
        private bool           _running     = false;
        private bool           _done        = false;

        private int    _hiA = -1, _hiB = -1, _pivot = -1;
        private bool[] _sorted   = [];
        private int[]  _mergeArr = [];

        private Panel    _vizPanel    = null!;
        private System.Windows.Forms.Timer _timer = new();
        private ComboBox _algoPicker  = null!;
        private Button   _btnStart    = null!;
        private Button   _btnReset    = null!;
        private Button   _btnGenerate = null!;
        private Button   _btnSettings = null!;

        private Label       _lblAlgoVal    = null!;
        private Label       _lblCompareVal = null!;
        private Label       _lblStepVal    = null!;
        private Label       _lblStatusVal  = null!;
        private ProgressBar _progress      = null!;

        private static readonly string[] AlgoNames =
            ["Insertion Sort", "Merge Sort", "Quick Sort", "Shell Sort", "Heap Sort"];

        public SortingVisualizerForm(VisualizerSettings settings)
        {
            _settings = settings;
            InitializeComponents();
            GenerateArray();
        }

        private void InitializeComponents()
        {
            Theme.ApplyTo(this);
            Text          = "ABI VISUAL — Sorting Visualizer";
            Size          = new Size(1100, 700);
            MinimumSize   = new Size(800, 550);
            StartPosition = FormStartPosition.CenterScreen;

            _vizPanel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Panel };
            _vizPanel.Paint += VizPanel_Paint;

            var sidebar = new Panel { Dock = DockStyle.Right, Width = 230, BackColor = Theme.Surface };
            int y = 10;

            sidebar.Controls.Add(new Label {
                Text = "SORT", Font = new Font("Consolas", 22f, FontStyle.Bold),
                ForeColor = Theme.NeonCyan, Location = new Point(10, y), Size = new Size(210, 40)
            }); y += 42;

            sidebar.Controls.Add(new Label {
                Text = "VISUALIZER", Font = new Font("Consolas", 9f),
                ForeColor = Theme.TextSecondary, Location = new Point(12, y), AutoSize = true
            }); y += 30;

            sidebar.Controls.Add(new Label {
                Text = "Algorithm", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 8f), Location = new Point(10, y), AutoSize = true
            }); y += 18;

            _algoPicker = new ComboBox {
                Location = new Point(10, y), Width = 205,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Theme.SurfaceAlt, ForeColor = Theme.TextPrimary,
                Font = new Font("Consolas", 9f)
            };
            _algoPicker.Items.AddRange(AlgoNames);
            _algoPicker.SelectedIndex = 0;
            sidebar.Controls.Add(_algoPicker); y += 36;

            y = AddStatRow(sidebar, "Algorithm",   out _lblAlgoVal,    y);
            y = AddStatRow(sidebar, "Comparisons", out _lblCompareVal,  y);
            y = AddStatRow(sidebar, "Step",        out _lblStepVal,     y);
            y = AddStatRow(sidebar, "Status",      out _lblStatusVal,   y);
            y += 4;

            _progress = new ProgressBar {
                Location = new Point(10, y), Width = 205, Height = 8,
                Style = ProgressBarStyle.Continuous, Minimum = 0, Maximum = 100, Value = 0
            };
            sidebar.Controls.Add(_progress); y += 24;

            _btnGenerate = Theme.MakeButton("NEW ARRAY", Theme.NeonCyan);
            _btnGenerate.Location = new Point(10, y); _btnGenerate.Width = 205;
            _btnGenerate.Click += (s, e) => { StopAndReset(); GenerateArray(); };
            sidebar.Controls.Add(_btnGenerate); y += 44;

            _btnStart = Theme.MakeButton("START", Theme.NeonGreen);
            _btnStart.Location = new Point(10, y); _btnStart.Width = 205;
            _btnStart.Click += BtnStart_Click;
            sidebar.Controls.Add(_btnStart); y += 44;

            _btnReset = Theme.MakeButton("RESET", Theme.NeonOrange);
            _btnReset.Location = new Point(10, y); _btnReset.Width = 205;
            _btnReset.Click += (s, e) => StopAndReset();
            _btnReset.Enabled = false;
            sidebar.Controls.Add(_btnReset); y += 44;

            _btnSettings = Theme.MakeButton("SETTINGS", Theme.NeonPurple);
            _btnSettings.Location = new Point(10, y); _btnSettings.Width = 205;
            _btnSettings.Click += BtnSettings_Click;
            sidebar.Controls.Add(_btnSettings); y += 50;

            AddLegend(sidebar, y);

            Controls.Add(_vizPanel);
            Controls.Add(sidebar);

            _timer.Interval = _settings.SortSpeedMs;
            _timer.Tick    += Timer_Tick;
        }

        private int AddStatRow(Panel parent, string name, out Label valueLabel, int y)
        {
            parent.Controls.Add(new Label {
                Text = name + ":", ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 7.5f), Location = new Point(10, y), AutoSize = true
            });
            valueLabel = new Label {
                Text = "—", ForeColor = Theme.NeonCyan,
                Font = new Font("Consolas", 9f, FontStyle.Bold),
                Location = new Point(10, y + 14), AutoSize = true
            };
            parent.Controls.Add(valueLabel);
            return y + 36;
        }

        private void AddLegend(Panel parent, int startY)
        {
            var items = new (string, Color)[] {
                ("Default",    Theme.BarDefault),
                ("Comparing",  Theme.BarCompare),
                ("Swap/Write", Theme.BarSwap),
                ("Pivot",      Theme.BarPivot),
                ("Sorted",     Theme.BarSorted),
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
                    Location = new Point(10, y + 3), Size = new Size(10, 10), BackColor = color
                });
                parent.Controls.Add(new Label {
                    Text = name, ForeColor = Theme.TextSecondary,
                    Font = new Font("Consolas", 7.5f), Location = new Point(26, y), AutoSize = true
                });
                y += 18;
            }
        }

        private void GenerateArray()
        {
            var rng = new Random();
            _array    = Enumerable.Range(0, _settings.ArraySize)
                                  .Select(_ => rng.Next(5, 100)).ToArray();
            _mergeArr = (int[])_array.Clone();
            _sorted   = new bool[_array.Length];
            _hiA = _hiB = _pivot = -1;
            _comparisons = 0; _steps = []; _stepIndex = 0; _done = false;
            UpdateStats(); _vizPanel.Invalidate();
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (_done) return;
            if (!_running)
            {
                if (_steps.Count == 0)
                {
                    _steps    = BuildSteps();
                    _mergeArr = (int[])_array.Clone();
                    _sorted   = new bool[_array.Length];
                    _stepIndex = 0; _comparisons = 0;
                    _progress.Maximum = _steps.Count;
                }
                _running = true;
                _timer.Interval = Math.Max(1, _settings.SortSpeedMs);
                _timer.Start();
                _btnStart.Text = "PAUSE"; _btnStart.ForeColor = Theme.NeonYellow;
                _btnReset.Enabled = true; _btnGenerate.Enabled = false; _algoPicker.Enabled = false;
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
            using var dlg = new SettingsForm(_settings, SettingsTarget.Sorting);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _settings = dlg.Settings;
                _timer.Interval = _settings.SortSpeedMs;
                if (_settings.ArraySize != _array.Length) { StopAndReset(); GenerateArray(); return; }
            }
            if (was) { _running = true; _timer.Start(); }
        }

        private void StopAndReset()
        {
            _timer.Stop(); _running = false;
            _steps = []; _stepIndex = 0; _comparisons = 0; _done = false;
            _hiA = _hiB = _pivot = -1;
            _mergeArr = (int[])_array.Clone(); _sorted = new bool[_array.Length];
            _btnStart.Text = "START"; _btnStart.ForeColor = Theme.NeonGreen;
            _btnStart.Enabled = true; _btnReset.Enabled = false;
            _btnGenerate.Enabled = true; _algoPicker.Enabled = true;
            _progress.Value = 0; UpdateStats(); _vizPanel.Invalidate();
        }

        private List<SortStep> BuildSteps()
        {
            var input = (int[])_array.Clone();
            return _algoPicker.SelectedIndex switch {
                0 => InsertionSort.GenerateSteps(input),
                1 => MergeSort.GenerateSteps(input),
                2 => QuickSort.GenerateSteps(input),
                3 => ShellSort.GenerateSteps(input),
                4 => HeapSort.GenerateSteps(input),
                _ => InsertionSort.GenerateSteps(input),
            };
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_stepIndex >= _steps.Count)
            {
                _timer.Stop(); _running = false; _done = true;
                for (int i = 0; i < _sorted.Length; i++) _sorted[i] = true;
                _hiA = _hiB = _pivot = -1;
                _btnStart.Text = "DONE"; _btnStart.Enabled = false;
                _btnStart.ForeColor = Theme.NeonGreen;
                UpdateStats(); _vizPanel.Invalidate(); return;
            }
            ApplyStep(_steps[_stepIndex]); _stepIndex++;
            _progress.Value = Math.Min(_stepIndex, _progress.Maximum);
            UpdateStats(); _vizPanel.Invalidate();
        }

        private void ApplyStep(SortStep step)
        {
            _hiA = _hiB = _pivot = -1;
            switch (step.Type)
            {
                case SortStepType.Compare:
                    _hiA = step.IndexA; _hiB = step.IndexB; _comparisons++; break;
                case SortStepType.Swap:
                    _hiA = step.IndexA; _hiB = step.IndexB;
                    (_mergeArr[step.IndexA], _mergeArr[step.IndexB]) =
                        (_mergeArr[step.IndexB], _mergeArr[step.IndexA]);
                    _array[step.IndexA] = _mergeArr[step.IndexA];
                    _array[step.IndexB] = _mergeArr[step.IndexB]; break;
                case SortStepType.Overwrite:
                    _hiA = step.IndexA;
                    if (step.Array != null) {
                        Array.Copy(step.Array, _array, _array.Length);
                        Array.Copy(step.Array, _mergeArr, _mergeArr.Length);
                    } else {
                        _array[step.IndexA] = step.Value;
                        _mergeArr[step.IndexA] = step.Value;
                    }
                    break;
                case SortStepType.SetSorted:
                    if (step.IndexA >= 0 && step.IndexA < _sorted.Length)
                        _sorted[step.IndexA] = true; break;
                case SortStepType.MarkPivot:  _pivot = step.IndexA; break;
                case SortStepType.MarkLeft:   _hiA   = step.IndexA; break;
                case SortStepType.MarkRight:  _hiB   = step.IndexA; break;
                case SortStepType.ClearMarks: _hiA = _hiB = _pivot = -1; break;
            }
        }

        private void VizPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Theme.Panel);
            if (_array.Length == 0) return;

            int w = _vizPanel.Width, h = _vizPanel.Height;
            int padding = 10, n = _array.Length;
            float barW = (float)(w - padding * 2) / n;
            int   maxV = _array.Max(), maxH = h - 60;

            for (int i = 0; i < n; i++)
            {
                float bh     = (float)_array[i] / maxV * maxH;
                float x      = padding + i * barW;
                float y      = h - bh - 30;
                float bwDraw = Math.Max(barW - 1.5f, 1f);

                Color c = Theme.BarDefault;
                if      (_sorted[i])  c = Theme.BarSorted;
                else if (i == _pivot) c = Theme.BarPivot;
                else if (i == _hiA)   c = Theme.BarCompare;
                else if (i == _hiB)   c = Theme.BarSwap;

                using var brush = new LinearGradientBrush(
                    new PointF(x, y), new PointF(x, y + bh),
                    Color.FromArgb(255, c), Color.FromArgb(100, c));
                g.FillRectangle(brush, x, y, bwDraw, bh);

                if (i == _hiA || i == _hiB || i == _pivot)
                {
                    using var glowPen = new Pen(Color.FromArgb(120, c), 1f);
                    g.DrawRectangle(glowPen, x - 1, y - 1, bwDraw + 2, bh + 2);
                }
            }
            using var axisPen = new Pen(Theme.Border, 1f);
            g.DrawLine(axisPen, padding, h - 30, w - padding, h - 30);
        }

        private void UpdateStats()
        {
            _lblAlgoVal.Text    = AlgoNames[_algoPicker.SelectedIndex];
            _lblCompareVal.Text = _comparisons.ToString("N0");
            _lblStepVal.Text    = $"{_stepIndex} / {_steps.Count}";
            _lblStatusVal.Text  = _done ? "SORTED" : _running ? "RUNNING" : "READY";
            _lblStatusVal.ForeColor = _done ? Theme.NeonGreen :
                                     _running ? Theme.NeonYellow : Theme.NeonCyan;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        { _timer.Stop(); base.OnFormClosed(e); }
    }
}
