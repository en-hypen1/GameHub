using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GameHub
{
    public partial class DinoJumpForm : Form
    {
        // ── TINGUJ ──────────────────────────────────────────────
        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);

        void PlayJump() { try { System.Threading.Tasks.Task.Run(() => Beep(600, 60)); } catch { } }
        void PlayCoin() { try { System.Threading.Tasks.Task.Run(() => { Beep(1047, 60); Beep(1319, 60); }); } catch { } }
        void PlayDie() { try { System.Threading.Tasks.Task.Run(() => { Beep(300, 120); Beep(200, 200); }); } catch { } }
        void PlayWin() { try { System.Threading.Tasks.Task.Run(() => { Beep(523, 80); Beep(659, 80); Beep(784, 80); Beep(1047, 200); }); } catch { } }

        // ── KONSTANTET ──────────────────────────────────────────
        const int DINO_W = 52;
        const int DINO_H = 64;
        const int DINO_SCREEN_X = 180;
        const int FINISH_DIST = 18000;  // 3x më gjatë!
        const int WORLD_SPEED = 6;

        // Këto llogariten dinamikisht sipas madhësisë së formës
        int SCREEN_W => this.ClientSize.Width;
        int SCREEN_H => this.ClientSize.Height;
        int GROUND_Y => this.ClientSize.Height - 100;

        // ── GJENDJA ─────────────────────────────────────────────
        float dinoY = 400;
        float dinoVY = 0;
        float gravity = 0.7f;
        float jumpForce = -16f;
        bool onGround = true;
        bool doubleJump = false;
        int legAnim = 0;

        int score = 0;
        int worldOffset = 0;
        bool gameStarted = false;
        bool isGameOver = false;
        bool hasWon = false;
        bool isPaused = false;
        int flashTimer = 0;

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rnd = new Random();

        struct Platform { public float X, Y, W; }
        struct Coin { public float X, Y; public bool Taken; }
        struct Enemy { public float X, Y; public int Type; } // 0=kaktus,1=zog
        struct Particle { public float X, Y, VX, VY, Life; public Color Color; }

        List<Platform> platforms = new List<Platform>();
        List<Coin> coins = new List<Coin>();
        List<Enemy> enemies = new List<Enemy>();
        List<Particle> particles = new List<Particle>();

        int spawnPlatTimer = 0;
        int spawnCoinTimer = 0;
        int spawnEnemTimer = 0;

        Font fontBig = new Font("Arial", 32, FontStyle.Bold);
        Font fontMed = new Font("Arial", 18, FontStyle.Bold);
        Font fontSmall = new Font("Arial", 13, FontStyle.Bold);
        Font fontTiny = new Font("Arial", 10);

        private Form _parentForm;

        public DinoJumpForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Text = "Dino Adventure";
            this.BackColor = Color.SkyBlue;
            this.FormClosing += DinoJumpForm_FormClosing;
            this.KeyDown += DinoJumpForm_KeyDown;
            this.KeyUp += DinoJumpForm_KeyUp;
            this.Resize += (s, e) => this.Invalidate();

            // Fillo maximized
            this.WindowState = FormWindowState.Maximized;

            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;

            // Platformë fillestare e gjatë
            platforms.Add(new Platform { X = -200, Y = 0, W = 99999 }); // toka bazë - Y llogaritet në OnPaint
        }

        public DinoJumpForm(Form parent) : this()
        {
            _parentForm = parent;
        }

        private void DinoJumpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            fontBig?.Dispose(); fontMed?.Dispose();
            fontSmall?.Dispose(); fontTiny?.Dispose();
            _parentForm?.Show();
        }

        // ── GAME LOOP ────────────────────────────────────────────
        void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || isGameOver || hasWon || isPaused)
            { this.Invalidate(); return; }

            int gY = GROUND_Y;

            // Fizika
            dinoVY += gravity;
            dinoY += dinoVY;

            if (dinoY >= gY - DINO_H)
            {
                dinoY = gY - DINO_H;
                dinoVY = 0;
                onGround = true;
                doubleJump = false;
            }

            if (onGround) legAnim = (legAnim + 1) % 20;
            else legAnim = 0;

            if (flashTimer > 0) flashTimer--;

            worldOffset += WORLD_SPEED;

            // Shpejtësia e kaktuasve rritet me kohën
            float difficulty = 1f + worldOffset / 10000f; // bëhet 2.8x harder në fund

            // ── PLATFORMAT ──
            spawnPlatTimer++;
            if (spawnPlatTimer > rnd.Next(100, 180))
            {
                spawnPlatTimer = 0;
                float platY = gY - rnd.Next(90, 220);
                float platW = rnd.Next(120, 260); // platforma MË TË GJERA
                platforms.Add(new Platform { X = SCREEN_W + worldOffset, Y = platY, W = platW });
            }

            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                var p = platforms[i];
                if (p.W > 1000) continue; // toka bazë - skip collision (bëhet nga gY)

                float px = p.X - worldOffset;

                float dinoL = DINO_SCREEN_X + 6;
                float dinoR = DINO_SCREEN_X + DINO_W - 6;
                float dinoB = dinoY + DINO_H;
                float dinoT = dinoY;

                if (dinoVY >= 0 &&
                    dinoR > px && dinoL < px + p.W &&
                    dinoB > p.Y && dinoB < p.Y + 22 &&
                    dinoT < p.Y)
                {
                    dinoY = p.Y - DINO_H;
                    dinoVY = 0;
                    onGround = true;
                    doubleJump = false;
                }

                if (px + p.W < -150) platforms.RemoveAt(i);
            }

            // ── COINS ──
            spawnCoinTimer++;
            if (spawnCoinTimer > rnd.Next(25, 60))
            {
                spawnCoinTimer = 0;
                float cy = rnd.Next(0, 3) == 0
                    ? gY - rnd.Next(120, 230)
                    : gY - 55;
                coins.Add(new Coin { X = SCREEN_W + worldOffset, Y = cy });
            }

            for (int i = coins.Count - 1; i >= 0; i--)
            {
                var c = coins[i];
                if (c.Taken) { coins.RemoveAt(i); continue; }
                float cx = c.X - worldOffset;

                if (cx + 16 > DINO_SCREEN_X + 8 && cx < DINO_SCREEN_X + DINO_W - 8 &&
                    c.Y + 16 > dinoY + 8 && c.Y < dinoY + DINO_H - 8)
                {
                    c.Taken = true;
                    coins[i] = c;
                    score += 10;
                    PlayCoin();
                    for (int k = 0; k < 6; k++)
                        particles.Add(new Particle
                        {
                            X = cx + 8,
                            Y = c.Y + 8,
                            VX = (float)(rnd.NextDouble() - 0.5) * 6,
                            VY = (float)(rnd.NextDouble() - 0.5) * 6,
                            Life = 1f,
                            Color = Color.Gold
                        });
                    continue;
                }
                if (cx < -50) coins.RemoveAt(i);
            }

            // ── ARMIQTË - MË SHUMË KAKTUSA ──
            spawnEnemTimer++;
            // Interval zvogëlohet me difficulty - kaktusa dalin MË SHPESH
            int spawnInterval = (int)(Math.Max(30, 90 / difficulty));
            if (spawnEnemTimer > spawnInterval)
            {
                spawnEnemTimer = 0;
                // 70% kaktus, 30% zog
                int type = rnd.Next(0, 10) < 7 ? 0 : 1;
                float ey = type == 0
                    ? gY - 55
                    : gY - rnd.Next(160, 300);

                // Ndonjëherë shfaqen 2 kaktusa radhazi!
                enemies.Add(new Enemy { X = SCREEN_W + worldOffset, Y = ey, Type = type });
                if (type == 0 && rnd.Next(0, 3) == 0 && difficulty > 1.5f)
                    enemies.Add(new Enemy
                    {
                        X = SCREEN_W + worldOffset + 70,
                        Y = ey,
                        Type = 0
                    }); // kaktus çift
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var en = enemies[i];
                float ex = en.X - worldOffset;

                int ew = en.Type == 0 ? 38 : 44;
                int eh = en.Type == 0 ? 55 : 28;

                if (ex + ew > DINO_SCREEN_X + 10 && ex < DINO_SCREEN_X + DINO_W - 10 &&
                    en.Y + eh > dinoY + 10 && en.Y < dinoY + DINO_H - 6)
                {
                    flashTimer = 10;
                    isGameOver = true;
                    gameTimer.Stop();
                    PlayDie();
                    this.Invalidate();
                    return;
                }

                if (ex < -100) enemies.RemoveAt(i);
            }

            // Particles
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];
                p.X += p.VX; p.Y += p.VY;
                p.VY += 0.2f; p.Life -= 0.05f;
                if (p.Life <= 0) particles.RemoveAt(i);
                else particles[i] = p;
            }

            score += 1;

            if (worldOffset >= FINISH_DIST)
            {
                hasWon = true;
                score += 2000;
                gameTimer.Stop();
                PlayWin();
            }

            this.Invalidate();
        }

        // ── VIZATIMI ─────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int gY = GROUND_Y;

            // SKY
            using (var sky = new LinearGradientBrush(
                new Rectangle(0, 0, SCREEN_W, SCREEN_H),
                Color.FromArgb(135, 206, 250), Color.FromArgb(200, 230, 255), 90f))
                g.FillRectangle(sky, 0, 0, SCREEN_W, SCREEN_H);

            DrawClouds(g);

            // PLATFORMAT
            foreach (var p in platforms)
            {
                if (p.W > 1000) continue;
                float px = p.X - worldOffset;
                if (px > SCREEN_W + 50 || px + p.W < -50) continue;

                using (var brush = new LinearGradientBrush(
                    new RectangleF(px, p.Y, p.W, 20),
                    Color.FromArgb(101, 67, 33), Color.FromArgb(160, 100, 50), 90f))
                    g.FillRectangle(brush, px, p.Y, p.W, 20);
                g.DrawRectangle(new Pen(Color.FromArgb(80, 40, 10), 1), px, p.Y, p.W, 20);
                g.FillRectangle(new SolidBrush(Color.FromArgb(80, 180, 60)), px, p.Y - 5, p.W, 7);
            }

            DrawGround(g, gY);

            // COINS
            foreach (var c in coins)
            {
                float cx = c.X - worldOffset;
                if (cx < -30 || cx > SCREEN_W + 30) continue;
                DrawCoin(g, cx, c.Y);
            }

            // ARMIQTË
            foreach (var en in enemies)
            {
                float ex = en.X - worldOffset;
                if (ex < -80 || ex > SCREEN_W + 80) continue;
                if (en.Type == 0) DrawCactus(g, ex, en.Y);
                else DrawBird(g, ex, en.Y);
            }

            // PARTICLES
            foreach (var p in particles)
            {
                int alpha = (int)(p.Life * 255);
                using (var b = new SolidBrush(Color.FromArgb(alpha, p.Color)))
                    g.FillEllipse(b, p.X - 4, p.Y - 4, 8, 8);
            }

            // DINO
            if (!(flashTimer > 0 && flashTimer % 2 == 0))
                DrawDino(g, DINO_SCREEN_X, (int)dinoY, onGround, legAnim, isGameOver);

            DrawFinishLine(g, gY);
            DrawUI(g, gY);

            if (!gameStarted) DrawStartScreen(g);
            if (isPaused) DrawPauseScreen(g);
            if (isGameOver) DrawGameOverScreen(g);
            if (hasWon) DrawWinScreen(g);
        }

        // ── DINO ────────────────────────────────────────────────
        void DrawDino(Graphics g, int x, int y, bool onGnd, int anim, bool dead)
        {
            Color bodyColor = dead ? Color.Gray : Color.FromArgb(80, 180, 80);
            Color darkColor = dead ? Color.DimGray : Color.FromArgb(50, 130, 50);
            Color bellyColor = dead ? Color.LightGray : Color.FromArgb(180, 230, 150);

            // Bishti
            PointF[] tail = {
                new PointF(x + 4,  y + 42),
                new PointF(x - 18, y + 52),
                new PointF(x - 10, y + 38),
                new PointF(x + 8,  y + 36),
            };
            g.FillPolygon(new SolidBrush(darkColor), tail);

            // Trupi
            g.FillEllipse(new SolidBrush(bodyColor), x + 2, y + 22, DINO_W - 8, DINO_H - 30);
            g.FillEllipse(new SolidBrush(bellyColor), x + 14, y + 30, 24, 20);

            // Koka
            g.FillEllipse(new SolidBrush(bodyColor), x + 24, y, 26, 26);
            PointF[] jaw = {
                new PointF(x + 26, y + 18),
                new PointF(x + 50, y + 18),
                new PointF(x + 52, y + 26),
                new PointF(x + 26, y + 24),
            };
            g.FillPolygon(new SolidBrush(bodyColor), jaw);
            g.FillEllipse(new SolidBrush(bellyColor), x + 30, y + 16, 16, 10);

            // Sy
            if (dead)
            {
                using (var p = new Pen(Color.Red, 2))
                {
                    g.DrawLine(p, x + 33, y + 5, x + 38, y + 10);
                    g.DrawLine(p, x + 38, y + 5, x + 33, y + 10);
                }
            }
            else
            {
                g.FillEllipse(Brushes.White, x + 32, y + 4, 10, 10);
                g.FillEllipse(Brushes.Black, x + 34, y + 6, 6, 6);
                g.FillEllipse(Brushes.White, x + 35, y + 6, 2, 2);
            }

            g.FillEllipse(new SolidBrush(darkColor), x + 44, y + 10, 3, 3);
            g.FillEllipse(new SolidBrush(darkColor), x + 20, y + 28, 14, 10);
            g.FillEllipse(new SolidBrush(darkColor), x + 28, y + 35, 8, 6);

            // Thumbat shpinë
            // Pas - thumbat lëvizin 20px më lart (y + 2 dhe y - 8):
            for (int i = 4; i > 0; i--)
            {
                PointF[] spike = {
        new PointF(x + 18 + i * 6, y + 2),   // ishte y + 22
        new PointF(x + 21 + i * 6, y - 8),   // ishte y + 12
        new PointF(x + 24 + i * 6, y + 2),   // ishte y + 22
    };
                g.FillPolygon(new SolidBrush(darkColor), spike);
            }

            // Këmbët
            if (onGnd)
            {
                int l1 = (anim < 10) ? 6 : -2;
                int l2 = (anim < 10) ? -2 : 6;
                DrawLeg(g, x + 8, y + 50, l1, darkColor, bodyColor);
                DrawLeg(g, x + 20, y + 50, l2, darkColor, bodyColor);
            }
            else
            {
                DrawLeg(g, x + 8, y + 50, -4, darkColor, bodyColor);
                DrawLeg(g, x + 20, y + 50, -4, darkColor, bodyColor);
            }
        }

        void DrawLeg(Graphics g, float x, float y, int off, Color dark, Color main)
        {
            g.FillEllipse(new SolidBrush(main), x, y + off, 12, 18);
            g.FillEllipse(new SolidBrush(dark), x + 1, y + off + 14, 10, 10);
            PointF[] foot = {
                new PointF(x,      y + off + 20),
                new PointF(x + 12, y + off + 20),
                new PointF(x + 16, y + off + 28),
                new PointF(x - 2,  y + off + 28),
            };
            g.FillPolygon(new SolidBrush(dark), foot);
        }

        void DrawCactus(Graphics g, float x, float y)
        {
            Color c = Color.FromArgb(34, 139, 34);
            Color dc = Color.FromArgb(20, 100, 20);
            g.FillRectangle(new SolidBrush(c), x + 12, y, 16, 56);
            g.FillRectangle(new SolidBrush(c), x, y + 16, 12, 10);
            g.FillRectangle(new SolidBrush(c), x, y + 8, 10, 20);
            g.FillRectangle(new SolidBrush(c), x + 28, y + 16, 12, 10);
            g.FillRectangle(new SolidBrush(c), x + 30, y + 8, 10, 20);
            using (var p = new Pen(dc, 1.5f))
            {
                g.DrawLine(p, x + 12, y + 10, x + 8, y + 6);
                g.DrawLine(p, x + 28, y + 10, x + 32, y + 6);
                g.DrawLine(p, x + 20, y, x + 20, y - 6);
            }
        }

        void DrawBird(Graphics g, float x, float y)
        {
            g.FillEllipse(Brushes.DarkSlateGray, x + 6, y + 4, 30, 20);
            bool wingUp = (worldOffset / 8) % 2 == 0;
            PointF[] wing = wingUp
                ? new[] { new PointF(x + 10, y + 8), new PointF(x + 28, y + 8), new PointF(x + 20, y - 10) }
                : new[] { new PointF(x + 10, y + 14), new PointF(x + 28, y + 14), new PointF(x + 20, y + 26) };
            g.FillPolygon(Brushes.SlateGray, wing);
            g.FillEllipse(Brushes.DarkSlateGray, x + 28, y, 18, 16);
            g.FillEllipse(Brushes.White, x + 36, y + 3, 6, 6);
            g.FillEllipse(Brushes.Black, x + 37, y + 4, 4, 4);
            PointF[] beak = {
                new PointF(x + 46, y + 8),
                new PointF(x + 54, y + 10),
                new PointF(x + 46, y + 12),
            };
            g.FillPolygon(Brushes.Orange, beak);
        }

        void DrawCoin(Graphics g, float x, float y)
        {
            using (var b = new LinearGradientBrush(
                new RectangleF(x, y, 22, 22), Color.Gold, Color.DarkGoldenrod, 45f))
                g.FillEllipse(b, x, y, 22, 22);
            g.DrawEllipse(new Pen(Color.Orange, 1.5f), x, y, 22, 22);
            g.DrawString("$", fontTiny, Brushes.White, x + 5, y + 4);
        }

        void DrawGround(Graphics g, int gY)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, 180, 60)),
                0, gY - 8, SCREEN_W, 12);
            using (var b = new LinearGradientBrush(
                new Rectangle(0, gY, SCREEN_W, SCREEN_H - gY),
                Color.FromArgb(160, 100, 50), Color.FromArgb(101, 67, 33), 90f))
                g.FillRectangle(b, 0, gY, SCREEN_W, SCREEN_H - gY);

            int off = worldOffset % 60;
            using (var p = new Pen(Color.FromArgb(120, 80, 30), 1))
                for (int i = -60; i < SCREEN_W + 60; i += 60)
                    g.DrawLine(p, i - off, gY + 8, i - off + 40, gY + 8);
        }

        void DrawClouds(Graphics g)
        {
            int[] cloudX = { 100, 350, 600, 850, 1100, 1350 };
            int[] cloudY = { 60, 90, 50, 80, 70, 100 };
            for (int i = 0; i < cloudX.Length; i++)
            {
                float cx = ((cloudX[i] - worldOffset * 0.2f) % (SCREEN_W + 200) + SCREEN_W + 200)
                           % (SCREEN_W + 200) - 100;
                DrawCloud(g, cx, cloudY[i]);
            }
        }

        void DrawCloud(Graphics g, float x, float y)
        {
            using (var b = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
            {
                g.FillEllipse(b, x, y, 80, 44);
                g.FillEllipse(b, x + 22, y - 16, 48, 36);
                g.FillEllipse(b, x + 45, y, 55, 38);
            }
        }

        void DrawFinishLine(Graphics g, int gY)
        {
            float fx = FINISH_DIST - worldOffset + DINO_SCREEN_X;
            if (fx < -50 || fx > SCREEN_W + 50) return;

            for (int i = 0; i < SCREEN_H; i += 20)
            {
                bool white = (i / 20) % 2 == 0;
                g.FillRectangle(white ? Brushes.White : Brushes.Black, fx, i, 18, 20);
            }
            using (var p = new Pen(Color.White, 4))
                g.DrawLine(p, fx + 9, 0, fx + 9, 110);

            PointF[] flag = {
                new PointF(fx + 9, 12),
                new PointF(fx + 55, 30),
                new PointF(fx + 9, 48),
            };
            g.FillPolygon(Brushes.Red, flag);

            float dist = FINISH_DIST - worldOffset;
            if (dist > 0 && dist < 700)
            {
                SizeF ts = g.MeasureString("FINISH!", fontMed);
                g.DrawString("FINISH!", fontMed, Brushes.Black, fx - ts.Width / 2 + 1, 58);
                g.DrawString("FINISH!", fontMed, Brushes.Yellow, fx - ts.Width / 2, 56);
            }
        }

        // ── UI ───────────────────────────────────────────────────
        void DrawUI(Graphics g, int gY)
        {
            // Score panel
            using (var b = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.FillRectangle(b, 8, 8, 230, 58);
            g.DrawString($"SCORE: {score}", fontSmall, Brushes.Gold, 15, 14);
            g.DrawString($"COINS: {score / 10}", fontSmall, Brushes.Yellow, 15, 36);

            // EXIT BUTTON
            Rectangle exitBtn = new Rectangle(SCREEN_W - 145, 8, 135, 38);
            using (var b = new SolidBrush(Color.FromArgb(200, 160, 30, 30)))
                g.FillRectangle(b, exitBtn);
            g.DrawRectangle(Pens.Red, exitBtn);
            SizeF es = g.MeasureString("EXIT TO MENU", fontTiny);
            g.DrawString("EXIT TO MENU", fontTiny, Brushes.White,
                exitBtn.X + (exitBtn.Width - es.Width) / 2,
                exitBtn.Y + (exitBtn.Height - es.Height) / 2);

            // PAUSE BUTTON
            Rectangle pauseBtn = new Rectangle(SCREEN_W - 290, 8, 135, 38);
            using (var b = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                g.FillRectangle(b, pauseBtn);
            g.DrawRectangle(isPaused ? Pens.LimeGreen : Pens.White, pauseBtn);
            string pl = isPaused ? "▶ RESUME" : "⏸ PAUSE";
            SizeF ps2 = g.MeasureString(pl, fontTiny);
            g.DrawString(pl, fontTiny, isPaused ? Brushes.LimeGreen : Brushes.White,
                pauseBtn.X + (pauseBtn.Width - ps2.Width) / 2,
                pauseBtn.Y + (pauseBtn.Height - ps2.Height) / 2);

            // Progress bar
            int barW = SCREEN_W - 600;
            int barX = 250;
            int barY = 16;
            float prog = Math.Min(1f, (float)worldOffset / FINISH_DIST);
            int fill = (int)(barW * prog);

            using (var b = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                g.FillRectangle(b, barX, barY, barW, 22);
            if (fill > 0)
            {
                Color fc = prog < 0.5f
                    ? Color.FromArgb(50, (int)(255 * prog * 2), 50)
                    : Color.FromArgb((int)(255 * (prog - 0.5f) * 2), 200, 50);
                using (var b = new SolidBrush(fc))
                    g.FillRectangle(b, barX, barY, fill, 22);
            }
            g.DrawRectangle(Pens.White, barX, barY, barW, 22);

            using (var f = new Font("Arial", 9))
            {
                g.DrawString("🦖", f, Brushes.White, barX + fill - 10, barY - 14);
                g.DrawString("🏁", f, Brushes.White, barX + barW + 2, barY - 2);
            }
            string pct = $"{(int)(prog * 100)}%";
            SizeF pts = g.MeasureString(pct, fontTiny);
            g.DrawString(pct, fontTiny, Brushes.White,
                barX + (barW - pts.Width) / 2, barY + 4);

            // Difficulty tregues
            float diff = 1f + worldOffset / 10000f;
            string dlbl = diff < 1.5f ? "EASY" : diff < 2f ? "MEDIUM" : diff < 2.5f ? "HARD" : "EXTREME!";
            Color dc = diff < 1.5f ? Color.LimeGreen : diff < 2f ? Color.Yellow : diff < 2.5f ? Color.Orange : Color.Red;
            g.DrawString($"⚡ {dlbl}", fontTiny, new SolidBrush(dc), barX + barW / 2 - 25, barY + 26);

            // Kontrollet
            using (var b = new SolidBrush(Color.FromArgb(140, 0, 0, 0)))
                g.FillRectangle(b, 8, SCREEN_H - 28, 380, 22);
            g.DrawString("SPACE = Kërce  (2x = double jump)   P = Pause",
                fontTiny, Brushes.LightGray, 12, SCREEN_H - 26);
        }

        // ── MOUSE CLICK për butona ───────────────────────────────
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Rectangle exitBtn = new Rectangle(SCREEN_W - 145, 8, 135, 38);
            Rectangle pauseBtn = new Rectangle(SCREEN_W - 290, 8, 135, 38);

            if (exitBtn.Contains(e.Location))
            {
                gameTimer.Stop();
                _parentForm?.Show();
                this.Close();
                return;
            }

            if (pauseBtn.Contains(e.Location) && gameStarted && !isGameOver && !hasWon)
            {
                isPaused = !isPaused;
                this.Invalidate();
            }
        }

        // ── EKRANET ─────────────────────────────────────────────
        void DrawStartScreen(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.FillRectangle(b, 0, 0, SCREEN_W, SCREEN_H);

            int bw = 580, bh = 360;
            int bx = (SCREEN_W - bw) / 2, by = (SCREEN_H - bh) / 2;
            using (var b = new SolidBrush(Color.FromArgb(220, 20, 40, 20)))
                g.FillRectangle(b, bx, by, bw, bh);
            using (var p = new Pen(Color.Gold, 3))
                g.DrawRectangle(p, bx, by, bw, bh);

            string t = "🦖 DINO ADVENTURE 🦖";
            SizeF ts = g.MeasureString(t, fontBig);
            g.DrawString(t, fontBig, Brushes.Black, (SCREEN_W - ts.Width) / 2 + 2, by + 17);
            g.DrawString(t, fontBig, Brushes.Gold, (SCREEN_W - ts.Width) / 2, by + 15);

            DrawDino(g, SCREEN_W / 2 - 26, by + 85, true, (worldOffset / 3) % 20, false);

            string[] lines = {
                "• SPACE = Kërce  (dy herë = double jump!)",
                "• Mblidh monedhat ari  +10 pikë",
                "• Shmang kaktusat dhe zogjtë",
                "• Kaktusat bëhen MË TË SHPEJTË me kohën!",
                "• Arri në FINISH LINE pas 18000 hapa!",
                "",
                "▶  Shtyp ENTER ose SPACE për të filluar"
            };
            int ly = by + 178;
            foreach (var line in lines)
            {
                SizeF ls = g.MeasureString(line, fontSmall);
                Color lc = line.Contains("ENTER") ? Color.Yellow
                         : line.Contains("shpejtë") ? Color.OrangeRed
                         : line.Contains("monedhat") ? Color.Gold : Color.White;
                g.DrawString(line, fontSmall, new SolidBrush(lc),
                    (SCREEN_W - ls.Width) / 2, ly);
                ly += 26;
            }
        }

        void DrawPauseScreen(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                g.FillRectangle(b, 0, 0, SCREEN_W, SCREEN_H);
            SizeF ps = g.MeasureString("PAUSED", fontBig);
            g.DrawString("PAUSED", fontBig, Brushes.Black,
                (SCREEN_W - ps.Width) / 2 + 2, SCREEN_H / 2 - 42);
            g.DrawString("PAUSED", fontBig, Brushes.Orange,
                (SCREEN_W - ps.Width) / 2, SCREEN_H / 2 - 44);
            SizeF ss = g.MeasureString("Shtyp P ose klik RESUME", fontMed);
            g.DrawString("Shtyp P ose klik RESUME", fontMed, Brushes.White,
                (SCREEN_W - ss.Width) / 2, SCREEN_H / 2 + 10);
        }

        void DrawGameOverScreen(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                g.FillRectangle(b, 0, 0, SCREEN_W, SCREEN_H);

            int bw = 460, bh = 260;
            int bx = (SCREEN_W - bw) / 2, by = (SCREEN_H - bh) / 2;
            using (var b = new SolidBrush(Color.FromArgb(220, 60, 0, 0)))
                g.FillRectangle(b, bx, by, bw, bh);
            using (var p = new Pen(Color.Red, 2))
                g.DrawRectangle(p, bx, by, bw, bh);

            DrawDino(g, SCREEN_W / 2 - 26, by + 12, false, 0, true);

            SizeF gs = g.MeasureString("GAME OVER", fontBig);
            g.DrawString("GAME OVER", fontBig, Brushes.Black,
                (SCREEN_W - gs.Width) / 2 + 2, by + 88);
            g.DrawString("GAME OVER", fontBig, Brushes.Red,
                (SCREEN_W - gs.Width) / 2, by + 86);

            string sc = $"Score: {score}   Coins: {score / 10}";
            SizeF ss = g.MeasureString(sc, fontMed);
            g.DrawString(sc, fontMed, Brushes.Yellow,
                (SCREEN_W - ss.Width) / 2, by + 148);

            string rs = "Shtyp ENTER / SPACE për të rinisur";
            SizeF rss = g.MeasureString(rs, fontSmall);
            g.DrawString(rs, fontSmall, Brushes.LightGray,
                (SCREEN_W - rss.Width) / 2, by + 200);
        }

        void DrawWinScreen(Graphics g)
        {
            using (var b = new SolidBrush(Color.FromArgb(200, 0, 60, 0)))
                g.FillRectangle(b, 0, 0, SCREEN_W, SCREEN_H);

            int bw = 540, bh = 300;
            int bx = (SCREEN_W - bw) / 2, by = (SCREEN_H - bh) / 2;
            using (var b = new SolidBrush(Color.FromArgb(220, 0, 80, 0)))
                g.FillRectangle(b, bx, by, bw, bh);
            using (var p = new Pen(Color.Gold, 3))
                g.DrawRectangle(p, bx, by, bw, bh);

            string win = "🏆 YOU WIN! 🏆";
            SizeF ws = g.MeasureString(win, fontBig);
            g.DrawString(win, fontBig, Brushes.Black,
                (SCREEN_W - ws.Width) / 2 + 2, by + 17);
            g.DrawString(win, fontBig, Brushes.Gold,
                (SCREEN_W - ws.Width) / 2, by + 15);

            DrawDino(g, SCREEN_W / 2 - 26, by + 80, true, (worldOffset / 3) % 20, false);

            string sub = "Ke arritur në FINISH LINE!";
            SizeF ss = g.MeasureString(sub, fontMed);
            g.DrawString(sub, fontMed, Brushes.White,
                (SCREEN_W - ss.Width) / 2, by + 168);

            string sc = $"Score Final: {score}   Coins: {score / 10}";
            SizeF scs = g.MeasureString(sc, fontMed);
            g.DrawString(sc, fontMed, Brushes.Yellow,
                (SCREEN_W - scs.Width) / 2, by + 206);

            string rs = "Shtyp ENTER për të luajtur përsëri";
            SizeF rss = g.MeasureString(rs, fontSmall);
            g.DrawString(rs, fontSmall, Brushes.Cyan,
                (SCREEN_W - rss.Width) / 2, by + 252);
        }

        // ── INPUT ────────────────────────────────────────────────
        void DinoJumpForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space))
            {
                gameStarted = true;
                gameTimer.Start();
                return;
            }

            if ((isGameOver || hasWon) && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space))
            {
                RestartGame();
                return;
            }

            if (e.KeyCode == Keys.P && gameStarted && !isGameOver && !hasWon)
            {
                isPaused = !isPaused;
                this.Invalidate();
                return;
            }

            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.Up) &&
                gameStarted && !isGameOver && !hasWon && !isPaused)
            {
                if (onGround)
                {
                    dinoVY = jumpForce;
                    onGround = false;
                    doubleJump = true;
                    PlayJump();
                }
                else if (doubleJump)
                {
                    dinoVY = jumpForce * 0.85f;
                    doubleJump = false;
                    PlayJump();
                    for (int k = 0; k < 8; k++)
                        particles.Add(new Particle
                        {
                            X = DINO_SCREEN_X + DINO_W / 2,
                            Y = dinoY + DINO_H,
                            VX = (float)(rnd.NextDouble() - 0.5) * 5,
                            VY = (float)rnd.NextDouble() * 3 + 1,
                            Life = 1f,
                            Color = Color.LightGreen
                        });
                }
            }
        }

        void DinoJumpForm_KeyUp(object sender, KeyEventArgs e) { }

        void RestartGame()
        {
            platforms.Clear();
            coins.Clear();
            enemies.Clear();
            particles.Clear();

            platforms.Add(new Platform { X = -200, Y = 0, W = 99999 });

            dinoY = GROUND_Y - DINO_H;
            dinoVY = 0;
            onGround = true;
            doubleJump = false;
            score = 0;
            worldOffset = 0;
            isGameOver = false;
            hasWon = false;
            isPaused = false;
            flashTimer = 0;
            legAnim = 0;
            spawnPlatTimer = 0;
            spawnCoinTimer = 0;
            spawnEnemTimer = 0;

            gameTimer.Interval = 16;
            gameTimer.Start();
        }

        private void DinoJumpForm_Load(object sender, EventArgs e) { }
    }
}