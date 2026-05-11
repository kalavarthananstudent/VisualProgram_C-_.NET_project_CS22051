using System;
using System.Drawing;
using System.Windows.Forms;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Forms
{
    public enum SettingsTarget { Sorting, Pathfinding }

    public class SettingsForm : Form
    {
        public VisualizerSettings Settings { get; private set; }
        private readonly SettingsTarget _target;

        public SettingsForm(VisualizerSettings current, SettingsTarget target)
        {
            Settings = current.Clone();
            _target  = target;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Theme.ApplyTo(this);
            Text            = _target == SettingsTarget.Sorting
                              ? "Settings — Sorting Visualizer"
                              : "Settings — Pathfinding Visualizer";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            Width           = 420;

            int y = 20;

            Controls.Add(new Label {
                Text = "SETTINGS", Font = new Font("Consolas", 14f, FontStyle.Bold),
                ForeColor = Theme.NeonCyan, Location = new Point(20, y), AutoSize = true
            });
            y += 44;

            if (_target == SettingsTarget.Sorting)
            {
                AddSlider("Array Size", 10, 200, Settings.ArraySize, y,
                    v => Settings.ArraySize = v);
                y += 72;
                AddSlider("Animation Speed — lower value = faster", 5, 300, Settings.SortSpeedMs, y,
                    v => Settings.SortSpeedMs = v, " ms");
                y += 72;
            }
            else
            {
                AddSlider("Grid Rows", 10, 50, Settings.GridRows, y,
                    v => Settings.GridRows = v);
                y += 72;
                AddSlider("Grid Columns", 10, 80, Settings.GridCols, y,
                    v => Settings.GridCols = v);
                y += 72;
                AddSlider("Animation Speed — lower value = faster", 5, 300, Settings.PathSpeedMs, y,
                    v => Settings.PathSpeedMs = v, " ms");
                y += 72;
            }

            y += 10;

            var btnOk = Theme.MakeButton("APPLY", Theme.NeonGreen);
            btnOk.Location = new Point(20, y); btnOk.Width = 160;
            btnOk.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            Controls.Add(btnOk);

            var btnCancel = Theme.MakeButton("CANCEL", Theme.NeonRed);
            btnCancel.Location = new Point(200, y); btnCancel.Width = 160;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            Height = y + 80;
        }

        private void AddSlider(string name, int min, int max, int initial,
            int startY, Action<int> onChange, string suffix = "")
        {
            Controls.Add(new Label {
                Text = name, ForeColor = Theme.TextSecondary,
                Font = new Font("Consolas", 8.5f),
                Location = new Point(20, startY), AutoSize = true
            });

            var valLabel = new Label {
                Text = initial.ToString() + suffix,
                ForeColor = Theme.NeonCyan,
                Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                Location = new Point(310, startY), Size = new Size(80, 18),
                TextAlign = ContentAlignment.MiddleRight
            };
            Controls.Add(valLabel);

            var bar = new TrackBar {
                Minimum = min, Maximum = max, Value = initial,
                Location = new Point(20, startY + 22),
                Width = 370, TickStyle = TickStyle.None,
                BackColor = Theme.Surface
            };

            string localSuffix   = suffix;
            Label  localValLabel = valLabel;
            Action<int> localOnChange = onChange;

            bar.Scroll += (s, e) => {
                int v = bar.Value;
                localValLabel.Text = v.ToString() + localSuffix;
                localOnChange(v);
            };
            Controls.Add(bar);
        }
    }
}
