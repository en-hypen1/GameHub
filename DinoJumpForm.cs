using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace GameHub
{
    public partial class DinoJumpForm : Form
    {
        bool goLeft, goRight, jumping;
        int jumpSpeed = 0;
        int force = 14;
        int playerSpeed = 8;
        int score = 0;
        int lastPlatformY;
        int gameDistance = 0;
        int finishDistance = 4000;
        bool gameStarted = false;
        bool gameFinished = false;

        // Pozicioni fiks i lojtarit në ekran (në mes)
        int playerScreenX = 200; // Dino do të jetë fiks në këtë pozicion

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rnd = new Random();

        Rectangle player;
        Rectangle ground;

        List<Rectangle> platforms = new List<Rectangle>();
        List<Rectangle> coins = new List<Rectangle>();
        List<Rectangle> enemies = new List<Rectangle>();

        // Audio components
        SoundPlayer jumpSound;
        SoundPlayer coinSound;
        SoundPlayer gameOverSound;
        SoundPlayer winSound;

        // Start Panel
        Panel startPanel;
        Label startTitle;
        Label startMessage;
        Button startButton;

        public DinoJumpForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.ClientSize = new Size(1200, 700);
            this.Text = "Dino Adventure";
            this.BackColor = Color.SkyBlue;

            // Lojtari në pozicion fiks horizontal
            player = new Rectangle(playerScreenX, 300, 55, 55);

            LoadAudioFiles();
            CreateLevel();
            CreateStartScreen();

            gameTimer.Interval = 20;
            gameTimer.Tick += GameLoop;

            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
            this.KeyPreview = true;
        }

        void LoadAudioFiles()
        {
            try
            {
                jumpSound = new SoundPlayer();
                coinSound = new SoundPlayer();
                gameOverSound = new SoundPlayer();
                winSound = new SoundPlayer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio not available: " + ex.Message);
            }
        }

        void PlayJumpSound()
        {
            try
            {
                System.Media.SystemSounds.Beep.Play();
            }
            catch { }
        }

        void PlayCoinSound()
        {
            try
            {
                Console.Beep(1000, 100); // Beep i shkurtër për të mos ndaluar lojën
            }
            catch { }
        }

        void PlayGameOverSound()
        {
            try
            {
                Console.Beep(300, 800);
                System.Threading.Thread.Sleep(100);
                Console.Beep(250, 600);
            }
            catch { }
        }

        void PlayWinSound()
        {
            try
            {
                int[] notes = { 523, 587, 659, 698, 784, 880 };
                foreach (int note in notes)
                {
                    Console.Beep(note, 150);
                    System.Threading.Thread.Sleep(50);
                }
            }
            catch { }
        }

        void CreateStartScreen()
        {
            startPanel = new Panel();
            startPanel.Size = new Size(500, 300);
            startPanel.Location = new Point((this.ClientSize.Width - 500) / 2, (this.ClientSize.Height - 300) / 2);
            startPanel.BackColor = Color.FromArgb(200, 0, 0, 0);
            startPanel.BorderStyle = BorderStyle.FixedSingle;

            startTitle = new Label();
            startTitle.Text = "🦖 DINO ADVENTURE 🦖";
            startTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            startTitle.ForeColor = Color.Gold;
            startTitle.BackColor = Color.Transparent;
            startTitle.Size = new Size(450, 50);
            startTitle.Location = new Point(25, 30);
            startTitle.TextAlign = ContentAlignment.MiddleCenter;
            startPanel.Controls.Add(startTitle);

            startMessage = new Label();
            startMessage.Text = "🏃‍♂️ PËRSHKRIMI I LOJËS:\n\n" +
                               "• Mblidh monedhat e arta për pikë\n" +
                               "• Shmang pengesat e kuqe\n" +
                               "• Përdor platformat fluturuese\n" +
                               "• Arri në vijën e finish-it\n\n" +
                               "🎮 KONTROLLET:\n" +
                               "• ← → : Lëviz majtas/djathtas\n" +
                               "• SPACE : Kërce\n\n" +
                               "⬇️ SHTYP ENTER PËR TË FILLUAR ⬇️";
            startMessage.Font = new Font("Arial", 12);
            startMessage.ForeColor = Color.White;
            startMessage.BackColor = Color.Transparent;
            startMessage.Size = new Size(450, 210);
            startMessage.Location = new Point(25, 85);
            startPanel.Controls.Add(startMessage);

            startButton = new Button();
            startButton.Text = "🎮 FILLO LOJËN 🎮";
            startButton.Font = new Font("Arial", 14, FontStyle.Bold);
            startButton.BackColor = Color.Green;
            startButton.ForeColor = Color.White;
            startButton.Size = new Size(200, 40);
            startButton.Location = new Point(150, 250);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Click += StartButton_Click;
            startPanel.Controls.Add(startButton);

            this.Controls.Add(startPanel);
            startPanel.BringToFront();
        }

        void StartButton_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        void StartGame()
        {
            gameStarted = true;
            gameFinished = false;
            if (startPanel != null)
            {
                this.Controls.Remove(startPanel);
                startPanel.Dispose();
            }
            gameTimer.Start();
        }

        public DinoJumpForm(Form parent) : this()
        {
            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else if (parent.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void DinoJumpForm_Load(object sender, EventArgs e)
        {

        }

        void CreateLevel()
        {
            ground = new Rectangle(0, this.ClientSize.Height - 50, this.ClientSize.Width + finishDistance + 1000, 45);
            lastPlatformY = ground.Y - 80;
        }

        void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || gameFinished) return;

            if (gameDistance >= finishDistance)
            {
                gameFinished = true;
                gameTimer.Stop();
                PlayWinSound();
                ShowWinDialog();
                return;
            }

            // LOJTARI NUK LËVIZET HORIZONTALISHT MË!
            // Vetëm objektet lëvizin dhe krijojnë iluzionin e lëvizjes

            // FIZIKA E KËRCIMIT (VETËM VERTIKAL)
            player.Y += jumpSpeed;

            if (jumping && force > 0)
            {
                jumpSpeed = -12;
                force--;
            }
            else
            {
                jumpSpeed = 10;
            }

            // COLLISION ME TOKË
            if (player.IntersectsWith(ground) && jumpSpeed >= 0 && player.Bottom >= ground.Y)
            {
                force = 14;
                player.Y = ground.Y - player.Height;
                jumpSpeed = 0;
                jumping = false;
            }

            if (player.Y + player.Height > ground.Y)
            {
                player.Y = ground.Y - player.Height;
                jumping = false;
                jumpSpeed = 0;
            }

            if (player.Y < 0)
                player.Y = 0;

            // LËVIZJA E PLATFORMAVE (TË GJITHA OBJEKTET LËVIZIN MAJTAS)
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                var p = platforms[i];
                p.X -= 5;

                if (player.IntersectsWith(p) && jumpSpeed >= 0 && player.Bottom <= p.Y + 10)
                {
                    force = 14;
                    player.Y = p.Y - player.Height;
                    jumpSpeed = 0;
                    jumping = false;
                }

                if (p.Right < 0)
                    platforms.RemoveAt(i);
                else
                    platforms[i] = p;
            }

            // MONEDHAT - LËVIZIN MAJTAS
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                var c = coins[i];
                c.X -= 5;

                if (player.IntersectsWith(c))
                {
                    coins.RemoveAt(i);
                    score += 10;
                    PlayCoinSound();
                    continue;
                }

                if (c.Right < 0)
                    coins.RemoveAt(i);
                else
                    coins[i] = c;
            }

            // ARMIQTË - LËVIZIN MAJTAS
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var en = enemies[i];
                en.X -= 6;

                if (player.IntersectsWith(en))
                {
                    gameTimer.Stop();
                    PlayGameOverSound();
                    ShowGameOverDialog();
                    return;
                }

                if (en.Right < 0)
                    enemies.RemoveAt(i);
                else
                    enemies[i] = en;
            }

            // SPAWN OBJEKTEVE
            SpawnObjects();

            // Rrit distancën e udhëtimit
            gameDistance += 5;

            this.Invalidate();
        }

        void SpawnObjects()
        {
            // Platformat
            if (rnd.Next(0, 100) < 2)
            {
                int change = rnd.Next(-40, 40);
                int newY = lastPlatformY + change;

                if (newY < 150) newY = 150;
                if (newY > ground.Y - 70) newY = ground.Y - 70;

                lastPlatformY = newY;

                bool canSpawn = true;
                foreach (var p in platforms)
                {
                    if (Math.Abs(p.X - this.ClientSize.Width) < 150)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn)
                    platforms.Add(new Rectangle(this.ClientSize.Width, newY, 110, 20));
            }

            // Monedhat
            if (rnd.Next(0, 100) < 4)
            {
                bool onPlatform = rnd.Next(0, 2) == 0 && platforms.Count > 0;
                int coinY;

                if (onPlatform && platforms.Count > 0)
                {
                    var platform = platforms[rnd.Next(platforms.Count)];
                    coinY = platform.Y - 30;
                }
                else
                {
                    coinY = ground.Y - 35;
                }

                bool canSpawnCoin = true;
                foreach (var c in coins)
                {
                    if (Math.Abs(c.X - this.ClientSize.Width) < 100)
                    {
                        canSpawnCoin = false;
                        break;
                    }
                }

                foreach (var en in enemies)
                {
                    if (Math.Abs(en.X - this.ClientSize.Width) < 80 && Math.Abs(en.Y - coinY) < 50)
                    {
                        canSpawnCoin = false;
                        break;
                    }
                }

                if (canSpawnCoin)
                {
                    coins.Add(new Rectangle(this.ClientSize.Width, coinY, 30, 30));
                }
            }

            // Armiqtë
            if (rnd.Next(0, 100) < 2)
            {
                int enemyY = ground.Y - 45;

                bool canSpawnEnemy = true;
                foreach (var c in coins)
                {
                    if (Math.Abs(c.X - this.ClientSize.Width) < 100 && Math.Abs(c.Y - enemyY) < 50)
                    {
                        canSpawnEnemy = false;
                        break;
                    }
                }

                foreach (var en in enemies)
                {
                    if (Math.Abs(en.X - this.ClientSize.Width) < 150)
                    {
                        canSpawnEnemy = false;
                        break;
                    }
                }

                if (canSpawnEnemy)
                {
                    enemies.Add(new Rectangle(this.ClientSize.Width, enemyY, 45, 45));
                }
            }
        }

        void ShowGameOverDialog()
        {
            DialogResult result = MessageBox.Show(
                "💀 GAME OVER 💀\n\n" +
                $"🎯 Score: {score}\n" +
                $"📏 Distance: {gameDistance}/{finishDistance}\n\n" +
                "Dëshiron të luash përsëri?",
                "Game Over",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                RestartGame();
            else
                this.Close();
        }

        void ShowWinDialog()
        {
            DialogResult result = MessageBox.Show(
                "🏆 VICTORY! 🏆\n\n" +
                $"✨ Score total: {score} ✨\n" +
                $"🎉 Ke përfunduar lojën me sukses! 🎉\n\n" +
                "Dëshiron të luash përsëri?",
                "YOU WIN!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                RestartGame();
            else
                this.Close();
        }

        void RestartGame()
        {
            platforms.Clear();
            coins.Clear();
            enemies.Clear();

            score = 0;
            gameDistance = 0;
            gameFinished = false;
            jumping = false;
            force = 14;
            jumpSpeed = 0;
            player.X = playerScreenX; // Pozicioni fiks në ekran
            player.Y = ground.Y - player.Height;

            CreateLevel();

            gameTimer.Start();

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.SkyBlue);

            // Vizatimi i tokës
            using (Brush groundBrush = new SolidBrush(Color.FromArgb(139, 69, 19)))
            {
                g.FillRectangle(groundBrush, ground);
            }

            using (Pen darkPen = new Pen(Color.FromArgb(101, 67, 33), 2))
            {
                for (int i = 0; i < ground.Width; i += 50)
                {
                    g.DrawLine(darkPen, ground.X + i, ground.Y + 10, ground.X + i + 30, ground.Y + 10);
                }
            }

            // Platformat
            foreach (var p in platforms)
            {
                using (Brush platformBrush = new SolidBrush(Color.FromArgb(160, 82, 45)))
                {
                    g.FillRectangle(platformBrush, p);
                }
                g.DrawRectangle(Pens.Brown, p);
            }

            // Monedhat
            foreach (var c in coins)
            {
                using (Brush goldBrush = new SolidBrush(Color.Gold))
                {
                    g.FillEllipse(goldBrush, c);
                }
                g.DrawEllipse(Pens.Orange, c);
                using (Font smallFont = new Font("Arial", 12, FontStyle.Bold))
                {
                    g.DrawString("$", smallFont, Brushes.DarkGoldenrod, c.X + 8, c.Y + 5);
                }
            }

            // Armiqtë
            foreach (var en in enemies)
            {
                using (Brush enemyBrush = new SolidBrush(Color.DarkRed))
                {
                    g.FillRectangle(enemyBrush, en);
                }
                g.DrawRectangle(Pens.Black, en);
                g.FillEllipse(Brushes.White, en.X + 10, en.Y + 10, 8, 8);
                g.FillEllipse(Brushes.White, en.X + 27, en.Y + 10, 8, 8);
                g.FillEllipse(Brushes.Black, en.X + 12, en.Y + 12, 4, 4);
                g.FillEllipse(Brushes.Black, en.X + 29, en.Y + 12, 4, 4);
            }

            // Lojtari (në pozicion fiks)
            using (Brush playerBrush = new SolidBrush(Color.Green))
            {
                g.FillRectangle(playerBrush, player);
            }
            g.DrawRectangle(Pens.DarkGreen, player);
            g.FillEllipse(Brushes.White, player.X + 35, player.Y + 15, 8, 8);
            g.FillEllipse(Brushes.White, player.X + 45, player.Y + 15, 8, 8);
            g.FillEllipse(Brushes.Black, player.X + 37, player.Y + 17, 4, 4);
            g.FillEllipse(Brushes.Black, player.X + 47, player.Y + 17, 4, 4);

            // Vizato thumbat e dino-s
            g.FillPolygon(Brushes.DarkGreen, new Point[] {
                new Point(player.X + 10, player.Y + 10),
                new Point(player.X + 20, player.Y + 5),
                new Point(player.X + 15, player.Y + 15)
            });

            // Score Panel
            using (Brush panelBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
            {
                g.FillRectangle(panelBrush, 10, 10, 220, 50);
            }
            g.DrawString($"💰 Score: {score}", new Font("Arial", 18, FontStyle.Bold), Brushes.Gold, 20, 20);

            // Distance Progress
            int progressWidth = (int)((double)gameDistance / finishDistance * 300);
            g.FillRectangle(Brushes.Gray, this.ClientSize.Width - 320, 20, 300, 20);
            g.FillRectangle(Brushes.Green, this.ClientSize.Width - 320, 20, progressWidth, 20);
            g.DrawString($"📏 {gameDistance}/{finishDistance}", new Font("Arial", 10, FontStyle.Bold), Brushes.White, this.ClientSize.Width - 310, 25);

            // Finish Line
            if (gameDistance > finishDistance - 800)
            {
                for (int i = 0; i < this.ClientSize.Height; i += 40)
                {
                    g.FillRectangle(Brushes.White, this.ClientSize.Width - 50, i, 15, 25);
                    g.FillRectangle(Brushes.Black, this.ClientSize.Width - 50, i + 25, 15, 25);
                }
                g.DrawString("🏁 FINISH 🏁", new Font("Arial", 14, FontStyle.Bold), Brushes.White, this.ClientSize.Width - 100, this.ClientSize.Height / 2 - 10);
            }

            if (!gameStarted && startPanel == null)
            {
                string msg = "SHTYP ENTER PËR TË FILLUAR";
                SizeF msgSize = g.MeasureString(msg, new Font("Arial", 24, FontStyle.Bold));
                g.DrawString(msg, new Font("Arial", 24, FontStyle.Bold), Brushes.White,
                    (this.ClientSize.Width - msgSize.Width) / 2, this.ClientSize.Height / 2);
            }
        }

        void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted && e.KeyCode == Keys.Enter)
            {
                StartGame();
                return;
            }

            if (!gameStarted) return;

            // VETËM KËRCIMI - PA LËVIZJE HORIZONTALE TË DINOS
            if (e.KeyCode == Keys.Space && !jumping && player.Y + player.Height >= ground.Y - 5)
            {
                jumping = true;
                force = 14;
                jumpSpeed = -12;
                PlayJumpSound();
            }
        }

        void KeyIsUp(object sender, KeyEventArgs e)
        {
            // Nuk kemi nevojë për goLeft/goRight më
            if (e.KeyCode == Keys.Space)
            {
                // Kërcimi vazhdon vetë
            }
        }
    }
}