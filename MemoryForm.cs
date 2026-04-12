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

        // Game state
        bool isGameActive = false;
        Form mainFormReference;
        bool isFormLoaded = false;

        // Lista e kartave (butonave) për t'i ripozicionuar gjatë resize
        List<Button> cardButtons = new List<Button>();

        // Card data
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
            mainFormReference = parent;
            this.BackColor = Color.FromArgb(20, 20, 60);
            this.ClientSize = new Size(600, 600);

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
            this.Controls.Add(timeLabel);
            timeLabel.Text = "Time: 60";

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
                if (mainFormReference != null && !mainFormReference.IsDisposed)
                    mainFormReference.Show();
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

            isFormLoaded = true;
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

            // Fshij të gjitha kontrollet përveç butonave fiks dhe timeLabel
            var toRemove = new List<Control>();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && (btn.Text == "🔄 Restart" || btn.Text == "Exit" || btn.Text == "▶ Start Game"))
                    continue;
                if (ctrl != timeLabel)
                    toRemove.Add(ctrl);
            }
            foreach (var ctrl in toRemove)
                this.Controls.Remove(ctrl);

            // Pastro listën e kartave
            cardButtons.Clear();

            if (!this.Controls.Contains(timeLabel))
                this.Controls.Add(timeLabel);

            // Gjej ose krijo Start button
            Button startBtn = null;
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button b && b.Text == "▶ Start Game")
                    startBtn = b;

            if (startBtn == null)
            {
                startBtn = CreateStyledButton("▶ Start Game", new Size(120, 40));
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
                MessageBox.Show("Time's up! 😢", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStartButton();
            }
        }

        void ShowStartButton()
        {
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button btn && btn.Text == "▶ Start Game")
                {
                    btn.Visible = true;
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
                    cardButtons.Add(btn);  // ruajm referencën për resize
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
                    Button btn = cardButtons[idx];
                    btn.Size = new Size(btnSize, btnSize);
                    btn.Location = new Point(
                        sidePadding + j * (btnSize + spacing),
                        topOffset + i * (btnSize + spacing)
                    );
                }
            }

            // Rregullo pozicionin e butonit Start që të mbetet në qendër
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button btn && btn.Text == "▶ Start Game")
                {
                    btn.Location = new Point(this.ClientSize.Width / 2 - btn.Width / 2, this.ClientSize.Height - 70);
                    break;
                }
        }

        void Button_Click(object sender, EventArgs e)
        {
            if (!isGameActive) return;

            Button clicked = sender as Button;
            if (matchedButtons.Contains(clicked) || timer.Enabled)
                return;

            int imgIndex = int.Parse(clicked.AccessibleDescription);
            clicked.BackgroundImage = images[imgIndex];

            if (firstClicked == null)
            {
                firstClicked = clicked;
                return;
            }

            secondClicked = clicked;

            if ((string)firstClicked.Tag == (string)secondClicked.Tag)
            {
                System.Media.SystemSounds.Asterisk.Play();
                matchedButtons.Add(firstClicked);
                matchedButtons.Add(secondClicked);
                firstClicked.BackColor = Color.LightGreen;
                secondClicked.BackColor = Color.LightGreen;

                firstClicked = null;
                secondClicked = null;

                if (matchedButtons.Count == 16)
                {
                    gameTimer.Stop();
                    timer.Stop();
                    isGameActive = false;
                    EnableAllCards(false);
                    // Ndryshimi 1: Nuk e ndryshojmë më background-in e formës
                    MessageBox.Show("Urime! Fitove! 🎉", "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowStartButton();
                }
            }
            else
            {
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
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isFormLoaded)
            {
                // Ndryshimi 2: Në vend që të resetojë lojën, vetëm ripozicionon kartat
                RepositionCards();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopAllTimers();
            base.OnFormClosing(e);
        }

        private void MemoryForm_Load(object sender, EventArgs e) { }
    }
}