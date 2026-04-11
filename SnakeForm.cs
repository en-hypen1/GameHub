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
        Image snakeBodyElbowImage; // PNG për kthesat
        int score = 0;
        Rectangle gameArea;
        Rectangle sidePanel;
        bool isPaused = false;
        bool showPauseMenu = false;

        bool gameStarted = false;
        List<Point> snake = new List<Point>();
        List<string> segmentDirections = new List<string>(); // drejtimi i secilit segment

        Point food;

        int cellSize = 20;
        int gridWidth = 25;
        int gridHeight = 25;

        string direction = "RIGHT";
        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rand = new Random();

        // Për audio
        SoundPlayer eatSound = new SoundPlayer();
        SoundPlayer winSound = new SoundPlayer();

        public SnakeForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.FormClosing += SnakeForm_FormClosing; // Për të pastruar timer-in

            this.BackColor = Color.Black;
            this.MinimumSize = new Size(900, 600);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);

            appleImage = Properties.Resources.apple;
            snakeHeadImage = Properties.Resources.snake_head;
            snakeBodyImage = Properties.Resources.snake_body;
            snakeBodyElbowImage = Properties.Resources.snake_body_elbow;

            // Inicializo audio (nëse ke skedarë zëri, vendosi në resources)
            try
            {
                // Për të përdorur skedarë zëri, shtoji në Resources dhe përdori kështu:
                // eatSound.Stream = Properties.Resources.eat_sound;
                // winSound.Stream = Properties.Resources.win_sound;

                // Nëse nuk ke skedarë, kjo do të jetë silent
                eatSound = new SoundPlayer();
                winSound = new SoundPlayer();
            }
            catch { }

            gameTimer.Tick += GameLoop;
        }

        // Pastrimi i burimeve kur mbyllet forma
        private void SnakeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }

            if (eatSound != null)
                eatSound.Dispose();
            if (winSound != null)
                winSound.Dispose();
        }

        // Constructor me parameter Form parent
        public SnakeForm(Form parent) : this()  // thirr constructor default
        {
            // Vendos madhësinë bazuar tek Form1
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
            gameStarted = true; // <--- KJO ISHTE E MUNGUAR!

            isPaused = false;
            showPauseMenu = false;

            GenerateFood();

            // Shpejtësia fillestare: 120 ms
            gameTimer.Interval = 120;
            gameTimer.Start();
        }

        private void GenerateFood()
        {
            List<Point> freeCells = new List<Point>();

            // Gjej të gjitha qelizat e lira
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Point p = new Point(x, y);
                    if (!snake.Contains(p))
                    {
                        freeCells.Add(p);
                    }
                }
            }

            // Nëse nuk ka qeliza të lira, loja ka mbaruar (fitore)
            if (freeCells.Count == 0)
            {
                GameWin();
                return;
            }

            // Zgjidh një qelizë të rastësishme nga ato të lirat
            int index = rand.Next(freeCells.Count);
            food = freeCells[index];
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || isPaused)
                return;

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

            // SELF COLLISION
            if (snake.Contains(head))
            {
                gameTimer.Stop();
                GameOver("You ran into yourself!");
                return;
            }

            snake.Insert(0, head);
            segmentDirections.Insert(0, direction); // krye merr drejtimin e ri

            if (head == food)
            {
                // Luaj zërin e ngrënies
                try
                {
                    if (eatSound != null)
                        eatSound.Play();
                }
                catch { }

                score += 10;

                // Rrit shpejtësinë (zvogëlo intervalin) - sa më shumë ha, aq më shpejtë lëviz
                // Shpejtësia maksimale: 40 ms (minimumi)
                // ZVOGËLOJMË INTERVALIN PËR TA BËRË MË TË SHPEJTË
                int newInterval = Math.Max(40, gameTimer.Interval - 2); // Zvogëlo me 2 ms për çdo mollë
                if (newInterval != gameTimer.Interval)
                {
                    gameTimer.Interval = newInterval;
                    // Debug: Shiko në konsol se çfarë ndodh me intervalin
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

        // Funksioni i ri për fitore
        private void GameWin()
        {
            gameTimer.Stop();
            gameStarted = false;
            isPaused = false;

            // Luaj zërin e fitores
            try
            {
                if (winSound != null)
                    winSound.Play();
            }
            catch { }

            MessageBox.Show("🏆 CONGRATULATIONS! 🏆\n\nYou filled the entire grid!\nPerfect score: " + score,
                "YOU WIN!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset lojën
            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "RIGHT";
            GenerateFood();
            score = 0;
            gameTimer.Interval = 120; // Rivendos shpejtësinë fillestare

            this.Invalidate(); // rifreskon ekranin me "Press ENTER"
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int panelWidth = 250; // fixed size

            gameArea = new Rectangle(
                20,
                20,
                this.ClientSize.Width - panelWidth - 60,
                this.ClientSize.Height - 40
            );

            sidePanel = new Rectangle(
                gameArea.Right + 10,
                20,
                panelWidth,
                this.ClientSize.Height - 40
            );

            // BACKGROUND
            g.FillRectangle(Brushes.Black, this.ClientRectangle);

            // GAME BORDER
            g.DrawRectangle(Pens.White, gameArea);

            // SCALE CELL SIZE DINAMIK
            int cellW = gameArea.Width / gridWidth;
            int cellH = gameArea.Height / gridHeight;

            // SNAKE
            for (int i = 0; i < snake.Count; i++)
            {
                int snakeX = gameArea.X + snake[i].X * cellW;
                int snakeY = gameArea.Y + snake[i].Y * cellH;

                int width = (int)(cellW * 1.2);
                int height = (int)(cellH * 1.2);
                int offsetX = (width - cellW) / 2;
                int offsetY = (height - cellH) / 2;

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                if (i == 0) // krye
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
                    // kontrollo drejtimet
                    string prevDir = (i - 1 < segmentDirections.Count) ? segmentDirections[i - 1] : direction;
                    string currDir = (i < segmentDirections.Count) ? segmentDirections[i] : direction;

                    if (prevDir == currDir) // segment straight
                    {
                        g.TranslateTransform(snakeX + cellW / 2, snakeY + cellH / 2);
                        float angle = (currDir == "UP") ? -90f :
                                      (currDir == "DOWN") ? 90f :
                                      (currDir == "LEFT") ? 180f : 0f;
                        g.RotateTransform(angle);
                        g.DrawImage(snakeBodyImage, -width / 2, -height / 2, width, height);
                        g.ResetTransform();
                    }
                    else // segment me kthesë
                    {
                        g.TranslateTransform(snakeX + cellW / 2, snakeY + cellH / 2);
                        float angle = 0f;
                        // Orare / clockwise turns
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
                        g.ResetTransform();
                    }
                }
            }

            // FOOD
            int appleW = (int)(cellW * 1.7);
            int appleH = (int)(cellH * 1.9);

            int appleX = gameArea.X + food.X * cellW - (appleW - cellW) / 2;
            int appleY = gameArea.Y + food.Y * cellH - (appleH - cellH) / 2;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(appleImage, appleX, appleY, appleW, appleH);

            // ================= PANEL DJATHTAS =================
            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), sidePanel);
            g.DrawRectangle(Pens.Gray, sidePanel);

            Font titleFont = new Font("Arial", 18, FontStyle.Bold);
            Font textFont = new Font("Arial", 12);

            int y = sidePanel.Y + 20;

            // SCORE
            g.DrawString("SCORE", titleFont, Brushes.White, sidePanel.X + 20, y);
            y += 40;
            g.DrawString(score.ToString(), new Font("Arial", 24, FontStyle.Bold),
                Brushes.Lime, sidePanel.X + 20, y);

            y += 60;

            // SPEED (tregon shpejtësinë aktuale)
            g.DrawString("SPEED", titleFont, Brushes.White, sidePanel.X + 20, y);
            y += 40;
            // Sa më i vogël intervali, aq më e madhe shpejtësia
            int speedLevel = (120 - gameTimer.Interval) / 2; // 0-40
            string speedText = $"{speedLevel}% FASTER";
            g.DrawString(speedText, new Font("Arial", 14, FontStyle.Bold),
                Brushes.Cyan, sidePanel.X + 20, y);
            y += 50;

            // INSTRUCTIONS
            g.DrawString("CONTROLS", titleFont, Brushes.White, sidePanel.X + 20, y);
            y += 40;

            g.DrawString("↑ = UP", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("↓ = DOWN", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("← = LEFT", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;
            g.DrawString("→ = RIGHT", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;

            y += 20;
            g.DrawString("P = PAUSE", textFont, Brushes.White, sidePanel.X + 20, y); y += 25;

            y += 10;
            g.DrawString("ENTER = START", textFont, Brushes.Yellow, sidePanel.X + 20, y);

            // START MESSAGE
            if (!gameStarted)
            {
                string msg = "PRESS ENTER TO START";
                Font font = new Font("Arial", 20, FontStyle.Bold);

                SizeF size = g.MeasureString(msg, font);

                g.DrawString(msg, font, Brushes.Yellow,
                    gameArea.X + (gameArea.Width - size.Width) / 2,
                    gameArea.Y + (gameArea.Height - size.Height) / 2);
            }

            if (isPaused)
            {
                string msg = "PAUSED";
                Font font = new Font("Arial", 26, FontStyle.Bold);

                SizeF size = g.MeasureString(msg, font);

                RectangleF rect = new RectangleF(
                    gameArea.X + (gameArea.Width - size.Width) / 2 - 20,
                    gameArea.Y + (gameArea.Height - size.Height) / 2 - 10,
                    size.Width + 40,
                    size.Height + 20
                );

                using (Brush bg = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                {
                    g.FillRectangle(bg, rect);
                }

                g.DrawString(msg, font, Brushes.Orange,
                    rect.X + 20, rect.Y + 10);
            }
            if (showPauseMenu)
            {
                string msg = "PAUSED";
                string sub = "Press P or ESC to Resume";

                Font titleFont2 = new Font("Arial", 28, FontStyle.Bold);
                Font subFont = new Font("Arial", 12);

                SizeF size = g.MeasureString(msg, titleFont2);

                RectangleF rect = new RectangleF(
                    gameArea.X + (gameArea.Width - size.Width) / 2 - 40,
                    gameArea.Y + (gameArea.Height - size.Height) / 2 - 20,
                    size.Width + 80,
                    size.Height + 60
                );

                using (Brush bg = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                {
                    g.FillRectangle(bg, rect);
                }

                g.DrawString(msg, titleFont2, Brushes.Orange,
                    rect.X + 40, rect.Y + 10);

                g.DrawString(sub, subFont, Brushes.White,
                    rect.X + 40, rect.Y + 40);
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

            gameStarted = false; // loja nuk ka filluar

            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // START GAME
            if (!gameStarted && keyData == Keys.Enter)
            {
                StartGame();
                return true;
            }

            // PAUSE me P ose ESC
            if (gameStarted && (keyData == Keys.P || keyData == Keys.Escape))
            {
                isPaused = !isPaused;
                showPauseMenu = isPaused;

                if (isPaused)
                    gameTimer.Stop();
                else
                    gameTimer.Start();

                this.Invalidate();
                return true;
            }

            if (isPaused)
                return true; // mos lejo lëvizje gjatë pause

            // MOVEMENT
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

            // reset snake dhe direction
            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "RIGHT";
            gameTimer.Interval = 120; // Rivendos shpejtësinë
            GenerateFood();
            score = 0;

            this.Invalidate(); // rifreskon ekranin me "Press ENTER"
        }

        private void SnakeForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate(); // detyron redraw korrekt
        }
    }
}