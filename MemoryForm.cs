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
        ProgressBar timeBar;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();

        // Ruajm imazhet dhe emrat e tyre si çifte paralele
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

        // Pas shuffle, mbajm renditjen e re edhe per emrat
        List<int> shuffledIndices = new List<int>();

        public MemoryForm()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(20, 20, 60);
            this.ClientSize = new Size(600, 600);

            timer.Interval = 750;
            timer.Tick += Timer_Tick;

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            timeLabel = new Label();
            timeLabel.Location = new Point(10, 10);
            timeLabel.Size = new Size(200, 30);
            timeLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            timeLabel.ForeColor = Color.White;
            this.Controls.Add(timeLabel);


            Button restartBtn = new Button();
            restartBtn.Text = "🔄 Restart";
            restartBtn.Size = new Size(120, 40);
            restartBtn.Location = new Point(this.ClientSize.Width - 140, 10);

            restartBtn.BackColor = Color.FromArgb(30, 30, 30);
            restartBtn.ForeColor = Color.White;
            restartBtn.FlatStyle = FlatStyle.Flat;
            restartBtn.FlatAppearance.BorderSize = 0;
            restartBtn.Font = new Font("Arial", 10, FontStyle.Bold);

            restartBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            restartBtn.Click += (s, e) => { ResetGame(); };

            this.Controls.Add(restartBtn);
           // this.BackgroundImage = Properties.Resources.galaxy; // shto një foto galaxy
            //this.BackgroundImageLayout = ImageLayout.Stretch;
            ShuffleIndices();
            CreateButtons();
        }

        public MemoryForm(Form parent) : this()
        {
            if (parent.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else if (parent.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        void ResetGame()
        {
            timer.Stop();
            gameTimer.Stop();

            this.Controls.Clear();

            timeLabel = new Label();
            timeLabel.Location = new Point(10, 10);
            timeLabel.Size = new Size(200, 30);
            timeLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            timeLabel.ForeColor = Color.White;
            this.Controls.Add(timeLabel);

            Button restartBtn = new Button();
            restartBtn.Text = "🔄 Restart";
            restartBtn.Size = new Size(120, 40);
            restartBtn.Location = new Point(this.ClientSize.Width - 140, 10);

            restartBtn.BackColor = Color.FromArgb(30, 30, 30);
            restartBtn.ForeColor = Color.White;
            restartBtn.FlatStyle = FlatStyle.Flat;
            restartBtn.FlatAppearance.BorderSize = 0;
            restartBtn.Font = new Font("Arial", 10, FontStyle.Bold);

            restartBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            restartBtn.Click += (s, e) => { ResetGame(); };

            this.Controls.Add(restartBtn);

            firstClicked = null;
            secondClicked = null;
            matchedButtons.Clear();
            timeLeft = 60;

            gameTimer.Start();
            ShuffleIndices();
            CreateButtons();
        }

        // Shuffle indices (jo imazhet direkt) - keshtu ruhet lidhja imazh<->emer
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
            timeLeft--;
            timeLabel.Text = "Time: " + timeLeft;
            if (timeLeft == 0)
            {
                gameTimer.Stop();
                MessageBox.Show("Time's up! 😢");
            }
        }

        /* void CreateButtons()
         {
             int btnSize = 120;
             int spacing = 10;

             for (int i = 0; i < 4; i++)
             {
                 for (int j = 0; j < 4; j++)
                 {
                     int index = i * 4 + j;
                     int originalIndex = shuffledIndices[index];

                     Button btn = new Button();
                     btn.Size = new Size(btnSize, btnSize);
                     btn.Location = new Point(j * (btnSize + spacing) + 50, i * (btnSize + spacing) + 60);

                     // TAG = emri i imazhit (string) - per krahasim te sakte
                     btn.Tag = imageNames[originalIndex];

                     // Ruajm imazhin si property e veçante (perdorim Name per imazhin)
                     btn.BackgroundImage = images[originalIndex]; // e fshehim menjehere
                     btn.BackgroundImage = null;

                     // Ruajm imazhin ne AccessibleDescription (menyra e thjesht)
                     btn.AccessibleDescription = originalIndex.ToString();

                     btn.BackgroundImageLayout = ImageLayout.Stretch;
                     btn.BackColor = Color.Gray;
                     btn.Click += Button_Click;
                     this.Controls.Add(btn);
                 }
             }
         }*/
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
                    btn.BackColor = Color.FromArgb(40, 40, 40);

                    btn.Click += Button_Click;
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.DarkSlateBlue;

                    btn.MouseLeave += (s, e) =>
                    {
                        if (!matchedButtons.Contains(btn))
                            btn.BackColor = Color.FromArgb(40, 40, 40);
                    };

                    this.Controls.Add(btn);
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResetGame(); // rigjeneron grid sipas madhësisë
        }

        void Button_Click(object sender, EventArgs e)
        {
            Button clicked = sender as Button;

            if (matchedButtons.Contains(clicked) || timer.Enabled)
                return;

            // Shfaq imazhin duke perdorur indeksin e ruajtur
            int imgIndex = int.Parse(clicked.AccessibleDescription);
            clicked.BackgroundImage = images[imgIndex];

            if (firstClicked == null)
            {
                firstClicked = clicked;
                return;
            }

            secondClicked = clicked;

            // Krahaso emrat (string) - jo referencat e objekteve
            if ((string)firstClicked.Tag == (string)secondClicked.Tag)
            {
                System.Media.SystemSounds.Asterisk.Play();
                matchedButtons.Add(firstClicked);
                matchedButtons.Add(secondClicked);

                firstClicked.BackColor = Color.LightGreen;
                secondClicked.BackColor = Color.LightGreen;

                firstClicked = null;
                secondClicked = null;

                // Kontrollo nese loja mbaroi
                if (matchedButtons.Count == 16)
                {
                    gameTimer.Stop();
                    this.BackColor = Color.DarkGreen;
                    MessageBox.Show("Urime! Fitove! 🎉");
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
                firstClicked.BackColor = Color.Gray;
            }
            if (secondClicked != null && !matchedButtons.Contains(secondClicked))
            {
                secondClicked.BackgroundImage = null;
                secondClicked.BackColor = Color.Gray;
            }

            firstClicked = null;
            secondClicked = null;
        }

        private void MemoryForm_Load(object sender, EventArgs e) { }
    }
}