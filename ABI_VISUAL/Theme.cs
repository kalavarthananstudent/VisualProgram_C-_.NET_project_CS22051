using System.Drawing;

namespace ABI_VISUAL
{
    public static class Theme
    {
        public static readonly Color Background   = Color.FromArgb(10,  10,  20);
        public static readonly Color Surface      = Color.FromArgb(18,  18,  35);
        public static readonly Color SurfaceAlt   = Color.FromArgb(25,  25,  50);
        public static readonly Color Panel        = Color.FromArgb(14,  14,  28);

        public static readonly Color NeonCyan     = Color.FromArgb(0,   255, 255);
        public static readonly Color NeonPurple   = Color.FromArgb(180, 0,   255);
        public static readonly Color NeonGreen    = Color.FromArgb(0,   255, 128);
        public static readonly Color NeonOrange   = Color.FromArgb(255, 140, 0);
        public static readonly Color NeonRed      = Color.FromArgb(255, 50,  80);
        public static readonly Color NeonYellow   = Color.FromArgb(255, 230, 0);
        public static readonly Color NeonPink     = Color.FromArgb(255, 0,   180);

        public static readonly Color BarDefault   = Color.FromArgb(0,   180, 220);
        public static readonly Color BarCompare   = Color.FromArgb(255, 200, 0);
        public static readonly Color BarSwap      = Color.FromArgb(255, 50,  80);
        public static readonly Color BarSorted    = Color.FromArgb(0,   255, 128);
        public static readonly Color BarPivot     = Color.FromArgb(180, 0,   255);
        public static readonly Color BarMergeLeft = Color.FromArgb(0,   200, 255);
        public static readonly Color BarMergeRight= Color.FromArgb(255, 100, 200);

        public static readonly Color TileUnvisited= Color.FromArgb(20,  20,  40);
        public static readonly Color TileWall     = Color.FromArgb(40,  40,  80);
        public static readonly Color TileStart    = Color.FromArgb(0,   255, 128);
        public static readonly Color TileEnd      = Color.FromArgb(255, 50,  80);
        public static readonly Color TileVisited  = Color.FromArgb(30,  60,  120);
        public static readonly Color TileVisitedHL= Color.FromArgb(0,   120, 220);
        public static readonly Color TileFrontier = Color.FromArgb(180, 0,   255);
        public static readonly Color TilePath     = Color.FromArgb(255, 230, 0);

        public static readonly Color TextPrimary  = Color.FromArgb(220, 220, 255);
        public static readonly Color TextSecondary= Color.FromArgb(120, 120, 180);
        public static readonly Color Border       = Color.FromArgb(40,  40,  80);
        public static readonly Color BorderGlow   = Color.FromArgb(0,   180, 220);

        public static Button MakeButton(string text, Color accent)
        {
            var btn = new Button
            {
                Text      = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(20, 20, 45),
                ForeColor = accent,
                Font      = new Font("Consolas", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Height    = 36,
            };
            btn.FlatAppearance.BorderColor = accent;
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(accent.R / 6, accent.G / 6, accent.B / 6);
            return btn;
        }

        public static void ApplyTo(Form form)
        {
            form.BackColor = Background;
            form.ForeColor = TextPrimary;
            form.Font      = new Font("Consolas", 9f);
        }
    }
}
