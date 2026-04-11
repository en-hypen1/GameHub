using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameHub
{
    public partial class FlappyBirdForm : Form
    {
        //variablat qe na duhen 
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
        bool gameStarted = false;
        bool isGameOver = false;

        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        Random rand = new Random();

        Button backBtn;

        //konstruktori default
        public FlappyBirdForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.KeyPreview = true;

            pipeImage = Properties.Resources.pipe;
            pipeDownImage = Properties.Resources.pipe_down;
            birdImage = Properties.Resources.bird;

            gameTimer.Tick += GameLoop;

            // Event për pastrimin e burimeve kur mbyllet forma
            this.FormClosing += FlappyBirdForm_FormClosing;

            // Butoni Back
            backBtn = new Button();
            backBtn.Text = "← Back";
            backBtn.Size = new Size(80, 30);
            backBtn.Location = new Point(this.ClientSize.Width - backBtn.Width - 10, 10);
            backBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            backBtn.Click += (s, e) => { this.Close(); };
            this.Controls.Add(backBtn);
        }

        // Konstruktori me parent
        public FlappyBirdForm(Form parent)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.KeyPreview = true;

            gameTimer.Tick += GameLoop;
            pipeImage = Properties.Resources.pipe;
            pipeDownImage = Properties.Resources.pipe_down;
            birdImage = Properties.Resources.bird;

            this.FormClosing += FlappyBirdForm_FormClosing;

            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        // Metoda që pastron burimet kur forma mbyllet
        private void FlappyBirdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }
        }

        // Nuk kemi nevojë për Dispose override sepse ajo është në designer.cs
        // Thjesht sigurohemi që timeri të ndalet në FormClosing

        void StartGame()
        {
            birdY = 200;
            velocity = 0;
            score = 0;

            pipes.Clear();

            gameStarted = true;
            isGameOver = false;

            gameTimer.Interval = 20;
            gameTimer.Start();
            this.Invalidate();
        }

        void GameLoop(object sender, EventArgs e)
        {
            if (!gameStarted || isGameOver)
                return;

            // gravity
            velocity += gravity;
            birdY += velocity;

            // move pipes
            for (int i = 0; i < pipes.Count; i++)
            {
                pipes[i].Rect = new Rectangle(
                    pipes[i].Rect.X - pipeSpeed,
                    pipes[i].Rect.Y,
                    pipes[i].Rect.Width,
                    pipes[i].Rect.Height
                );

                // SCORE kur kalon zogun
                if (!pipes[i].Passed && !pipes[i].IsTop && pipes[i].Rect.Right < 100)
                {
                    score++;
                    pipes[i].Passed = true;
                }
            }

            // remove old pipes
            pipes.RemoveAll(p => p.Rect.Right < 0);

            // add new pipes
            if (pipes.Count == 0 || pipes[pipes.Count - 1].Rect.X < 300)
            {
                int gapY = rand.Next(100, 300);

                pipes.Add(new Pipe
                {
                    Rect = new Rectangle(500, 0, pipeWidth, gapY),
                    IsTop = true
                });

                pipes.Add(new Pipe
                {
                    Rect = new Rectangle(500, gapY + gapHeight, pipeWidth, 600),
                    IsTop = false
                });
            }

            CheckCollision();

            this.Invalidate();
        }

        void CheckCollision()
        {
            Rectangle bird = new Rectangle(100, birdY, 40, 40);

            foreach (var pipe in pipes)
            {
                if (bird.IntersectsWith(pipe.Rect))
                {
                    GameOver();
                }
            }

            if (birdY > this.Height || birdY < 0)
            {
                GameOver();
            }
        }

        void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // START ose RESTART
            if ((!gameStarted || isGameOver) && keyData == Keys.Enter)
            {
                StartGame();
                return true;
            }

            if (keyData == Keys.Space && !isGameOver && gameStarted)
            {
                velocity = jumpForce;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Vizato background të zi
            //g.Clear(Color.Black);

            // bird
            g.DrawImage(birdImage, 100, birdY, 60, 60);

            // pipes
            foreach (var pipe in pipes)
            {
                if (pipe.IsTop)
                {
                    g.DrawImage(pipeDownImage, pipe.Rect);
                }
                else
                {
                    g.DrawImage(pipeImage, pipe.Rect);
                }
            }

            // score
            g.DrawString("Score: " + score, new Font("Arial", 16), Brushes.White, 10, 10);

            if (!gameStarted)
            {
                string msg = "FLAPPY BIRD";
                string sub = "Press ENTER to Start";

                Font f1 = new Font("Arial", 28, FontStyle.Bold);
                Font f2 = new Font("Arial", 16);

                SizeF s = g.MeasureString(msg, f1);

                g.DrawString(msg, f1, Brushes.Yellow,
                    (this.Width - s.Width) / 2,
                    (this.Height - s.Height) / 2 - 50);

                g.DrawString(sub, f2, Brushes.White,
                    (this.Width - 200) / 2,
                    (this.Height / 2) + 20);

                // Instruksionet
                Font f3 = new Font("Arial", 12);
                g.DrawString("Press SPACE to jump", f3, Brushes.LightGray, 10, this.Height - 30);
            }

            if (isGameOver)
            {
                string msg = "GAME OVER";
                string sub = "Press ENTER to Replay";
                string scoreMsg = "Your Score: " + score;

                Font f1 = new Font("Arial", 28, FontStyle.Bold);
                Font f2 = new Font("Arial", 14);
                Font f3 = new Font("Arial", 18, FontStyle.Bold);

                SizeF s1 = g.MeasureString(msg, f1);
                SizeF s3 = g.MeasureString(scoreMsg, f3);

                g.DrawString(msg, f1, Brushes.Red,
                    (this.Width - s1.Width) / 2,
                    (this.Height - s1.Height) / 2 - 40);

                g.DrawString(scoreMsg, f3, Brushes.Yellow,
                    (this.Width - s3.Width) / 2,
                    (this.Height / 2) - 10);

                g.DrawString(sub, f2, Brushes.White,
                    (this.Width - 200) / 2,
                    (this.Height / 2) + 30);
            }
        }

        private void FlappyBirdForm_Load(object sender, EventArgs e)
        {
            // Sigurohemi që butoni back është në pozicionin e duhur
            if (backBtn != null)
            {
                backBtn.Location = new Point(this.ClientSize.Width - backBtn.Width - 10, 10);
            }
        }
    }

    public class Pipe
    {
        public Rectangle Rect;
        public bool IsTop;
        public bool Passed = false;
    }
}