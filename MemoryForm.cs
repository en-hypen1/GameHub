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
        private Form _parentForm;
        bool isGameActive = false;
        Form mainFormReference;
        bool isFormLoaded = false;
        bool isProcessingClick = false; // FIX: bllokon klikimin e tretë

        List<Button> cardButtons = new List<Button>();

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
            this.BackColor = Color.FromArgb(20, 20, 60);
            this.ClientSize = new Size(600, 600);

            // Match parent window state
            if (parent != null)
            {
                if (parent.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Maximized;
                else if (parent.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Minimized;
                else
                    this.WindowState = FormWindowState.Normal;
            }

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
            timeLabel.Text = "Time: 60";
            this.Controls.Add(timeLabel);

            // Restart button
            Button restartBtn = CreateStyledButton("🔄 Restart", new Size(120, 40));
            restartBtn.Location = new Point(this.ClientSize.Width - 140, 10);
            restartBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            restartBtn.Click += (s, e) => ResetGame();
            this.Controls.Add(restartBtn);

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

            // Shto panelin e info në të djathtë
            AddInfoPanel();

            isFormLoaded = true;
        }

        private void AddInfoPanel()
        {
            // Panel background
            Panel infoPanel = new Panel();
            infoPanel.BackColor = Color.FromArgb(30, 30, 70);
            infoPanel.Location = new Point(660, 65);
            infoPanel.Size = new Size(this.ClientSize.Width - 675, this.ClientSize.Height - 80);
            infoPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                               AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(infoPanel);

            int y = 15;
            int x = 15;
            int w = infoPanel.Width - 30;

            // Titulli
            Label lblTitle = new Label();
            lblTitle.Text = "🌌 MEMORY GAME";
            lblTitle.Font = new Font("Arial", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(180, 200, 255);
            lblTitle.Location = new Point(x, y);
            lblTitle.Size = new Size(w, 28);
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            infoPanel.Controls.Add(lblTitle);
            y += 35;

            // Nënshkrimi
            Label lblSub = new Label();
            lblSub.Text = "Gjej të gjitha çiftet e\nplanetave të fshehur!";
            lblSub.Font = new Font("Arial", 10);
            lblSub.ForeColor = Color.FromArgb(160, 180, 230);
            lblSub.Location = new Point(x, y);
            lblSub.Size = new Size(w, 42);
            lblSub.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            infoPanel.Controls.Add(lblSub);
            y += 50;

            // Ndarëse
            AddSeparator(infoPanel, x, y, w); y += 18;

            // Si luhet
            Label lblHow = new Label();
            lblHow.Text = "📋 Si luhet:";
            lblHow.Font = new Font("Arial", 10, FontStyle.Bold);
            lblHow.ForeColor = Color.FromArgb(180, 200, 255);
            lblHow.Location = new Point(x, y);
            lblHow.Size = new Size(w, 22);
            infoPanel.Controls.Add(lblHow);
            y += 28;

            string[] steps = {
        "① Kliko një kartë për ta kthyer",
        "② Gjej kartën tjetër të njëjtë",
        "③ Çifto të 8 planetat!"
    };
            foreach (string step in steps)
            {
                Label l = new Label();
                l.Text = step;
                l.Font = new Font("Arial", 9);
                l.ForeColor = Color.FromArgb(200, 220, 255);
                l.Location = new Point(x + 5, y);
                l.Size = new Size(w - 5, 20);
                l.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                infoPanel.Controls.Add(l);
                y += 24;
            }
            y += 6;

            // Ndarëse
            AddSeparator(infoPanel, x, y, w); y += 18;

            // Koha dhe score
            Label lblInfo = new Label();
            lblInfo.Text = "⏱ Ke 60 sekonda";
            lblInfo.Font = new Font("Arial", 10, FontStyle.Bold);
            lblInfo.ForeColor = Color.FromArgb(255, 220, 100);
            lblInfo.Location = new Point(x, y);
            lblInfo.Size = new Size(w, 22);
            infoPanel.Controls.Add(lblInfo);
            y += 28;

            Label lblScore = new Label();
            lblScore.Text = "🏆 Sa më shpejt aq më mirë!";
            lblScore.Font = new Font("Arial", 9);
            lblScore.ForeColor = Color.FromArgb(200, 220, 255);
            lblScore.Location = new Point(x, y);
            lblScore.Size = new Size(w, 20);
            lblScore.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            infoPanel.Controls.Add(lblScore);
            y += 30;

            // Ndarëse
            AddSeparator(infoPanel, x, y, w); y += 18;

            // Planetet
            Label lblPlanets = new Label();
            lblPlanets.Text = "🪐 Planetet:";
            lblPlanets.Font = new Font("Arial", 10, FontStyle.Bold);
            lblPlanets.ForeColor = Color.FromArgb(180, 200, 255);
            lblPlanets.Location = new Point(x, y);
            lblPlanets.Size = new Size(w, 22);
            infoPanel.Controls.Add(lblPlanets);
            y += 28;

            string[] planets = {
        "🌍  Earth", "🔴  Mars",    "🟠  Jupiter",  "🌙  Moon",
        "☀️  Sun",   "🌟  Venus",   "🔵  Neptune",  "💫  Saturn"
    };

            // 2 kolona
            int col1X = x + 5;
            int col2X = x + w / 2 + 5;
            int rowY = y;
            for (int i = 0; i < planets.Length; i++)
            {
                Label lp = new Label();
                lp.Text = planets[i];
                lp.Font = new Font("Arial", 9);
                lp.ForeColor = Color.FromArgb(200, 220, 255);
                lp.Size = new Size(w / 2 - 10, 22);
                lp.Location = new Point(i % 2 == 0 ? col1X : col2X, rowY);
                infoPanel.Controls.Add(lp);
                if (i % 2 == 1) rowY += 24;
            }
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
        private Button CreateStyledButton(string text, Size size)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = size;
            btn.BackColor = Color.FromArgb(30, 30, 30);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Arial", 10, FontStyle.Bold);
            return btn;
        }

        void ResetGame()
        {
            StopAllTimers();
            isGameActive = false;
            isProcessingClick = false; // reset edhe këtë

            var toRemove = new List<Control>();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn &&
                    (btn.Text == "🔄 Restart" || btn.Text == "Exit" || btn.Text == "▶ Start Game"))
                    continue;
                if (ctrl == timeLabel)
                    continue;
                // Mos fshi info label-in
                if (ctrl is Label lbl && lbl != timeLabel)
                    continue;
                toRemove.Add(ctrl);
            }
            foreach (var ctrl in toRemove)
                this.Controls.Remove(ctrl);

            cardButtons.Clear();

            if (!this.Controls.Contains(timeLabel))
                this.Controls.Add(timeLabel);

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
            int sidePadding = 40;
            int spacing = 10;

            int availableWidth = this.ClientSize.Width - (sidePadding * 2);
            int availableHeight = this.ClientSize.Height - topOffset - 20;
            int btnSize = Math.Min(
                (availableWidth - (cols - 1) * spacing) / cols,
                (availableHeight - (rows - 1) * spacing) / rows
            );

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * 4 + j;
                    int originalIndex = shuffledIndices[index];

                    Button btn = new Button();
                    btn.Size = new Size(btnSize, btnSize);
                    btn.Location = new Point(
                        sidePadding + j * (btnSize + spacing),
                        topOffset + i * (btnSize + spacing)
                    );
                    btn.Tag = imageNames[originalIndex];
                    btn.AccessibleDescription = originalIndex.ToString();
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
                    btn.BackColor = Color.FromArgb(40, 40, 40);
                    btn.Click += Button_Click;
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.DarkSlateBlue;
                    btn.MouseLeave += (s, e) =>
                    {
                        if (!matchedButtons.Contains(btn))
                            btn.BackColor = Color.FromArgb(40, 40, 40);
                    };
                    this.Controls.Add(btn);
                    cardButtons.Add(btn);
                }
            }
        }

        void RepositionCards()
        {
            if (cardButtons.Count == 0) return;

            int rows = 4, cols = 4;
            int topOffset = 80;
            int sidePadding = 40;
            int spacing = 10;

            int availableWidth = this.ClientSize.Width - (sidePadding * 2);
            int availableHeight = this.ClientSize.Height - topOffset - 20;
            int btnSize = Math.Min(
                (availableWidth - (cols - 1) * spacing) / cols,
                (availableHeight - (rows - 1) * spacing) / rows
            );
            if (btnSize < 40) btnSize = 40;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int idx = i * cols + j;
                    if (idx >= cardButtons.Count) continue;
                    cardButtons[idx].Size = new Size(btnSize, btnSize);
                    cardButtons[idx].Location = new Point(
                        sidePadding + j * (btnSize + spacing),
                        topOffset + i * (btnSize + spacing)
                    );
                }
            }

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

            // FIX: bllokon klikimin e tretë
            if (isProcessingClick) return;
            if (matchedButtons.Contains(clicked)) return;
            if (clicked == firstClicked) return; // mos lejo klikimin e së njëjtës kartë dy herë

            int imgIndex = int.Parse(clicked.AccessibleDescription);
            clicked.BackgroundImage = images[imgIndex];

            if (firstClicked == null)
            {
                firstClicked = clicked;
                return;
            }

            secondClicked = clicked;

            // Bllokon çdo klikim tjetër derisa të përpunohet çifti
            isProcessingClick = true;
            EnableAllCards(false);

            if ((string)firstClicked.Tag == (string)secondClicked.Tag)
            {
                System.Media.SystemSounds.Asterisk.Play();
                matchedButtons.Add(firstClicked);
                matchedButtons.Add(secondClicked);
                firstClicked.BackColor = Color.LightGreen;
                secondClicked.BackColor = Color.LightGreen;

                firstClicked = null;
                secondClicked = null;
                isProcessingClick = false;

                // Rienable kartat e papërzgjedhura
                EnableAllCards(true);
                // Mos rienable kartat e matched
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
                // Trego kartat për 750ms pastaj mbuloji
                timer.Start();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            if (firstClicked != null && !matchedButtons.Contains(firstClicked))
            {
                firstClicked.BackgroundImage = null;
                firstClicked.BackColor = Color.FromArgb(40, 40, 40);
            }
            if (secondClicked != null && !matchedButtons.Contains(secondClicked))
            {
                secondClicked.BackgroundImage = null;
                secondClicked.BackColor = Color.FromArgb(40, 40, 40);
            }

            firstClicked = null;
            secondClicked = null;
            isProcessingClick = false;

            // Rienable të gjitha kartat pas mbulimit
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
            StopAllTimers();
            // FIX: shfaq gjithmonë formën prind kur mbyllet kjo formë
            if (mainFormReference != null && !mainFormReference.IsDisposed)
                mainFormReference.Show();
            base.OnFormClosing(e);
        }

        private void MemoryForm_Load(object sender, EventArgs e) { }
    }
}