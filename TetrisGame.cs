using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GameHub
{


    // ── Double-buffered Panel ─────────────────────────────────────────────
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
        protected override void OnPaintBackground(PaintEventArgs e) { }
    }

    // ── Tetrominoes ───────────────────────────────────────────────────────
    public static class Pieces
    {
        public static readonly int[][,] Shapes = new int[][,]
        {
            new int[,] { {1,1,1,1} },
            new int[,] { {1,1},{1,1} },
            new int[,] { {0,1,0},{1,1,1} },
            new int[,] { {1,0},{1,0},{1,1} },
            new int[,] { {0,1},{0,1},{1,1} },
            new int[,] { {0,1,1},{1,1,0} },
            new int[,] { {1,1,0},{0,1,1} },
        };
        public static readonly Color[] Colors =
        {
            Color.FromArgb(0,230,230),
            Color.FromArgb(230,230,0),
            Color.FromArgb(150,0,230),
            Color.FromArgb(230,140,0),
            Color.FromArgb(30,80,220),
            Color.FromArgb(0,200,0),
            Color.FromArgb(220,30,30),
        };
    }

    // ── Piece ─────────────────────────────────────────────────────────────
    public class Piece
    {
        public int[,] Shape;
        public Color Color;
        public int X, Y;

        public Piece(int idx)
        {
            Shape = (int[,])Pieces.Shapes[idx].Clone();
            Color = Pieces.Colors[idx];
            X = 3; Y = 0;
        }

        public int[,] Rotated()
        {
            int R = Shape.GetLength(0), C = Shape.GetLength(1);
            var rot = new int[C, R];
            for (int r = 0; r < R; r++)
                for (int c = 0; c < C; c++)
                    rot[c, R - 1 - r] = Shape[r, c];
            return rot;
        }
    }

    // ── Board ─────────────────────────────────────────────────────────────
    public class Board
    {
        public const int Cols = 10, Rows = 20;
        public Color[,] Grid = new Color[Rows, Cols];
        public Piece Current, Next;
        public int Score, Lines, Level;
        public bool GameOver, Paused;

        private readonly Random rng = new Random();
        public event Action Redraw;

        public Board() { SpawnPair(); }

        public void Reset()
        {
            Grid = new Color[Rows, Cols];
            Score = Lines = Level = 0;
            GameOver = Paused = false;
            SpawnPair();
        }

        private void SpawnPair()
        {
            Current = Next ?? new Piece(rng.Next(7));
            Next = new Piece(rng.Next(7));
            if (!CanFit(Current, Current.X, Current.Y)) GameOver = true;
            Redraw?.Invoke();
        }

        public bool CanFit(Piece p, int nx, int ny, int[,] sh = null)
        {
            sh = sh ?? p.Shape;
            int R = sh.GetLength(0), C = sh.GetLength(1);
            for (int r = 0; r < R; r++)
                for (int c = 0; c < C; c++)
                    if (sh[r, c] != 0)
                    {
                        int gr = ny + r, gc = nx + c;
                        if (gr < 0 || gr >= Rows || gc < 0 || gc >= Cols) return false;
                        if (Grid[gr, gc] != Color.Empty) return false;
                    }
            return true;
        }

        public void MoveLeft()
        {
            if (!Paused && CanFit(Current, Current.X - 1, Current.Y))
            { Current.X--; Redraw?.Invoke(); }
        }
        public void MoveRight()
        {
            if (!Paused && CanFit(Current, Current.X + 1, Current.Y))
            { Current.X++; Redraw?.Invoke(); }
        }
        public void Rotate()
        {
            if (Paused) return;
            var rot = Current.Rotated();
            if (CanFit(Current, Current.X, Current.Y, rot)) { Current.Shape = rot; Redraw?.Invoke(); }
        }
        public void HardDrop()
        {
            if (Paused) return;
            while (CanFit(Current, Current.X, Current.Y + 1)) Current.Y++;
            Lock();
        }
        public bool Tick()
        {
            if (GameOver || Paused) return !GameOver;
            if (CanFit(Current, Current.X, Current.Y + 1))
            { Current.Y++; Redraw?.Invoke(); return true; }
            Lock();
            return !GameOver;
        }

        private void Lock()
        {
            int R = Current.Shape.GetLength(0), C = Current.Shape.GetLength(1);
            for (int r = 0; r < R; r++)
                for (int c = 0; c < C; c++)
                    if (Current.Shape[r, c] != 0)
                        Grid[Current.Y + r, Current.X + c] = Current.Color;
            ClearLines();
            SpawnPair();
        }

        private void ClearLines()
        {
            int cleared = 0;
            for (int r = Rows - 1; r >= 0; r--)
            {
                bool full = true;
                for (int c = 0; c < Cols && full; c++)
                    if (Grid[r, c] == Color.Empty) full = false;
                if (!full) continue;
                for (int rr = r; rr > 0; rr--)
                    for (int c = 0; c < Cols; c++) Grid[rr, c] = Grid[rr - 1, c];
                for (int c = 0; c < Cols; c++) Grid[0, c] = Color.Empty;
                cleared++; r++;
            }
            if (cleared <= 0) return;
            int[] pts = { 0, 100, 300, 500, 800 };
            Score += pts[Math.Min(cleared, 4)] * (Level + 1);
            Lines += cleared;
            Level = Lines / 10;
        }

        public int GhostY()
        {
            int gy = Current.Y;
            while (CanFit(Current, Current.X, gy + 1)) gy++;
            return gy;
        }
    }

    // ── TetrisGame Form ──────────────────────────────────────────────────
    public partial class TetrisGame : Form
    {
        // Madhësia normale e dritares (CS=30, PAD=14, SIDE=162)
        private const int BASE_CS = 30;
        private const int BASE_PAD = 14;
        private const int BASE_SIDE = 162;
        private const int BASE_W = Board.Cols * BASE_CS + BASE_SIDE + BASE_PAD * 3;
        private const int BASE_H = Board.Rows * BASE_CS + BASE_PAD * 2;

        private readonly Board board = new Board();
        private readonly System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private bool started = false;
        private Form _parentForm;
        // Panelet
        private BufferedPanel boardPanel;
        private BufferedPanel nextPanel;
        private Panel sidePanel;

        // Labels
        private Label lblTitle, lblNext;
        private Label lblScoreCap, lblScoreVal;
        private Label lblLinesCap, lblLinesVal;
        private Label lblLevelCap, lblLevelVal;

        // Buttons
        private Button btnPlay, btnPause, btnRestart, btnExit;

        // ── Scale dinamik ─────────────────────────────────────────────
        // Merr shkallën e tanishme (sa herë resize / maximize)
        private float Scale
        {
            get
            {
                float sx = (float)ClientSize.Width / BASE_W;
                float sy = (float)ClientSize.Height / BASE_H;
                return Math.Min(sx, sy);
            }
        }

        private int CS => Math.Max(4, (int)(BASE_CS * Scale));
        private int PAD => Math.Max(4, (int)(BASE_PAD * Scale));
        private int SIDE_W => Math.Max(70, (int)(BASE_SIDE * Scale));
        private int BW => Board.Cols * CS;
        private int BH => Board.Rows * CS;
        private float FS(float b) => Math.Max(5.5f, b * Scale);

        // ─────────────────────────────────────────────────────────────
        public TetrisGame()
        {
           // InitializeComponent();

            // Form properties
            Text = "TETRIS";
            BackColor = Color.FromArgb(13, 13, 22);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            MinimumSize = new Size(320, 420);
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            ClientSize = new Size(BASE_W, BASE_H);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            BuildUI();
            SetupTimer();

            // Çdo herë që ndryshon madhësia → ri-layout-o
            Resize += (s, e) => DoLayout();

            board.Redraw += RefreshView;
            SetButtonState(false);
        }
        public TetrisGame(Form parent) : this()
        {
            _parentForm = parent; // ruaj referencën
            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else if (parent.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Normal;
        }
        private void RefreshView()
        {
            if (boardPanel.InvokeRequired)
            {
                boardPanel.Invoke((Action)RefreshView);
                return;
            }
            boardPanel.Invalidate();
            nextPanel.Invalidate();
            UpdateLabels();
        }

        // ── Build UI ─────────────────────────────────────────────────
        private void BuildUI()
        {
            boardPanel = new BufferedPanel { BackColor = Color.FromArgb(8, 8, 16) };
            boardPanel.Paint += BoardPanel_Paint;
            Controls.Add(boardPanel);

            sidePanel = new Panel { BackColor = Color.Transparent };
            Controls.Add(sidePanel);

            lblTitle = MkLbl("TETRIS", Color.FromArgb(0, 210, 255));
            lblNext = MkLbl("NEXT", Color.FromArgb(130, 130, 160));
            lblScoreCap = MkLbl("SCORE", Color.FromArgb(130, 130, 160));
            lblScoreVal = MkLbl("0", Color.White);
            lblLinesCap = MkLbl("LINES", Color.FromArgb(130, 130, 160));
            lblLinesVal = MkLbl("0", Color.White);
            lblLevelCap = MkLbl("LEVEL", Color.FromArgb(130, 130, 160));
            lblLevelVal = MkLbl("1", Color.White);

            nextPanel = new BufferedPanel { BackColor = Color.FromArgb(18, 18, 32) };
            nextPanel.Paint += NextPanel_Paint;

            btnPlay = MkBtn("▶  PLAY", Color.FromArgb(0, 155, 65));
            btnPause = MkBtn("⏸  PAUSE", Color.FromArgb(175, 115, 0));
            btnRestart = MkBtn("↺  RESTART", Color.FromArgb(155, 50, 0));
            btnExit = MkBtn("✕  EXIT", Color.FromArgb(155, 20, 20));

            btnPlay.Click += BtnPlay_Click;
            btnPause.Click += BtnPause_Click;
            btnRestart.Click += BtnRestart_Click;
            btnExit.Click += BtnExit_Click;

            Label[] lbls = { lblTitle, lblNext, lblScoreCap, lblScoreVal,
                              lblLinesCap, lblLinesVal, lblLevelCap, lblLevelVal };
            foreach (var l in lbls) sidePanel.Controls.Add(l);
            sidePanel.Controls.Add(nextPanel);
            foreach (var b in new[] { btnPlay, btnPause, btnRestart, btnExit })
                sidePanel.Controls.Add(b);

            KeyDown += Form_KeyDown;
            DoLayout();
        }

        // ── DoLayout: ripozicionon gjithçka sipas Scale aktual ────────
        private void DoLayout()
        {
            int cs = CS;
            int pad = PAD;
            int sw = SIDE_W;
            int bw = BW;
            int bh = BH;

            // Qendërzim nëse dritarja është më e madhe se layout-i
            int totalW = bw + sw + pad * 3;
            int totalH = bh + pad * 2;
            int ox = Math.Max(0, (ClientSize.Width - totalW) / 2);
            int oy = Math.Max(0, (ClientSize.Height - totalH) / 2);

            boardPanel.SetBounds(ox + pad, oy + pad, bw, bh);
            sidePanel.SetBounds(ox + bw + pad * 2, oy + pad, sw, bh);

            int sy = 0;

            // Title
            int th = Math.Max(28, (int)(42 * Scale));
            SetCtrl(lblTitle, 0, sy, sw, th, FS(19), FontStyle.Bold); sy += th + (int)(4 * Scale);

            // NEXT label
            int slh = Math.Max(14, (int)(18 * Scale));
            SetCtrl(lblNext, 0, sy, sw, slh, FS(8), FontStyle.Bold); sy += slh + (int)(2 * Scale);

            // Next panel
            int nm = Math.Max(4, (int)(8 * Scale));
            int nph = Math.Max(40, (int)(78 * Scale));
            nextPanel.SetBounds(nm, sy, sw - nm * 2, nph);
            sy += nph + (int)(10 * Scale);

            // Stats
            void Stat(Label cap, Label val)
            {
                int ch = Math.Max(13, (int)(17 * Scale));
                int vh = Math.Max(18, (int)(28 * Scale));
                SetCtrl(cap, 0, sy, sw, ch, FS(8), FontStyle.Bold); sy += ch + (int)(2 * Scale);
                SetCtrl(val, 0, sy, sw, vh, FS(14), FontStyle.Bold); sy += vh + (int)(8 * Scale);
            }
            Stat(lblScoreCap, lblScoreVal);
            Stat(lblLinesCap, lblLinesVal);
            Stat(lblLevelCap, lblLevelVal);
            sy += (int)(6 * Scale);

            // Buttons
            int bh2 = Math.Max(24, (int)(36 * Scale));
            int bg = Math.Max(3, (int)(6 * Scale));
            int bfs = (int)FS(9);
            foreach (var b in new[] { btnPlay, btnPause, btnRestart, btnExit })
            {
                b.SetBounds(0, sy, sw, bh2);
                b.Font = new Font("Consolas", Math.Max(6, bfs), FontStyle.Bold);
                sy += bh2 + bg;
            }

            boardPanel.Invalidate();
            nextPanel.Invalidate();
        }

        private void SetCtrl(Label l, int x, int y, int w, int h, float fs, FontStyle style)
        {
            l.SetBounds(x, y, w, h);
            l.Font = new Font("Consolas", fs, style);
        }

        // ── Timer ────────────────────────────────────────────────────
        private void SetupTimer()
        {
            gameTimer.Interval = 500;
            gameTimer.Tick += (s, e) =>
            {
                if (!board.Tick())
                {
                    gameTimer.Stop();
                    boardPanel.Invalidate();
                    SetButtonState(false);
                    MessageBox.Show(
                        $"GAME OVER!\n\nScore : {board.Score:N0}\nLines : {board.Lines}\nLevel : {board.Level + 1}",
                        "Tetris", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    int iv = Math.Max(80, 500 - board.Level * 45);
                    if (gameTimer.Interval != iv) gameTimer.Interval = iv;
                }
            };
        }

        // ── Button handlers ──────────────────────────────────────────
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (!started) { board.Reset(); started = true; gameTimer.Start(); }
            else if (board.Paused) { board.Paused = false; gameTimer.Start(); }
            SetButtonState(true);
            boardPanel.Focus();
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            // Ndal timer-in e lojës
            gameTimer.Stop();
            // Shenjo se loja nuk është më aktive
            started = false;

            // Nëse ke një formular prind (p.sh. menu kryesore), shfaqe
            // Për këtë, duhet të kesh një fushë private Form _parentForm dhe ta vendosësh në konstruktor.
            // Nëse nuk ke, thjesht mbyll formularin aktual.
            if (_parentForm != null)
            {
                _parentForm.Show();
            }

            // Mbyll formularin e Tetris-it
            this.Close();
        }
        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (!started || board.GameOver || board.Paused) return;
            board.Paused = true;
            gameTimer.Stop();
            boardPanel.Invalidate();
            SetButtonState(true);
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            gameTimer.Stop();
            board.Reset();
            started = true;
            gameTimer.Interval = 500;
            gameTimer.Start();
            SetButtonState(true);
            UpdateLabels();
            boardPanel.Focus();
        }

        private void SetButtonState(bool running)
        {
            bool p = board.Paused;
            btnPlay.Text = (!running || p) ? "▶  PLAY / RESUME" : "▶  DUKE LUAJTUR";
            btnPlay.Enabled = !running || p;
            btnPause.Enabled = running && !p && !board.GameOver;
        }

        private void UpdateLabels()
        {
            if (lblScoreVal == null) return;
            lblScoreVal.Text = board.Score.ToString("N0");
            lblLinesVal.Text = board.Lines.ToString();
            lblLevelVal.Text = (board.Level + 1).ToString();
        }

        // ── Keyboard ─────────────────────────────────────────────────
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (!started || board.GameOver) return;
            switch (e.KeyCode)
            {
                case Keys.Left: board.MoveLeft(); break;
                case Keys.Right: board.MoveRight(); break;
                case Keys.Up: board.Rotate(); break;
                case Keys.Down: board.Tick(); break;
                case Keys.Space: board.HardDrop(); break;
                case Keys.P:
                case Keys.Escape: BtnPause_Click(null, null); break;
            }
            e.Handled = true;
        }

        // ── Paint: Board ─────────────────────────────────────────────
        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            int cs = CS;
            int bw = boardPanel.Width;
            int bh = boardPanel.Height;

            g.SmoothingMode = SmoothingMode.None;
            g.Clear(Color.FromArgb(8, 8, 16));

            using (var gp = new Pen(Color.FromArgb(22, 255, 255, 255)))
            {
                for (int c = 0; c <= Board.Cols; c++) g.DrawLine(gp, c * cs, 0, c * cs, bh);
                for (int r = 0; r <= Board.Rows; r++) g.DrawLine(gp, 0, r * cs, bw, r * cs);
            }

            for (int r = 0; r < Board.Rows; r++)
                for (int c = 0; c < Board.Cols; c++)
                    if (board.Grid[r, c] != Color.Empty)
                        PaintCell(g, c, r, board.Grid[r, c], 255, 0, 0, cs);

            if (started && !board.GameOver)
            {
                PaintPiece(g, board.Current, board.Current.X, board.GhostY(), 55, cs);
                PaintPiece(g, board.Current, board.Current.X, board.Current.Y, 255, cs);
            }

            if (board.Paused)
            {
                using (var dim = new SolidBrush(Color.FromArgb(170, 0, 0, 0)))
                    g.FillRectangle(dim, 0, 0, bw, bh);
                DrawCentered(g, "⏸  PAUSED", FS(22), Color.White, bw, bh);
            }

            if (!started)
            {
                using (var dim = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                    g.FillRectangle(dim, 0, 0, bw, bh);
                DrawCentered(g, "Shtyp PLAY\npër të filluar", FS(15), Color.FromArgb(0, 210, 255), bw, bh);
            }

            using (var bp = new Pen(Color.FromArgb(0, 185, 255), 2))
                g.DrawRectangle(bp, 1, 1, bw - 2, bh - 2);
        }

        // ── Paint: Next ───────────────────────────────────────────────
        private void NextPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.FromArgb(18, 18, 32));
            if (!started || board.Next == null) return;

            var p = board.Next;
            int R = p.Shape.GetLength(0), C = p.Shape.GetLength(1);
            int cs = Math.Max(6, (int)(18 * Scale));
            int ox = (nextPanel.Width - C * cs) / 2;
            int oy = (nextPanel.Height - R * cs) / 2;

            for (int r = 0; r < R; r++)
                for (int c = 0; c < C; c++)
                    if (p.Shape[r, c] != 0)
                        PaintCell(g, c, r, p.Color, 255, ox, oy, cs);
        }

        // ── Vizatim helpers ───────────────────────────────────────────
        private void PaintPiece(Graphics g, Piece p, int px, int py, int alpha, int cs)
        {
            int R = p.Shape.GetLength(0), C = p.Shape.GetLength(1);
            for (int r = 0; r < R; r++)
                for (int c = 0; c < C; c++)
                    if (p.Shape[r, c] != 0)
                        PaintCell(g, px + c, py + r, p.Color, alpha, 0, 0, cs);
        }

        private void PaintCell(Graphics g, int col, int row, Color color,
                                int alpha, int ox, int oy, int size)
        {
            int x = ox + col * size, y = oy + row * size;
            int hl = Math.Max(2, size / 7);

            using (var b = new SolidBrush(Color.FromArgb(alpha, color)))
                g.FillRectangle(b, x + 1, y + 1, size - 2, size - 2);
            using (var h = new SolidBrush(Color.FromArgb(alpha * 70 / 255, 255, 255, 255)))
            { g.FillRectangle(h, x + 1, y + 1, size - 2, hl); g.FillRectangle(h, x + 1, y + 1, hl, size - 2); }
            using (var s = new SolidBrush(Color.FromArgb(alpha * 80 / 255, 0, 0, 0)))
            { g.FillRectangle(s, x + 1, y + size - hl - 1, size - 2, hl); g.FillRectangle(s, x + size - hl - 1, y + 1, hl, size - 2); }
        }

        private static void DrawCentered(Graphics g, string txt, float fs,
                                          Color col, int w, int h)
        {
            using (var fnt = new Font("Consolas", fs, FontStyle.Bold))
            using (var br = new SolidBrush(col))
            using (var sf = new StringFormat
            { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(txt, fnt, br, new RectangleF(0, 0, w, h), sf);
        }

        // ── Control factories ─────────────────────────────────────────
        private static Label MkLbl(string text, Color fg) => new Label
        {
            Text = text,
            ForeColor = fg,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Consolas", 10, FontStyle.Bold)
        };

        private static Button MkBtn(string text, Color bg)
        {
            var b = new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Consolas", 10, FontStyle.Bold)
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(60, 255, 255, 255);
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.25f);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(bg, 0.2f);
            return b;
        }
    }
}