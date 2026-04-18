namespace GameHub
{
    public partial class Main : Form
    {
        private Rectangle originalFormRect;
        private Size normalFormSize;
        private Rectangle pictureSnakeOriginalRect;
        private Rectangle pictureMemoryOriginalRect;
        private Rectangle pictureFlappyOriginalRect;
        private Rectangle pictureDinoOriginalRect;
        private Rectangle pictureHillOriginalRect;
        private Rectangle pictureTetrisOriginalRect;

        public Main()
        {
            InitializeComponent();

            // Optimizimi i performancës
            this.DoubleBuffered = true;

            // Lidh eventin e Resize
            this.Resize += Main_Resize;

            // Ruaj madhësinë normale PARA se ta ndryshojmë
            normalFormSize = this.Size;

            // Aktivizo DoubleBuffering për PictureBox-at për të eliminuar dridhjet
            EnableDoubleBuffering(pictureSnake);
            EnableDoubleBuffering(pictureMemory);
            EnableDoubleBuffering(pictureFlappy);
            EnableDoubleBuffering(pictureDino);
            EnableDoubleBuffering(pictureHill);
            EnableDoubleBuffering(HomePanel);
            EnableDoubleBuffering(snakePanel);
            EnableDoubleBuffering(memoryPanel);
            EnableDoubleBuffering(dinoPanel);
            EnableDoubleBuffering(flappyBirdPanel);
            EnableDoubleBuffering(hillPanel);
            EnableDoubleBuffering(TetrisPanel);
            // ========== MENAXHIMI I PANELEVE ==========
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();

            OptimizeAllImages();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Ruaj madhësinë origjinale të formës
            originalFormRect = new Rectangle(this.Location.X, this.Location.Y, normalFormSize.Width, normalFormSize.Height);

            // Ruaj madhësitë origjinale të PictureBox-ëve
            pictureSnakeOriginalRect = new Rectangle(pictureSnake.Location, pictureSnake.Size);
            pictureMemoryOriginalRect = new Rectangle(pictureMemory.Location, pictureMemory.Size);
            pictureFlappyOriginalRect = new Rectangle(pictureFlappy.Location, pictureFlappy.Size);
            pictureDinoOriginalRect = new Rectangle(pictureDino.Location, pictureDino.Size);
            pictureHillOriginalRect = new Rectangle(pictureHill.Location, pictureHill.Size);
            pictureTetrisOriginalRect = new Rectangle(pictureTetris.Location, pictureTetris.Size);


            // Bëj resize fillestar
            PerformResize();
        }

        private void PerformResize()
        {
            if (originalFormRect.Width == 0 || originalFormRect.Height == 0) return;

            this.SuspendLayout();

            ResizeControl(pictureSnakeOriginalRect, pictureSnake);
            ResizeControl(pictureMemoryOriginalRect, pictureMemory);
            ResizeControl(pictureFlappyOriginalRect, pictureFlappy);
            ResizeControl(pictureDinoOriginalRect, pictureDino);
            ResizeControl(pictureHillOriginalRect, pictureHill);
            ResizeControl(pictureTetrisOriginalRect, pictureTetris);

            this.ResumeLayout();
            this.Refresh();
        }

        private void ResizeControl(Rectangle originalRect, Control control)
        {
            if (control == null || originalRect.IsEmpty) return;

            float xRatio = (float)this.Width / originalFormRect.Width;
            float yRatio = (float)this.Height / originalFormRect.Height;

            int newX = (int)(originalRect.X * xRatio);
            int newY = (int)(originalRect.Y * yRatio);
            int newWidth = (int)(originalRect.Width * xRatio);
            int newHeight = (int)(originalRect.Height * yRatio);

            newWidth = Math.Max(newWidth, 10);
            newHeight = Math.Max(newHeight, 10);

            control.Location = new Point(newX, newY);
            control.Size = new Size(newWidth, newHeight);
        }


        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            PerformResize();
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

        private void HideAllGamePanels()
        {
            if (snakePanel != null) snakePanel.Visible = false;
            if (memoryPanel != null) memoryPanel.Visible = false;
            if (dinoPanel != null) dinoPanel.Visible = false;
            if (flappyBirdPanel != null) flappyBirdPanel.Visible = false;
            if (hillPanel != null) hillPanel.Visible = false;
            if (TetrisPanel != null) TetrisPanel.Visible = false;
            // if (hillClimbPanel != null) hillClimbPanel.Visible = false;
        }
        private void ShowGamePanel(Panel gamePanel)
        {
            HideAllGamePanels();
            HomePanel.Visible = false;
            if (gamePanel != null)
            {
                gamePanel.Visible = true;
                gamePanel.BringToFront();
                gamePanel.Refresh();
            }
        }
        private void OptimizeAllImages()
        {
            OptimizeImagesInContainer(HomePanel);
            OptimizeImagesInContainer(snakePanel);
            OptimizeImagesInContainer(memoryPanel);
            OptimizeImagesInContainer(dinoPanel);
            OptimizeImagesInContainer(flappyBirdPanel);
            OptimizeImagesInContainer(hillPanel);
            OptimizeImagesInContainer(TetrisPanel);
            // OptimizeImagesInContainer(hillClimbPanel);
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

        private void pictureDino_Click(object sender, EventArgs e)
        {
            ShowGamePanel(dinoPanel);
        }

        private void SnakePictureClick(object sender, EventArgs e)
        {
            ShowGamePanel(snakePanel);
        }

        private void hillPictureClick(object sender, EventArgs e)
        {
            ShowGamePanel(hillPanel);
        }
        private void pictureTetris_Click(object sender, EventArgs e)
        {
            ShowGamePanel(TetrisPanel);
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

        private void hillPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            HillClimbRacing hill = new HillClimbRacing(this);
            hill.Show();
        }

        private void tetrisPlayButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            TetrisGame tetrisGame = new TetrisGame(this);
            tetrisGame.Show();
        }



        // ========== BUTONAT BACK ==========
        private void backButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }

        private void flappyBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }

        private void memoryBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }

        private void dinoBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }
        private void hillBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }

      

        private void tetrisBackButton_Click(object sender, EventArgs e)
        {
            HideAllGamePanels();
            HomePanel.Visible = true;
            HomePanel.BringToFront();
            this.Refresh();
        }
    }
}