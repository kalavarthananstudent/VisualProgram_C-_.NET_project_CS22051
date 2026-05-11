using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ABI_VISUAL.Models;

namespace ABI_VISUAL.Forms
{
    public class MainForm : Form
    {
        private VisualizerSettings _settings = new();
        private System.Windows.Forms.Timer _glowTimer = new();
        private float _glowPhase = 0f;
        private Panel _headerPanel = null!;

        public MainForm()
        {
            InitializeComponents();
            _glowTimer.Interval = 50;
            _glowTimer.Tick    += (s, e) => { _glowPhase += 0.08f; _headerPanel.Invalidate(); };
            _glowTimer.Start();
        }

        private void InitializeComponents()
        {
            Theme.ApplyTo(this);
            Text            = "ABI VISUAL — Algorithm Visualizer";
            Size            = new Size(860, 600);
            MinimumSize     = new Size(700, 500);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;

            // ── Header ────────────────────────────────────────────────────────────
            _headerPanel = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Color.Transparent };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text      = "ABI VISUAL",
                Font      = new Font("Consolas", 34f, FontStyle.Bold),
                ForeColor = Theme.NeonCyan,
                AutoSize  = true,
                BackColor = Color.Transparent,
            };
            titleLabel.Location = new Point(40, 30);

            var subtitleLabel = new Label
            {
                Text      = "Algorithm Visualizer",
                Font      = new Font("Consolas", 12f),
                ForeColor = Theme.TextSecondary,
                AutoSize  = true,
                BackColor = Color.Transparent,
            };
            subtitleLabel.Location = new Point(44, 95);

            var versionLabel = new Label
            {
                Text      = "Sorting  ·  Pathfinding  ·  Interactive Animations",
                Font      = new Font("Consolas", 9f),
                ForeColor = Color.FromArgb(80, 80, 140),
                AutoSize  = true,
                BackColor = Color.Transparent,
            };
            versionLabel.Location = new Point(44, 125);

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);
            _headerPanel.Controls.Add(versionLabel);

            // ── Cards ─────────────────────────────────────────────────────────────
            var cardPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding   = new Padding(40, 10, 40, 20),
            };

            var sortCard = MakeCard(
                "SORTING VISUALIZER",
                "Insertion Sort  ·  Merge Sort  ·  Quick Sort\nShell Sort  ·  Heap Sort\n\nStep-by-step animated bar chart",
                Theme.NeonCyan,
                (s, e) => new SortingVisualizerForm(_settings).Show());

            var pathCard = MakeCard(
                "PATHFINDING VISUALIZER",
                "BFS  ·  Dijkstra  ·  A*  ·  Greedy Best-First\nInteractive grid with wall drawing\n\nAnimated node expansion & path trace",
                Theme.NeonPurple,
                (s, e) => new PathfindingVisualizerForm(_settings).Show());

            sortCard.Location = new Point(40, 20);
            pathCard.Location = new Point(420, 20);

            cardPanel.Controls.Add(sortCard);
            cardPanel.Controls.Add(pathCard);

            Controls.Add(cardPanel);
            Controls.Add(_headerPanel);
        }

        private Panel MakeCard(string title, string desc, Color accent, EventHandler onClick)
        {
            var card = new Panel
            {
                Size      = new Size(340, 280),
                BackColor = Theme.Surface,
                Cursor    = Cursors.Hand,
            };

            card.Paint += (s, e) =>
            {
                var g   = e.Graphics;
                using var pen = new Pen(accent, 1.5f);
                g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                using var brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(card.Width, 0),
                    Color.FromArgb(0, accent), Color.FromArgb(80, accent));
                g.FillRectangle(brush, 0, 0, card.Width, 3);
            };

            var lTitle = new Label
            {
                Text      = title,
                Font      = new Font("Consolas", 11f, FontStyle.Bold),
                ForeColor = accent,
                Location  = new Point(20, 28),
                Size      = new Size(300, 28),
            };

            var lDesc = new Label
            {
                Text      = desc,
                Font      = new Font("Consolas", 8.5f),
                ForeColor = Theme.TextSecondary,
                Location  = new Point(20, 66),
                Size      = new Size(300, 90),
            };

            var btnLaunch = Theme.MakeButton("LAUNCH", accent);
            btnLaunch.Size     = new Size(150, 38);
            btnLaunch.Location = new Point(20, 210);
            btnLaunch.Click   += onClick;

            card.Click  += onClick;
            lTitle.Click += onClick;

            card.Controls.Add(lTitle);
            card.Controls.Add(lDesc);
            card.Controls.Add(btnLaunch);
            return card;
        }

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            float alpha = (float)(Math.Sin(_glowPhase) * 0.5 + 0.5) * 55 + 10;
            using var brush = new LinearGradientBrush(
                new Point(0, 0), new Point(_headerPanel.Width, _headerPanel.Height),
                Color.FromArgb((int)alpha, Theme.NeonCyan),
                Color.FromArgb((int)(alpha * 0.3f), Theme.NeonPurple));
            g.FillRectangle(brush, 0, 0, _headerPanel.Width, _headerPanel.Height);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        { _glowTimer.Stop(); base.OnFormClosed(e); }
    }
}
