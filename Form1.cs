using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GameHub
{
    public partial class Form1 : Form
    {
        private Size originalFormSize;
        private Dictionary<Control, Rectangle> originalBounds = new Dictionary<Control, Rectangle>();

        // Hill Climb Racing controls
        private Panel hillClimbPanel;
        private Button hillClimbPlayButton;
        private Button hillClimbBackButton;
        private PictureBox pictureHillClimb;

        public Form1()
        {
            InitializeComponent();

            // ========== OPTIMIZIMI I PERFORMANCËS ==========
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.DoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // Aktivizo DoubleBuffering për të gjitha panelet
            EnableDoubleBuffering(Home);
            EnableDoubleBuffering(snakePanel);
            EnableDoubleBuffering(memoryPanel);
            EnableDoubleBuffering(dinoPanel);
            EnableDoubleBuffering(flappyBirdPanel);
            EnableDoubleBuffering(panelGames);
            EnableDoubleBuffering(panelSnale);
            EnableDoubleBuffering(panelMemory);
            EnableDoubleBuffering(panelFlappy);
            EnableDoubleBuffering(panelTicTacToe);

            EnableDoubleBufferingForAllControls(Home);

            // ========== KRIJO PANELIN PËR HILL CLIMB RACING ==========
            CreateHillClimbPanel();

            // ========== MENAXHIMI I PANELEVE ==========
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();

            OptimizeAllImages();
        }

        private void CreateHillClimbPanel()
        {
            // Krijo panelin kryesor
            hillClimbPanel = new Panel
            {
                Name = "hillClimbPanel",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = false
            };

            // Krijo butonin Play
            hillClimbPlayButton = new Button
            {
                Text = "PLAY HILL CLIMB RACING",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(300, 60),
                Location = new Point(362, 450)
            };
            hillClimbPlayButton.Click += HillClimbPlayButton_Click;

            // Krijo butonin Back
            hillClimbBackButton = new Button
            {
                Text = "BACK TO MENU",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 50),
                Location = new Point(412, 520)
            };
            hillClimbBackButton.Click += HillClimbBackButton_Click;

            // Krijo PictureBox për thumbnail
            pictureHillClimb = new PictureBox
            {
                Size = new Size(400, 300),
                Location = new Point(312, 100),
                BackColor = Color.FromArgb(50, 50, 50),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Vizato thumbnail-in e lojës
            Bitmap thumbnail = new Bitmap(400, 300);
            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                g.Clear(Color.LightBlue);

                // Vizato terren
                using (Pen pen = new Pen(Color.SaddleBrown, 3))
                {
                    for (int i = 0; i < 400; i += 40)
                    {
                        int y = 200 + (int)(Math.Sin(i * 0.05) * 30);
                        g.DrawLine(pen, i, y, i + 40, 200 + (int)(Math.Sin((i + 40) * 0.05) * 30));
                    }
                }

                // Vizato makinë
                g.FillRectangle(Brushes.Red, 100, 170, 50, 30);
                g.FillEllipse(Brushes.Black, 110, 195, 15, 15);
                g.FillEllipse(Brushes.Black, 135, 195, 15, 15);

                // Vizato tekst
                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                {
                    g.DrawString("HILL CLIMB", font, Brushes.White, 150, 150);
                    g.DrawString("RACING", font, Brushes.White, 160, 170);
                }
            }
            pictureHillClimb.Image = thumbnail;

            hillClimbPanel.Controls.Add(pictureHillClimb);
            hillClimbPanel.Controls.Add(hillClimbPlayButton);
            hillClimbPanel.Controls.Add(hillClimbBackButton);

            this.Controls.Add(hillClimbPanel);
        }

        private void HillClimbPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            HillClimbRacing hillClimb = new HillClimbRacing(this);
            hillClimb.Show();
        }

        private void HillClimbBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();
            this.Refresh();
        }

        private void EnableDoubleBuffering(Control control)
        {
            if (control == null) return;
            try
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic,
                    null, control, new object[] { true });
            }
            catch { }
        }

        private void EnableDoubleBufferingForAllControls(Control parent)
        {
            if (parent == null) return;
            EnableDoubleBuffering(parent);
            foreach (Control child in parent.Controls)
            {
                EnableDoubleBuffering(child);
                if (child.HasChildren)
                {
                    EnableDoubleBufferingForAllControls(child);
                }
            }
        }

        private void HideAllGamePanels()
        {
            if (snakePanel != null) snakePanel.Visible = false;
            if (memoryPanel != null) memoryPanel.Visible = false;
            if (dinoPanel != null) dinoPanel.Visible = false;
            if (flappyBirdPanel != null) flappyBirdPanel.Visible = false;
            if (hillClimbPanel != null) hillClimbPanel.Visible = false;
        }

        private void CleanupPanel(Panel panel)
        {
            if (panel == null) return;
            foreach (Control ctrl in panel.Controls)
            {
                // Pastrim nëse nevojitet
            }
        }

        private void ShowGamePanel(Panel gamePanel)
        {
            HideAllGamePanels();
            Home.Visible = false;
            if (gamePanel != null)
            {
                gamePanel.Visible = true;
                gamePanel.BringToFront();
                gamePanel.Refresh();
            }
        }

        private void OptimizeAllImages()
        {
            OptimizeImagesInContainer(Home);
            OptimizeImagesInContainer(snakePanel);
            OptimizeImagesInContainer(memoryPanel);
            OptimizeImagesInContainer(dinoPanel);
            OptimizeImagesInContainer(flappyBirdPanel);
            OptimizeImagesInContainer(hillClimbPanel);
        }

        private void OptimizeImagesInContainer(Control container)
        {
            if (container == null) return;
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is PictureBox pic)
                {
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (pic.Image != null)
                    {
                        var currentImage = pic.Image;
                        pic.Image = null;
                        pic.Image = currentImage;
                    }
                }
                else if (ctrl.HasChildren)
                {
                    OptimizeImagesInContainer(ctrl);
                }
            }
        }

        // ========== EVENTET E KLIKIMIT PËR PICTUREBOX-AT ==========
        private void pictureSnake_Click(object sender, EventArgs e)
        {
            ShowGamePanel(snakePanel);
        }

        private void pictureMemory_Click(object sender, EventArgs e)
        {
            ShowGamePanel(memoryPanel);
        }

        private void pictureFlappy_Click(object sender, EventArgs e)
        {
            ShowGamePanel(flappyBirdPanel);
        }

        private void pictureTicTacToe_Click(object sender, EventArgs e)
        {
            ShowGamePanel(dinoPanel);
        }

        private void SnakePictureClick(object sender, EventArgs e)
        {
            ShowGamePanel(snakePanel);
        }

        // ========== BUTONAT PLAY ==========
        private void playButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SnakeForm snake = new SnakeForm(this);
            snake.Show();
        }

        private void flappyPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            FlappyBirdForm bird = new FlappyBirdForm(this);
            bird.Show();
        }

        private void memoryPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            MemoryForm memory = new MemoryForm(this);
            memory.Show();
        }

        private void dinoPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            DinoJumpForm dino = new DinoJumpForm(this);
            dino.Show();
        }

        // ========== BUTONAT BACK ==========
        private void backButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();
            this.Refresh();
        }

        private void flappyBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();
            this.Refresh();
        }

        private void memoryBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();
            this.Refresh();
        }

        private void dinoBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            Home.Visible = true;
            Home.BringToFront();
            this.Refresh();
        }

        // ========== METODAT PËR RESIZE ==========
        private void Form1_Load(object sender, EventArgs e)
        {
            originalFormSize = this.ClientSize;
            SaveOriginalBounds(this);
        }

        private void SaveOriginalBounds(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!originalBounds.ContainsKey(c))
                {
                    originalBounds[c] = new Rectangle(c.Location, c.Size);
                }
                if (c.HasChildren)
                {
                    SaveOriginalBounds(c);
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (originalFormSize.IsEmpty ||
                this.WindowState == FormWindowState.Minimized ||
                originalBounds.Count == 0)
                return;

            double ratioX = (double)this.ClientSize.Width / originalFormSize.Width;
            double ratioY = (double)this.ClientSize.Height / originalFormSize.Height;

            double minRatio = 0.3;
            ratioX = Math.Max(ratioX, minRatio);
            ratioY = Math.Max(ratioY, minRatio);

            this.SuspendLayout();

            try
            {
                foreach (var item in originalBounds)
                {
                    var ctrl = item.Key;
                    var orig = item.Value;

                    if (ctrl == null || !ctrl.IsHandleCreated || ctrl.IsDisposed)
                        continue;

                    try
                    {
                        int newX = (int)(orig.X * ratioX);
                        int newY = (int)(orig.Y * ratioY);
                        int newWidth = (int)(orig.Width * ratioX);
                        int newHeight = (int)(orig.Height * ratioY);

                        newWidth = Math.Max(newWidth, 10);
                        newHeight = Math.Max(newHeight, 10);

                        ctrl.Location = new Point(newX, newY);
                        ctrl.Size = new Size(newWidth, newHeight);
                    }
                    catch { }
                }
            }
            finally
            {
                this.ResumeLayout(false);
                this.PerformLayout();
                this.Refresh();
            }
        }

        // ========== METODAT BOSHE PËR PAJTUESHMËRI ME DESIGNER ==========
        private void label1_Click(object sender, EventArgs e) { }
        private void label1_Click_1(object sender, EventArgs e) { }
        private void label1_Click_2(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void label2_Click_2(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label3_Click_1(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label5_Click_1(object sender, EventArgs e) { }
        private void label5_Click_2(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click_2(object sender, EventArgs e) { }
        private void pictureBox1_Click_3(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void snakeGame_Click(object sender, EventArgs e) { }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e) { }
        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e) { }
        private void snakePanel_Paint(object sender, PaintEventArgs e) { }
        private void panelGames_Paint(object sender, PaintEventArgs e) { }

        // Metoda për PictureBox-in e Hill Climb në Home panel (me Click_1)
        private void pictureHillClimbHome_Click_1(object sender, EventArgs e)
        {
            ShowGamePanel(hillClimbPanel);
        }
    }
}