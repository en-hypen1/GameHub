using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GameHub
{
    public partial class FlappyBirdForm : Form
    {
        int birdY = 200;
        int velocity = 0;
        int gravity = 1;
        int jumpForce = -12;
        int pipeSpeed = 5;

        List<Pipe> pipes = new List<Pipe>();

        int pipeWidth = 60;
        int gapHeight = 150;

        Image pipeImage;
        Image pipeDownImage;
        Image birdImage;

        int score = 0;
        int highScore = 0;
        bool gameStarted = false;
        bool isGameOver = false;
        bool isPaused = false;

        // Progress / finish
        const int FINISH_SCORE = 20; // 20 pipe = fitore
        bool gameWon = false;

        // Animacion zogu
        float birdAngle = 0f;
        int flashTimer = 0;

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rand = new Random();

        // Font-et si fields
        Font fontScore = new Font("Arial", 16, FontStyle.Bold);
        Font fontTitle = new Font("Arial", 32, FontStyle.Bold);
        Font fontSub = new Font("Arial", 16);
        Font fontSmall = new Font("Arial", 12);
        Font fontScore2 = new Font("Arial", 20, FontStyle.Bold);
        Font fontGameOver = new Font("Arial", 32, FontStyle.Bold);
        Font fontPause = new Font("Arial", 32, FontStyle.Bold);
        Font fontHigh = new Font("Arial", 13);
        Font fontProgress = new Font("Arial", 10, FontStyle.Bold);
        Font fontWin = new Font("Arial", 36, FontStyle.Bold);

        private Form _parentForm;

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);

        public FlappyBirdForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            pipeImage = Properties.Resources.pipe;
            pipeDownImage = Properties.Resources.pipe_down;
            birdImage = Properties.Resources.bird;

            gameTimer.Tick += GameLoop;
            this.FormClosing += FlappyBirdForm_FormClosing;
            this.MouseClick += FlappyBirdForm_MouseClick;
        }

        public FlappyBirdForm(Form parent) : this()
        {
            _parentForm = parent;

            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void FlappyBirdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();

            fontScore?.Dispose(); fontTitle?.Dispose();
            fontSub?.Dispose(); fontSmall?.Dispose();
            fontScore2?.Dispose(); fontGameOver?.Dispose();
            fontPause?.Dispose(); fontHigh?.Dispose();
            fontProgress?.Dispose(); fontWin?.Dispose();
        }

        // ── TINGUJ ──────────────────────────────────────────────
        void PlayJump()
        {
            try { System.Threading.Tasks.Task.Run(() => Beep(900, 60)); } catch { }
        }

        void PlayScore()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => {
                    Beep(1047, 80);
                    Beep(1319, 80);
                });
            }
            catch { }
        }

        void PlayDie()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => {
                    Beep(400, 100);
                    Beep(250, 200);
                });
            }
            catch { }
        }

        void PlayStart()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => {
                    Beep(523, 80);
                    Beep(659, 80);
                    Beep(784, 120);
                });
            }
            catch { }
        }

        void PlayWin()
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => {
                    Beep(784, 100); Beep(784, 100);
                    Beep(784, 100); Beep(659, 400);
                    Beep(698, 100); Beep(698, 100);
                    Beep(698, 100); Beep(587, 400);
                });
            }
            catch { }
        }
        // ────────────────────────────────────────────────────────

        void StartGame()
        {
            birdY = this.ClientSize.Height / 2;
            velocity = 0;
            score = 0;
            isPaused = false;
            birdAngle = 0f;
            flashTimer = 0;
            gameWon = false;

            pipes.Clear();

            // Pipe e parë afër
             int gapY = rand.Next(100, this.ClientSize.Height - gapHeight - 100);
            pipes.Add(new Pipe { X = 500, GapY = gapY, IsTop = true, Passed = false });
            pipes.Add(new Pipe { X = 500, GapY = gapY, IsTop = false, Passed = false });
            // PAS - relative ndaj gjerësisë së formës:
           // int firstPipeX = Math.Min(500, this.ClientSize.Width / 3);
           //// pipes.Add(new Pipe { X = firstPipeX, GapY = gapY, IsTop = true, Passed = false });
           // pipes.Add(new Pipe { X = firstPipeX, GapY = gapY, IsTop = false, Passed = false });

            gameStarted = true;
            isGameOver = false;

            gameTimer.Interval = 20;
            gameTimer.Start();

            PlayStart();
            this.Invalidate();
        }

        void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || isGameOver || isPaused || gameWon)
                return;

            velocity += gravity;
            birdY += velocity;

            birdAngle = Math.Max(-30f, Math.Min(90f, velocity * 5f));

            if (flashTimer > 0) flashTimer--;

            int formW = this.ClientSize.Width;

            for (int i = 0; i < pipes.Count; i++)
            {
                pipes[i].X -= pipeSpeed;

                if (!pipes[i].Passed && !pipes[i].IsTop && pipes[i].X + pipeWidth < 100)
                {
                    score++;
                    pipes[i].Passed = true;
                    if (score > highScore) highScore = score;
                    PlayScore();

                    // Kontrol fitore
                    if (score >= FINISH_SCORE)
                    {
                        DoGameWin();
                        return;
                    }
                }
            }

            pipes.RemoveAll(p => p.X + pipeWidth < 0);

            // Shto pipe të reja - 200px hapësirë (ishte 300) = më shumë pipe në ekran
            bool needNew = pipes.Count == 0;
            if (!needNew)
            {
                int lastX = int.MinValue;
                foreach (var p in pipes)
                    if (p.X > lastX) lastX = p.X;
                if (lastX < formW - 200) needNew = true;
            }

            if (needNew)
            {
                int gapY = rand.Next(100, this.ClientSize.Height - gapHeight - 100);
                pipes.Add(new Pipe { X = formW, GapY = gapY, IsTop = true, Passed = false });
                pipes.Add(new Pipe { X = formW, GapY = gapY, IsTop = false, Passed = false });
            }

            CheckCollision();
            this.Invalidate();
        }

        void CheckCollision()
        {
            Rectangle bird = new Rectangle(104, birdY + 4, 32, 32);

            foreach (var pipe in pipes)
            {
                Rectangle pipeRect = pipe.IsTop
                    ? new Rectangle(pipe.X, 0, pipeWidth, pipe.GapY)
                    : new Rectangle(pipe.X, pipe.GapY + gapHeight, pipeWidth, this.ClientSize.Height);

                if (bird.IntersectsWith(pipeRect))
                {
                    DoGameOver();
                    return;
                }
            }

            if (birdY > this.ClientSize.Height || birdY < 0)
                DoGameOver();
        }

        void DoGameOver()
        {
            isGameOver = true;
            flashTimer = 8;
            gameTimer.Stop();
            PlayDie();
        }

        void DoGameWin()
        {
            gameWon = true;
            gameTimer.Stop();
            PlayWin();
            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((!gameStarted || isGameOver || gameWon) && keyData == Keys.Enter)
            {
                StartGame();
                return true;
            }

            if (keyData == Keys.Space && gameStarted && !isGameOver && !isPaused && !gameWon)
            {
                velocity = jumpForce;
                PlayJump();
                return true;
            }

            if (keyData == Keys.P && gameStarted && !isGameOver && !gameWon)
            {
                TogglePause();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void TogglePause()
        {
            isPaused = !isPaused;
            if (isPaused) gameTimer.Stop();
            else gameTimer.Start();
            this.Invalidate();
        }

        private void FlappyBirdForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (GetExitButtonRect().Contains(e.Location))
            {
                gameTimer.Stop();
                _parentForm?.Show();
                this.Close();
                return;
            }

            if (GetPauseButtonRect().Contains(e.Location) && gameStarted && !isGameOver && !gameWon)
            {
                TogglePause();
                return;
            }

            if (gameStarted && !isGameOver && !isPaused && !gameWon)
            {
                velocity = jumpForce;
                PlayJump();
            }
        }

        Rectangle GetExitButtonRect() => new Rectangle(this.ClientSize.Width - 130, 10, 120, 35);
        Rectangle GetPauseButtonRect() => new Rectangle(this.ClientSize.Width - 260, 10, 120, 35);

        // ── PROGRESS BAR ────────────────────────────────────────
        void DrawProgressBar(Graphics g, int W, int H)
        {
            int barW = 200;
            int barH = 18;
            int barX = (W - barW) / 2;
            int barY = 12;

            float pct = Math.Min(1f, (float)score / FINISH_SCORE);
            int fill = (int)(barW * pct);

            // Background bar
            using (SolidBrush bg = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                g.FillRectangle(bg, barX, barY, barW, barH);

            // Fill me gradient ngjyre: jeshile -> e verdhë -> e kuqe
            Color barColor;
            if (pct < 0.5f)
                barColor = Color.FromArgb(
                    (int)(255 * pct * 2),
                    200,
                    0);
            else
                barColor = Color.FromArgb(
                    255,
                    (int)(200 * (1 - pct) * 2),
                    0);

            if (fill > 0)
                using (SolidBrush fillBrush = new SolidBrush(barColor))
                    g.FillRectangle(fillBrush, barX, barY, fill, barH);

            // Border
            g.DrawRectangle(Pens.White, barX, barY, barW, barH);

            // Flamuri i finishit në fund të barit
            g.DrawString("🏁", fontSmall, Brushes.White, barX + barW + 4, barY - 2);

            // Zogu i vogël mbi progress bar (tregon pozicionin)
            int birdIconX = barX + fill - 8;
            if (birdIconX >= barX)
                g.DrawString("🐦", fontSmall, Brushes.White, birdIconX, barY - 16);

            // Tekst: score / finish
            string prog = $"{score} / {FINISH_SCORE}";
            SizeF ts = g.MeasureString(prog, fontProgress);
            g.DrawString(prog, fontProgress, Brushes.Black, barX + (barW - ts.Width) / 2 + 1, barY + 2);
            g.DrawString(prog, fontProgress, Brushes.White, barX + (barW - ts.Width) / 2, barY + 1);
        }
        // ────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int W = this.ClientSize.Width;
            int H = this.ClientSize.Height;

            // Flash të kuq kur vdes
            if (flashTimer > 0 && flashTimer % 2 == 0)
            {
                using (Brush flash = new SolidBrush(Color.FromArgb(120, 255, 0, 0)))
                    g.FillRectangle(flash, 0, 0, W, H);
            }

            // BIRD me rotacion
            g.TranslateTransform(100 + 30, birdY + 30);
            g.RotateTransform(birdAngle);
            g.DrawImage(birdImage, -30, -30, 60, 60);
            g.ResetTransform();

            // PIPES
            foreach (var pipe in pipes)
            {
                if (pipe.IsTop)
                {
                    g.DrawImage(pipeDownImage, pipe.X, 0, pipeWidth, pipe.GapY);
                }
                else
                {
                    int pipeY = pipe.GapY + gapHeight;
                    int pipeH = H - pipeY;
                    if (pipeH > 0)
                        g.DrawImage(pipeImage, pipe.X, pipeY, pipeWidth, pipeH);
                }
            }

            // SCORE me shadow (lart majtas)
            g.DrawString("Score: " + score, fontScore, Brushes.Black, 12, 52);
            g.DrawString("Score: " + score, fontScore, Brushes.White, 10, 50);

            // HIGH SCORE
            g.DrawString("Best: " + highScore, fontHigh, Brushes.Black, 12, 77);
            g.DrawString("Best: " + highScore, fontHigh, Brushes.Yellow, 10, 75);

            // PROGRESS BAR - qendër lart
            if (gameStarted && !isGameOver && !gameWon)
                DrawProgressBar(g, W, H);

            // PAUSE BUTTON
            Rectangle pauseBtn = GetPauseButtonRect();
            using (SolidBrush pb = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                g.FillRectangle(pb, pauseBtn);
            g.DrawRectangle(isPaused ? Pens.LimeGreen : Pens.White, pauseBtn);
            string pauseLabel = isPaused ? "▶  RESUME" : "⏸  PAUSE";
            SizeF ps = g.MeasureString(pauseLabel, fontSmall);
            g.DrawString(pauseLabel, fontSmall, isPaused ? Brushes.LimeGreen : Brushes.White,
                pauseBtn.X + (pauseBtn.Width - ps.Width) / 2,
                pauseBtn.Y + (pauseBtn.Height - ps.Height) / 2);

            // EXIT BUTTON
            Rectangle exitBtn = GetExitButtonRect();
            using (SolidBrush eb = new SolidBrush(Color.FromArgb(200, 160, 30, 30)))
                g.FillRectangle(eb, exitBtn);
            g.DrawRectangle(Pens.Red, exitBtn);
            SizeF es = g.MeasureString("EXIT TO MENU", fontSmall);
            g.DrawString("EXIT TO MENU", fontSmall, Brushes.White,
                exitBtn.X + (exitBtn.Width - es.Width) / 2,
                exitBtn.Y + (exitBtn.Height - es.Height) / 2);

            // ── EKRAN STARTUES ──
            if (!gameStarted)
            {
                string msg = "FLAPPY BIRD";
                string sub = "Press ENTER or Click to Start";
                string ins = "SPACE / Click = Jump     P = Pause";
                string fin = $"Goal: pass {FINISH_SCORE} pipes to win!";

                SizeF s1 = g.MeasureString(msg, fontTitle);
                SizeF s2 = g.MeasureString(sub, fontSub);
                SizeF s3 = g.MeasureString(ins, fontSmall);
                SizeF s4 = g.MeasureString(fin, fontSmall);

                int boxW = (int)Math.Max(s1.Width, s2.Width) + 80;
                int boxH = 195;
                int boxX = (W - boxW) / 2;
                int boxY = H / 2 - 105;

                using (Brush bg = new SolidBrush(Color.FromArgb(170, 0, 0, 0)))
                    g.FillRectangle(bg, boxX, boxY, boxW, boxH);
                using (Pen border = new Pen(Color.FromArgb(200, 255, 220, 0), 2))
                    g.DrawRectangle(border, boxX, boxY, boxW, boxH);

                g.DrawString(msg, fontTitle, Brushes.Black, (W - s1.Width) / 2 + 2, boxY + 17);
                g.DrawString(msg, fontTitle, Brushes.Yellow, (W - s1.Width) / 2, boxY + 15);
                g.DrawString(sub, fontSub, Brushes.White, (W - s2.Width) / 2, boxY + 75);
                g.DrawString(ins, fontSmall, Brushes.LightGray, (W - s3.Width) / 2, boxY + 120);
                g.DrawString(fin, fontSmall, Brushes.Cyan, (W - s4.Width) / 2, boxY + 155);
            }

            // ── PAUSED ──
            if (isPaused)
            {
                using (Brush bg = new SolidBrush(Color.FromArgb(140, 0, 0, 0)))
                    g.FillRectangle(bg, 0, 0, W, H);

                string msg = "PAUSED";
                string sub = "Press P or click RESUME to continue";
                SizeF s1 = g.MeasureString(msg, fontPause);
                SizeF s2 = g.MeasureString(sub, fontSub);

                g.DrawString(msg, fontPause, Brushes.Black, (W - s1.Width) / 2 + 2, H / 2 - 48);
                g.DrawString(msg, fontPause, Brushes.Orange, (W - s1.Width) / 2, H / 2 - 50);
                g.DrawString(sub, fontSub, Brushes.White, (W - s2.Width) / 2, H / 2 + 10);
            }

            // ── GAME OVER ──
            if (isGameOver)
            {
                string msg = "GAME OVER";
                string scoreMsg = "Score: " + score;
                string bestMsg = score >= highScore && score > 0 ? "🏆 NEW BEST!" : "Best: " + highScore;
                string sub = "Press ENTER to Replay";

                SizeF s1 = g.MeasureString(msg, fontGameOver);
                SizeF s2 = g.MeasureString(scoreMsg, fontScore2);
                SizeF s3 = g.MeasureString(bestMsg, fontScore2);
                SizeF s4 = g.MeasureString(sub, fontSub);

                int boxW = (int)Math.Max(s1.Width, s2.Width) + 100;
                int boxH = 200;
                int boxX = (W - boxW) / 2;
                int boxY = H / 2 - 100;

                using (Brush bg = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                    g.FillRectangle(bg, boxX, boxY, boxW, boxH);
                using (Pen border = new Pen(Color.FromArgb(220, 220, 0, 0), 2))
                    g.DrawRectangle(border, boxX, boxY, boxW, boxH);

                g.DrawString(msg, fontGameOver, Brushes.Black, (W - s1.Width) / 2 + 2, boxY + 12);
                g.DrawString(msg, fontGameOver, Brushes.Red, (W - s1.Width) / 2, boxY + 10);
                g.DrawString(scoreMsg, fontScore2, Brushes.Yellow, (W - s2.Width) / 2, boxY + 65);
                g.DrawString(bestMsg, fontScore2, Brushes.Cyan, (W - s3.Width) / 2, boxY + 100);
                g.DrawString(sub, fontSub, Brushes.White, (W - s4.Width) / 2, boxY + 150);
            }

            // ── YOU WIN ──
            if (gameWon)
            {
                using (Brush bg = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                    g.FillRectangle(bg, 0, 0, W, H);

                string msg = "🏆 YOU WIN! 🏆";
                string s2t = $"You passed all {FINISH_SCORE} pipes!";
                string s3t = "Score: " + score;
                string sub = "Press ENTER to Play Again";

                SizeF ms = g.MeasureString(msg, fontWin);
                SizeF s2s = g.MeasureString(s2t, fontSub);
                SizeF s3s = g.MeasureString(s3t, fontScore2);
                SizeF ss = g.MeasureString(sub, fontSub);

                int boxW = (int)Math.Max(ms.Width, s2s.Width) + 100;
                int boxH = 230;
                int boxX = (W - boxW) / 2;
                int boxY = H / 2 - 115;

                using (Brush winBg = new SolidBrush(Color.FromArgb(220, 0, 80, 0)))
                    g.FillRectangle(winBg, boxX, boxY, boxW, boxH);
                using (Pen border = new Pen(Color.FromArgb(255, 255, 215, 0), 3))
                    g.DrawRectangle(border, boxX, boxY, boxW, boxH);

                g.DrawString(msg, fontWin, Brushes.Black, (W - ms.Width) / 2 + 2, boxY + 15);
                g.DrawString(msg, fontWin, Brushes.Gold, (W - ms.Width) / 2, boxY + 13);
                g.DrawString(s2t, fontSub, Brushes.White, (W - s2s.Width) / 2, boxY + 80);
                g.DrawString(s3t, fontScore2, Brushes.Yellow, (W - s3s.Width) / 2, boxY + 120);
                g.DrawString(sub, fontSub, Brushes.Cyan, (W - ss.Width) / 2, boxY + 170);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private void FlappyBirdForm_Load(object sender, EventArgs e) { }
    }

    public class Pipe
    {
        public int X;
        public int GapY;
        public bool IsTop;
        public bool Passed;
    }
}