using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameHub
{
    public partial class MemoryForm : Form
    {
        int timeLeft = 60;
        Label timeLabel;
        Button firstClicked = null;
        Button secondClicked = null;
        List<Button> matchedButtons = new List<Button>();
        Random rand = new Random();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer starTimer = new System.Windows.Forms.Timer(); // YJET
        private Form _parentForm;
        bool isGameActive = false;
        Form mainFormReference;
        bool isFormLoaded = false;
        bool isProcessingClick = false;

        List<Button> cardButtons = new List<Button>();

        // YJET - strukturë për çdo yll
        struct Star
        {
            public float X, Y, Size, Opacity, Speed;
        }
        List<Star> stars = new List<Star>();
        Panel starPanel; // paneli i yjeve, nën kartat

        List<Image> images = new List<Image>()
        {
            Properties.Resources.earth,   Properties.Resources.earth,
            Properties.Resources.mars,    Properties.Resources.mars,
            Properties.Resources.jupiter, Properties.Resources.jupiter,
            Properties.Resources.moon,    Properties.Resources.moon,
            Properties.Resources.sun,     Properties.Resources.sun,
            Properties.Resources.venus,   Properties.Resources.venus,
            Properties.Resources.neptune, Properties.Resources.neptune,
            Properties.Resources.saturn,  Properties.Resources.saturn
        };

        List<string> imageNames = new List<string>()
        {
            "earth",   "earth",
            "mars",    "mars",
            "jupiter", "jupiter",
            "moon",    "moon",
            "sun",     "sun",
            "venus",   "venus",
            "neptune", "neptune",
            "saturn",  "saturn"
        };

        List<int> shuffledIndices = new List<int>();

        public MemoryForm() : this(null) { }

        public MemoryForm(Form parent)
        {
            InitializeComponent();
            _parentForm = parent;
            mainFormReference = parent;
            this.BackColor = Color.FromArgb(10, 10, 40);
            this.ClientSize = new Size(600, 600);

            if (parent != null)
            {
                if (parent.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Maximized;
                else if (parent.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Minimized;
                else
                    this.WindowState = FormWindowState.Normal;
            }

            // ===== KRIJONI YJET =====
            CreateStarPanel();
            GenerateStars();

            starTimer.Interval = 50; // 20 FPS për animacion yje
            starTimer.Tick += StarTimer_Tick;
            starTimer.Start();
            // ========================

            timer.Interval = 750;
            timer.Tick += Timer_Tick;

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            // Time label
            timeLabel = new Label();
            timeLabel.Location = new Point(10, 10);
            timeLabel.Size = new Size(200, 30);
            timeLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            timeLabel.ForeColor = Color.White;
            timeLabel.BackColor = Color.Transparent;
            timeLabel.Text = "Time: 60";
            this.Controls.Add(timeLabel);
            timeLabel.BringToFront();

            // Restart button
            Button restartBtn = CreateStyledButton("🔄 Restart", new Size(120, 40));
            restartBtn.Location = new Point(this.ClientSize.Width - 140, 10);
            restartBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            restartBtn.Click += (s, e) => ResetGame();
            this.Controls.Add(restartBtn);
            restartBtn.BringToFront();

            // Exit button
            Button exitBtn = CreateStyledButton("Exit", new Size(80, 40));
            exitBtn.Location = new Point(this.ClientSize.Width - 230, 10);
            exitBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exitBtn.Click += (s, e) =>
            {
                StopAllTimers();
                this.Close();
            };
            this.Controls.Add(exitBtn);
            exitBtn.BringToFront();

            // Start button
            Button startBtn = CreateStyledButton("▶ Start Game", new Size(120, 40));
            startBtn.Location = new Point(this.ClientSize.Width / 2 - 60, this.ClientSize.Height - 80);
            startBtn.Anchor = AnchorStyles.Bottom;
            startBtn.Click += (s, e) =>
            {
                if (!isGameActive)
                {
                    ResetGame();
                    isGameActive = true;
                    startBtn.Visible = false;
                    EnableAllCards(true);
                    gameTimer.Start();
                }
            };
            this.Controls.Add(startBtn);
            startBtn.BringToFront();

            ShuffleIndices();
            CreateButtons();
            EnableAllCards(false);

            isFormLoaded = true;
        }

        // ===== YJET - Krijon panelin e background =====
        void CreateStarPanel()
        {
            starPanel = new Panel();
            starPanel.Dock = DockStyle.Fill;
            starPanel.BackColor = Color.Transparent;
            starPanel.Paint += StarPanel_Paint;
            this.Controls.Add(starPanel);
            starPanel.SendToBack();
        }

        void GenerateStars()
        {
            stars.Clear();
            int count = 120;
            for (int i = 0; i < count; i++)
            {
                stars.Add(new Star
                {
                    X = (float)(rand.NextDouble() * this.ClientSize.Width),
                    Y = (float)(rand.NextDouble() * this.ClientSize.Height),
                    Size = (float)(rand.NextDouble() * 2.5 + 0.5),
                    Opacity = (float)(rand.NextDouble() * 0.8 + 0.2),
                    Speed = (float)(rand.NextDouble() * 0.3 + 0.05)
                });
            }
        }

        void StarTimer_Tick(object sender, EventArgs e)
        {
            // Yjet pulojnë - ndryshojnë opacity ngadalë
            for (int i = 0; i < stars.Count; i++)
            {
                Star s = stars[i];
                s.Opacity += s.Speed * (rand.Next(2) == 0 ? 1 : -1) * 0.05f;
                if (s.Opacity > 1.0f) s.Opacity = 1.0f;
                if (s.Opacity < 0.1f) s.Opacity = 0.1f;
                stars[i] = s;
            }
            starPanel?.Invalidate();
        }

        void StarPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(10, 10, 40));

            foreach (Star s in stars)
            {
                int alpha = (int)(s.Opacity * 255);
                if (alpha < 10) continue;
                Color starColor = Color.FromArgb(alpha, 200, 220, 255);
                using (SolidBrush brush = new SolidBrush(starColor))
                {
                    float half = s.Size / 2f;
                    g.FillEllipse(brush, s.X - half, s.Y - half, s.Size, s.Size);
                }
                // Yllë të mëdha marrin një kryq të vogël shkëlqyes
                if (s.Size > 2.0f)
                {
                    int gAlpha = (int)(s.Opacity * 120);
                    using (Pen pen = new Pen(Color.FromArgb(gAlpha, 180, 210, 255), 0.5f))
                    {
                        g.DrawLine(pen, s.X - s.Size, s.Y, s.X + s.Size, s.Y);
                        g.DrawLine(pen, s.X, s.Y - s.Size, s.X, s.Y + s.Size);
                    }
                }
            }
        }
        // ===============================================

        private Button CreateStyledButton(string text, Size size)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = size;
            btn.BackColor = Color.FromArgb(30, 30, 80);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(80, 100, 180);
            btn.Font = new Font("Arial", 10, FontStyle.Bold);
            return btn;
        }

        private void AddSeparator(Panel parent, int x, int y, int w)
        {
            Panel sep = new Panel();
            sep.BackColor = Color.FromArgb(60, 80, 140);
            sep.Location = new Point(x, y);
            sep.Size = new Size(w, 1);
            sep.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            parent.Controls.Add(sep);
        }

        void ResetGame()
        {
            StopAllTimers();
            isGameActive = false;
            isProcessingClick = false;

            var toRemove = new List<Control>();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn &&
                    (btn.Text == "🔄 Restart" || btn.Text == "Exit" || btn.Text == "▶ Start Game"))
                    continue;
                if (ctrl == timeLabel) continue;
                if (ctrl == starPanel) continue;
                if (ctrl is Label lbl && lbl != timeLabel) continue;
                toRemove.Add(ctrl);
            }
            foreach (var ctrl in toRemove)
                this.Controls.Remove(ctrl);

            cardButtons.Clear();

            if (!this.Controls.Contains(timeLabel))
            {
                this.Controls.Add(timeLabel);
                timeLabel.BringToFront();
            }

            Button startBtn = null;
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button b && b.Text == "▶ Start Game")
                    startBtn = b;

            if (startBtn == null)
            {
                startBtn = CreateStyledButton("▶ Start Game", new Size(120, 40));
                startBtn.Location = new Point(this.ClientSize.Width / 2 - 60, this.ClientSize.Height - 80);
                startBtn.Anchor = AnchorStyles.Bottom;
                var sb = startBtn;
                startBtn.Click += (s, e) =>
                {
                    if (!isGameActive)
                    {
                        ResetGame();
                        isGameActive = true;
                        sb.Visible = false;
                        EnableAllCards(true);
                        gameTimer.Start();
                    }
                };
                this.Controls.Add(startBtn);
            }
            startBtn.Visible = true;
            startBtn.BringToFront();

            firstClicked = null;
            secondClicked = null;
            matchedButtons.Clear();
            timeLeft = 60;
            timeLabel.Text = "Time: 60";

            // Restart star timer nëse ishte ndalur
            if (!starTimer.Enabled) starTimer.Start();

            ShuffleIndices();
            CreateButtons();
            EnableAllCards(false);
        }

        void EnableAllCards(bool enabled)
        {
            foreach (Button btn in cardButtons)
                btn.Enabled = enabled;
        }

        void StopAllTimers()
        {
            gameTimer.Stop();
            timer.Stop();
            // NUK ndalojmë starTimer - yjet vazhdojnë gjithmonë
        }

        void ShuffleIndices()
        {
            shuffledIndices.Clear();
            for (int i = 0; i < images.Count; i++)
                shuffledIndices.Add(i);
            for (int i = 0; i < shuffledIndices.Count; i++)
            {
                int j = rand.Next(i, shuffledIndices.Count);
                int temp = shuffledIndices[i];
                shuffledIndices[i] = shuffledIndices[j];
                shuffledIndices[j] = temp;
            }
        }

        void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!isGameActive) return;
            timeLeft--;
            timeLabel.Text = "Time: " + timeLeft;
            if (timeLeft <= 0)
            {
                gameTimer.Stop();
                timer.Stop();
                isGameActive = false;
                EnableAllCards(false);
                MessageBox.Show("Time's up! 😢", "Game Over",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStartButton();
            }
        }

        void ShowStartButton()
        {
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button btn && btn.Text == "▶ Start Game")
                {
                    btn.Visible = true;
                    btn.BringToFront();
                    break;
                }
        }

        void CreateButtons()
        {
            int rows = 4;
            int cols = 4;
            int topOffset = 80;
            int spacing = 10;

            int availableWidth = this.ClientSize.Width;
            int availableHeight = this.ClientSize.Height - topOffset - 20;
            int btnSize = Math.Min(
                (availableWidth - (cols - 1) * spacing) / (cols + 2),
                (availableHeight - (rows - 1) * spacing) / rows
            );

            int totalGridWidth = cols * btnSize + (cols - 1) * spacing;
            int totalGridHeight = rows * btnSize + (rows - 1) * spacing;
            int startX = (this.ClientSize.Width - totalGridWidth) / 2;
            int startY = topOffset + (availableHeight - totalGridHeight) / 2;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * 4 + j;
                    int originalIndex = shuffledIndices[index];

                    Button btn = new Button();
                    btn.Size = new Size(btnSize, btnSize);
                    btn.Location = new Point(
                        startX + j * (btnSize + spacing),
                        startY + i * (btnSize + spacing)
                    );
                    btn.Tag = imageNames[originalIndex];
                    btn.AccessibleDescription = originalIndex.ToString();
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 2;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(80, 120, 220);
                    btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 80, 200);
                    btn.BackColor = Color.FromArgb(30, 60, 160);
                    // ===== PIKËPYETJA =====
                    btn.Text = "?";
                    btn.Font = new Font("Arial", 28, FontStyle.Bold);
                    btn.ForeColor = Color.FromArgb(100, 149, 237);
                    // ======================
                    btn.Click += Button_Click;
                    btn.MouseEnter += (s, e) =>
                    {
                        if (!matchedButtons.Contains(btn))
                            btn.BackColor = Color.FromArgb(60, 100, 220);
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        if (!matchedButtons.Contains(btn))
                            btn.BackColor = Color.FromArgb(30, 60, 160);
                    };
                    this.Controls.Add(btn);
                    btn.BringToFront();
                    cardButtons.Add(btn);
                }
            }
        }

        void RepositionCards()
        {
            if (cardButtons.Count == 0) return;

            int rows = 4, cols = 4;
            int topOffset = 80;
            int spacing = 10;

            int availableWidth = this.ClientSize.Width;
            int availableHeight = this.ClientSize.Height - topOffset - 20;
            int btnSize = Math.Min(
                (availableWidth - (cols - 1) * spacing) / (cols + 2),
                (availableHeight - (rows - 1) * spacing) / rows
            );
            if (btnSize < 40) btnSize = 40;

            int totalGridWidth = cols * btnSize + (cols - 1) * spacing;
            int totalGridHeight = rows * btnSize + (rows - 1) * spacing;
            int startX = (this.ClientSize.Width - totalGridWidth) / 2;
            int startY = topOffset + (availableHeight - totalGridHeight) / 2;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int idx = i * cols + j;
                    if (idx >= cardButtons.Count) continue;
                    cardButtons[idx].Size = new Size(btnSize, btnSize);
                    cardButtons[idx].Location = new Point(
                        startX + j * (btnSize + spacing),
                        startY + i * (btnSize + spacing)
                    );
                }
            }

            // Rigjeneroj yjet sipas madhësisë së re
            GenerateStars();

            foreach (Control ctrl in this.Controls)
                if (ctrl is Button btn && btn.Text == "▶ Start Game")
                {
                    btn.Location = new Point(
                        this.ClientSize.Width / 2 - btn.Width / 2,
                        this.ClientSize.Height - 70);
                    break;
                }
        }

        void Button_Click(object sender, EventArgs e)
        {
            if (!isGameActive) return;

            Button clicked = sender as Button;

            if (isProcessingClick) return;
            if (matchedButtons.Contains(clicked)) return;
            if (clicked == firstClicked) return;

            int imgIndex = int.Parse(clicked.AccessibleDescription);
            clicked.BackgroundImage = images[imgIndex];
            clicked.Text = ""; // FIX: fshij pikëpyetjen kur hapet karta

            if (firstClicked == null)
            {
                firstClicked = clicked;
                return;
            }

            secondClicked = clicked;
            isProcessingClick = true;
            EnableAllCards(false);

            if ((string)firstClicked.Tag == (string)secondClicked.Tag)
            {
                // MATCH - FIX: pikëpyetja NUK kthehet, imazhi mbetet, vetëm ngjyra ndryshon
                System.Media.SystemSounds.Asterisk.Play();
                matchedButtons.Add(firstClicked);
                matchedButtons.Add(secondClicked);

                firstClicked.BackColor = Color.FromArgb(50, 180, 80);   // gjelbër i bukur
                firstClicked.FlatAppearance.BorderColor = Color.FromArgb(80, 220, 100);
                // NUK i kthejmë Text = "?" - karta mbetet hapur me imazh!

                secondClicked.BackColor = Color.FromArgb(50, 180, 80);
                secondClicked.FlatAppearance.BorderColor = Color.FromArgb(80, 220, 100);
                // NUK i kthejmë Text = "?" - karta mbetet hapur me imazh!

                firstClicked = null;
                secondClicked = null;
                isProcessingClick = false;

                EnableAllCards(true);
                foreach (Button mb in matchedButtons)
                    mb.Enabled = false;

                if (matchedButtons.Count == 16)
                {
                    gameTimer.Stop();
                    timer.Stop();
                    isGameActive = false;
                    EnableAllCards(false);
                    MessageBox.Show("Urime! Fitove! 🎉", "Victory",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowStartButton();
                }
            }
            else
            {
                // JO MATCH - trego 750ms pastaj mbuloji me pikëpyetje
                timer.Start();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            // FIX: kthej pikëpyetjen dhe ngjyrën blu - jo gri!
            if (firstClicked != null && !matchedButtons.Contains(firstClicked))
            {
                firstClicked.BackgroundImage = null;
                firstClicked.BackColor = Color.FromArgb(30, 60, 160);       // BLU jo gri
                firstClicked.FlatAppearance.BorderColor = Color.FromArgb(80, 120, 220);
                firstClicked.Text = "?";                                      // kthe pikëpyetjen
                firstClicked.Font = new Font("Arial", 28, FontStyle.Bold);
                firstClicked.ForeColor = Color.FromArgb(100, 149, 237);
            }
            if (secondClicked != null && !matchedButtons.Contains(secondClicked))
            {
                secondClicked.BackgroundImage = null;
                secondClicked.BackColor = Color.FromArgb(30, 60, 160);      // BLU jo gri
                secondClicked.FlatAppearance.BorderColor = Color.FromArgb(80, 120, 220);
                secondClicked.Text = "?";                                     // kthe pikëpyetjen
                secondClicked.Font = new Font("Arial", 28, FontStyle.Bold);
                secondClicked.ForeColor = Color.FromArgb(100, 149, 237);
            }

            firstClicked = null;
            secondClicked = null;
            isProcessingClick = false;

            EnableAllCards(true);
            foreach (Button mb in matchedButtons)
                mb.Enabled = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isFormLoaded)
                RepositionCards();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            starTimer.Stop();
            StopAllTimers();
            if (mainFormReference != null && !mainFormReference.IsDisposed)
                mainFormReference.Show();
            base.OnFormClosing(e);
        }

        private void MemoryForm_Load(object sender, EventArgs e) { }
    }
}