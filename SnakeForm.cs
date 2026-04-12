using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace GameHub
{
    public partial class SnakeForm : Form
    {
        Image appleImage;
        Image snakeHeadImage;
        Image snakeBodyImage;
        Image snakeBodyElbowImage;
        private Form _parentForm;

        int score = 0;
        Rectangle gameArea;
        Rectangle sidePanel;
        bool isPaused = false;
        bool showPauseMenu = false;
        bool gameStarted = false;

        List<Point> snake = new List<Point>();
        List<string> segmentDirections = new List<string>();

        Point food;

        int cellSize = 20;
        int gridWidth = 25;
        int gridHeight = 25;

        string direction = "RIGHT";
        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rand = new Random();

        // Font-et krijohen NJË herë si fields, jo çdo frame në OnPaint
        Font titleFont = new Font("Arial", 18, FontStyle.Bold);
        Font textFont = new Font("Arial", 12);
        Font scoreFont = new Font("Arial", 24, FontStyle.Bold);
        Font speedFont = new Font("Arial", 14, FontStyle.Bold);
        Font pauseFont = new Font("Arial", 26, FontStyle.Bold);
        Font pauseTitleFont = new Font("Arial", 28, FontStyle.Bold);
        Font startFont = new Font("Arial", 20, FontStyle.Bold);
        Font subFont = new Font("Arial", 12);
        Font debugFont = new Font("Arial", 10);

        SoundPlayer eatSound = new SoundPlayer();
        SoundPlayer winSound = new SoundPlayer();

        public SnakeForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.FormClosing += SnakeForm_FormClosing;
            this.MouseClick += SnakeForm_MouseClick;

            this.BackColor = Color.Black;
            this.MinimumSize = new Size(900, 600);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);

            appleImage = Properties.Resources.apple;
            snakeHeadImage = Properties.Resources.snake_head;
            snakeBodyImage = Properties.Resources.snake_body;
            snakeBodyElbowImage = Properties.Resources.snake_body_elbow;

            try
            {
                eatSound = new SoundPlayer();
                winSound = new SoundPlayer();
            }
            catch { }

            gameTimer.Tick += GameLoop;
        }

        private void SnakeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameTimer != null) { gameTimer.Stop(); gameTimer.Dispose(); }

            titleFont?.Dispose();
            textFont?.Dispose();
            scoreFont?.Dispose();
            speedFont?.Dispose();
            pauseFont?.Dispose();
            pauseTitleFont?.Dispose();
            startFont?.Dispose();
            subFont?.Dispose();
            debugFont?.Dispose();

            eatSound?.Dispose();
            winSound?.Dispose();
        }

        public SnakeForm(Form parent) : this()
        {
            _parentForm = parent; // ruaj referencën
            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else if (parent.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void StartGame()
        {
            snake.Clear();
            segmentDirections.Clear();
            snake.Add(new Point(5, 5)); segmentDirections.Add("RIGHT");
            snake.Add(new Point(4, 5)); segmentDirections.Add("RIGHT");
            snake.Add(new Point(3, 5)); segmentDirections.Add("RIGHT");

            direction = "RIGHT";
            score = 0;
            gameStarted = true;
            isPaused = false;
            showPauseMenu = false;

            GenerateFood();

            // Fillon me 120ms
            gameTimer.Interval = 120;
            gameTimer.Start();
        }

        private void GenerateFood()
        {
            // HashSet për kërkim O(1) - eliminon lag kur gjarpri është i gjatë
            HashSet<Point> snakeSet = new HashSet<Point>(snake);
            List<Point> freeCells = new List<Point>();

            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                {
                    Point p = new Point(x, y);
                    if (!snakeSet.Contains(p))
                        freeCells.Add(p);
                }

            if (freeCells.Count == 0) { GameWin(); return; }

            food = freeCells[rand.Next(freeCells.Count)];
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || isPaused) return;
            MoveSnake();
            this.Invalidate();
        }

        private void MoveSnake()
        {
            Point head = snake[0];

            if (direction == "RIGHT") head.X++;
            if (direction == "LEFT") head.X--;
            if (direction == "UP") head.Y--;
            if (direction == "DOWN") head.Y++;

            // WALL COLLISION
            if (head.X < 0 || head.Y < 0 || head.X >= gridWidth || head.Y >= gridHeight)
            {
                gameTimer.Stop();
                GameOver("You hit the wall!");
                return;
            }

            // SELF COLLISION - HashSet O(1)
            HashSet<Point> snakeSet = new HashSet<Point>(snake);
            if (snakeSet.Contains(head))
            {
                gameTimer.Stop();
                GameOver("You ran into yourself!");
                return;
            }

            snake.Insert(0, head);
            segmentDirections.Insert(0, direction);

            if (head == food)
            {
                try { eatSound?.Play(); } catch { }

                score += 10;

                // Zvogëlohet 8% çdo mollë - ndjehet menjëherë!
                // 120 -> 110 -> 101 -> 93 -> 86 -> 79 -> 73 -> 67 -> 61 -> 56 -> 52 -> 48 -> 44 -> 40
                // pas ~13 mollëve arrin maksimumin (40ms)
                int newInterval = Math.Max(40, (int)(gameTimer.Interval * 0.92));
                if (newInterval != gameTimer.Interval)
                {
                    gameTimer.Stop();
                    gameTimer.Interval = newInterval;
                    gameTimer.Start();
                    System.Diagnostics.Debug.WriteLine($"Score: {score}, Interval: {gameTimer.Interval}ms");
                }

                GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
                segmentDirections.RemoveAt(segmentDirections.Count - 1);
            }
        }

        private void GameWin()
        {
            gameTimer.Stop();
            gameStarted = false;
            isPaused = false;

            try { winSound?.Play(); } catch { }

            MessageBox.Show("🏆 CONGRATULATIONS! 🏆\n\nYou filled the entire grid!\nPerfect score: " + score,
                "YOU WIN!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            snake.Clear();
            segmentDirections.Clear(); // ✅ KY MUNGONTE!
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "RIGHT";
            GenerateFood();
            score = 0;
            gameTimer.Interval = 120;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int panelWidth = 250;

            gameArea = new Rectangle(
                20, 20,
                this.ClientSize.Width - panelWidth - 60,
                this.ClientSize.Height - 40
            );

            sidePanel = new Rectangle(
                gameArea.Right + 10, 20,
                panelWidth,
                this.ClientSize.Height - 40
            );
        

            g.FillRectangle(Brushes.Black, this.ClientRectangle);
            g.DrawRectangle(Pens.White, gameArea);

            int cellW = gameArea.Width / gridWidth;
            int cellH = gameArea.Height / gridHeight;

            // NearestNeighbor - SHUMË më i shpejtë se HighQualityBicubic
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // SNAKE
            for (int i = 0; i < snake.Count; i++)
            {
                int snakeX = gameArea.X + snake[i].X * cellW;
                int snakeY = gameArea.Y + snake[i].Y * cellH;

                int width = (int)(cellW * 1.2);
                int height = (int)(cellH * 1.2);

                if (i == 0) // kreu
                {
                    g.TranslateTransform(snakeX + cellW / 2, snakeY + cellH / 2);
                    float angle = 0f;
                    switch (direction)
                    {
                        case "UP": angle = -90f; break;
                        case "DOWN": angle = 90f; break;
                        case "LEFT": angle = 180f; break;
                        case "RIGHT": angle = 0f; break;
                    }
                    g.RotateTransform(angle);
                    g.DrawImage(snakeHeadImage, -width / 2, -height / 2, width, height);
                    g.ResetTransform();
                }
                else
                {
                    string prevDir = (i - 1 < segmentDirections.Count) ? segmentDirections[i - 1] : direction;
                    string currDir = (i < segmentDirections.Count) ? segmentDirections[i] : direction;

                    g.TranslateTransform(snakeX + cellW / 2, snakeY + cellH / 2);

                    if (prevDir == currDir)
                    {
                        float angle = (currDir == "UP") ? -90f :
                                      (currDir == "DOWN") ? 90f :
                                      (currDir == "LEFT") ? 180f : 0f;
                        g.RotateTransform(angle);
                        g.DrawImage(snakeBodyImage, -width / 2, -height / 2, width, height);
                    }
                    else
                    {
                        float angle = 0f;
                        if (prevDir == "LEFT" && currDir == "UP") angle = 90f;
                        else if (prevDir == "DOWN" && currDir == "LEFT") angle = 0f;
                        else if (prevDir == "RIGHT" && currDir == "DOWN") angle = 270f;
                        else if (prevDir == "UP" && currDir == "RIGHT") angle = 180f;
                        else if (prevDir == "RIGHT" && currDir == "UP") angle = 0f;
                        else if (prevDir == "DOWN" && currDir == "RIGHT") angle = 90f;
                        else if (prevDir == "LEFT" && currDir == "DOWN") angle = 180f;
                        else if (prevDir == "UP" && currDir == "LEFT") angle = 270f;
                        g.RotateTransform(angle);
                        g.DrawImage(snakeBodyElbowImage, -width / 2, -height / 2, width, height);
                    }

                    g.ResetTransform();
                }
            }

            // FOOD
            int appleW = (int)(cellW * 1.7);
            int appleH = (int)(cellH * 1.9);
            int appleX = gameArea.X + food.X * cellW - (appleW - cellW) / 2;
            int appleY = gameArea.Y + food.Y * cellH - (appleH - cellH) / 2;
            g.DrawImage(appleImage, appleX, appleY, appleW, appleH);

            // ================= SIDE PANEL =================
            using (SolidBrush panelBrush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                g.FillRectangle(panelBrush, sidePanel);
            g.DrawRectangle(Pens.Gray, sidePanel);

            int y = sidePanel.Y + 20;

            g.DrawString("SCORE", titleFont, Brushes.White, sidePanel.X + 20, y); y += 40;
            g.DrawString(score.ToString(), scoreFont, Brushes.Lime, sidePanel.X + 20, y); y += 60;

            g.DrawString("SPEED", titleFont, Brushes.White, sidePanel.X + 20, y); y += 40;
            int speedPct = (int)((1.0 - (gameTimer.Interval - 40) / 80.0) * 100);
            speedPct = Math.Max(0, Math.Min(100, speedPct));
            g.DrawString($"{speedPct}%", speedFont, Brushes.Cyan, sidePanel.X + 20, y); y += 28;
            // Debug line - shfaq intervalin aktual që ta shohësh me sytë
            g.DrawString($"({gameTimer.Interval}ms)", debugFont, Brushes.Gray, sidePanel.X + 20, y); y += 35;

            g.DrawString("CONTROLS", titleFont, Brushes.White, sidePanel.X + 20, y); y += 40;
            g.DrawString("↑ = UP", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("↓ = DOWN", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("← = LEFT", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("→ = RIGHT", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            y += 20;
            g.DrawString("P = PAUSE", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            y += 10;
            g.DrawString("ENTER = START", textFont, Brushes.Yellow, sidePanel.X + 20, y);
            // EXIT BUTTON - në fund të side panel
            Rectangle exitBtn = new Rectangle(sidePanel.X + 20, sidePanel.Bottom - 60, sidePanel.Width - 40, 40);
            using (SolidBrush btnBrush = new SolidBrush(Color.FromArgb(180, 30, 30)))
                g.FillRectangle(btnBrush, exitBtn);
            g.DrawRectangle(Pens.Red, exitBtn);
            g.DrawString("EXIT TO MENU", textFont, Brushes.White,
                exitBtn.X + (exitBtn.Width - g.MeasureString("EXIT TO MENU", textFont).Width) / 2,
                exitBtn.Y + 10);

            // START MESSAGE
            if (!gameStarted)
            {
                string msg = "PRESS ENTER TO START";
                SizeF size = g.MeasureString(msg, startFont);
                g.DrawString(msg, startFont, Brushes.Yellow,
                    gameArea.X + (gameArea.Width - size.Width) / 2,
                    gameArea.Y + (gameArea.Height - size.Height) / 2);
            }

            if (isPaused)
            {
                string msg = "PAUSED";
                SizeF size = g.MeasureString(msg, pauseFont);
                RectangleF rect = new RectangleF(
                    gameArea.X + (gameArea.Width - size.Width) / 2 - 20,
                    gameArea.Y + (gameArea.Height - size.Height) / 2 - 10,
                    size.Width + 40, size.Height + 20);
                using (Brush bg = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                    g.FillRectangle(bg, rect);
                g.DrawString(msg, pauseFont, Brushes.Orange, rect.X + 20, rect.Y + 10);
            }

            if (showPauseMenu)
            {
                string msg = "PAUSED";
                string sub = "Press P or ESC to Resume";
                SizeF size = g.MeasureString(msg, pauseTitleFont);
                RectangleF rect = new RectangleF(
                    gameArea.X + (gameArea.Width - size.Width) / 2 - 40,
                    gameArea.Y + (gameArea.Height - size.Height) / 2 - 20,
                    size.Width + 80, size.Height + 60);
                using (Brush bg = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                    g.FillRectangle(bg, rect);
                g.DrawString(msg, pauseTitleFont, Brushes.Orange, rect.X + 40, rect.Y + 10);
                g.DrawString(sub, subFont, Brushes.White, rect.X + 40, rect.Y + 40);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "RIGHT";
            GenerateFood();
            gameStarted = false;
            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!gameStarted && keyData == Keys.Enter)
            {
                StartGame();
                return true;
            }

            if (gameStarted && (keyData == Keys.P || keyData == Keys.Escape))
            {
                isPaused = !isPaused;
                showPauseMenu = isPaused;
                if (isPaused) gameTimer.Stop();
                else gameTimer.Start();
                this.Invalidate();
                return true;
            }

            if (isPaused) return true;

            if (keyData == Keys.Up && direction != "DOWN") direction = "UP";
            if (keyData == Keys.Down && direction != "UP") direction = "DOWN";
            if (keyData == Keys.Left && direction != "RIGHT") direction = "LEFT";
            if (keyData == Keys.Right && direction != "LEFT") direction = "RIGHT";

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GameOver(string reason)
        {
            gameTimer.Stop();
            gameStarted = false;
            isPaused = false;

            MessageBox.Show(reason, "🐍 Game Over!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            snake.Clear();
            segmentDirections.Clear(); // ✅ KY MUNGONTE!
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "RIGHT";
            gameTimer.Interval = 120;
            GenerateFood();
            score = 0;

            this.Invalidate();
        }

        private void SnakeForm_Load(object sender, EventArgs e) { }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
        private void SnakeForm_MouseClick(object sender, MouseEventArgs e)
        {
            int panelWidth = 250;
            Rectangle exitBtn = new Rectangle(sidePanel.X + 20, sidePanel.Bottom - 60, sidePanel.Width - 40, 40);

            if (exitBtn.Contains(e.Location))
            {
                gameTimer.Stop();
                gameStarted = false;

                if (_parentForm != null)
                {
                    _parentForm.Show();
                }
                this.Close();
            }
        }
    }
}