namespace GameHub
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            pictureSnake = new PictureBox();
            pictureFlappy = new PictureBox();
            pictureMemory = new PictureBox();
            pictureDino = new PictureBox();
            HomePanel = new Panel();
            pictureTetris = new PictureBox();
            pictureHill = new PictureBox();
            label1 = new Label();
            snakePanel = new Panel();
            playButton = new PictureBox();
            backButton = new PictureBox();
            dinoPanel = new Panel();
            dinoPlayButton = new PictureBox();
            dinoBackButton = new PictureBox();
            flappyBirdPanel = new Panel();
            flappyPlayButton = new PictureBox();
            flappyBackButton = new PictureBox();
            memoryPanel = new Panel();
            pictureBox1 = new PictureBox();
            memoryBackButton = new PictureBox();
            hillPanel = new Panel();
            hillPlayButton = new PictureBox();
            hillBackButton = new PictureBox();
            TetrisPanel = new Panel();
            tetrisPlayButton = new PictureBox();
            tetrisBackButton = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureSnake).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureFlappy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureMemory).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureDino).BeginInit();
            HomePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTetris).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureHill).BeginInit();
            snakePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)playButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)backButton).BeginInit();
            dinoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dinoPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dinoBackButton).BeginInit();
            flappyBirdPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)flappyPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)flappyBackButton).BeginInit();
            memoryPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoryBackButton).BeginInit();
            hillPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)hillPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)hillBackButton).BeginInit();
            TetrisPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tetrisPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tetrisBackButton).BeginInit();
            SuspendLayout();
            // 
            // pictureSnake
            // 
            pictureSnake.BackColor = Color.Transparent;
            pictureSnake.BackgroundImage = (Image)resources.GetObject("pictureSnake.BackgroundImage");
            pictureSnake.BackgroundImageLayout = ImageLayout.Stretch;
            pictureSnake.Location = new Point(149, 85);
            pictureSnake.Name = "pictureSnake";
            pictureSnake.Size = new Size(156, 187);
            pictureSnake.TabIndex = 0;
            pictureSnake.TabStop = false;
            pictureSnake.Click += pictureSnake_Click;
            // 
            // pictureFlappy
            // 
            pictureFlappy.BackColor = Color.Transparent;
            pictureFlappy.BackgroundImage = (Image)resources.GetObject("pictureFlappy.BackgroundImage");
            pictureFlappy.BackgroundImageLayout = ImageLayout.Stretch;
            pictureFlappy.Location = new Point(353, 85);
            pictureFlappy.Name = "pictureFlappy";
            pictureFlappy.Size = new Size(156, 187);
            pictureFlappy.TabIndex = 1;
            pictureFlappy.TabStop = false;
            pictureFlappy.Click += pictureFlappy_Click;
            // 
            // pictureMemory
            // 
            pictureMemory.BackColor = Color.Transparent;
            pictureMemory.BackgroundImage = (Image)resources.GetObject("pictureMemory.BackgroundImage");
            pictureMemory.BackgroundImageLayout = ImageLayout.Stretch;
            pictureMemory.Location = new Point(563, 85);
            pictureMemory.Name = "pictureMemory";
            pictureMemory.Size = new Size(156, 187);
            pictureMemory.TabIndex = 2;
            pictureMemory.TabStop = false;
            pictureMemory.Click += pictureMemory_Click;
            // 
            // pictureDino
            // 
            pictureDino.BackColor = Color.Transparent;
            pictureDino.BackgroundImage = (Image)resources.GetObject("pictureDino.BackgroundImage");
            pictureDino.BackgroundImageLayout = ImageLayout.Stretch;
            pictureDino.Location = new Point(563, 260);
            pictureDino.Name = "pictureDino";
            pictureDino.Size = new Size(156, 187);
            pictureDino.TabIndex = 3;
            pictureDino.TabStop = false;
            pictureDino.Click += pictureDino_Click;
            // 
            // HomePanel
            // 
            HomePanel.BackColor = Color.Transparent;
            HomePanel.BackgroundImage = (Image)resources.GetObject("HomePanel.BackgroundImage");
            HomePanel.BackgroundImageLayout = ImageLayout.Stretch;
            HomePanel.Controls.Add(pictureTetris);
            HomePanel.Controls.Add(pictureHill);
            HomePanel.Controls.Add(label1);
            HomePanel.Controls.Add(pictureSnake);
            HomePanel.Controls.Add(pictureFlappy);
            HomePanel.Controls.Add(pictureMemory);
            HomePanel.Controls.Add(pictureDino);
            HomePanel.Dock = DockStyle.Fill;
            HomePanel.Location = new Point(0, 0);
            HomePanel.Name = "HomePanel";
            HomePanel.Size = new Size(875, 459);
            HomePanel.TabIndex = 4;
            // 
            // pictureTetris
            // 
            pictureTetris.BackColor = Color.Transparent;
            pictureTetris.BackgroundImage = (Image)resources.GetObject("pictureTetris.BackgroundImage");
            pictureTetris.BackgroundImageLayout = ImageLayout.Stretch;
            pictureTetris.Location = new Point(353, 260);
            pictureTetris.Name = "pictureTetris";
            pictureTetris.Size = new Size(156, 187);
            pictureTetris.TabIndex = 6;
            pictureTetris.TabStop = false;
            pictureTetris.Click += pictureTetris_Click;
            // 
            // pictureHill
            // 
            pictureHill.BackColor = Color.Transparent;
            pictureHill.BackgroundImage = (Image)resources.GetObject("pictureHill.BackgroundImage");
            pictureHill.BackgroundImageLayout = ImageLayout.Stretch;
            pictureHill.Location = new Point(149, 260);
            pictureHill.Name = "pictureHill";
            pictureHill.Size = new Size(156, 187);
            pictureHill.TabIndex = 5;
            pictureHill.TabStop = false;
            pictureHill.Click += hillPictureClick;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Broadway", 34F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(302, 30);
            label1.Name = "label1";
            label1.Size = new Size(283, 52);
            label1.TabIndex = 4;
            label1.Text = "GAME HUB";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // snakePanel
            // 
            snakePanel.BackgroundImage = (Image)resources.GetObject("snakePanel.BackgroundImage");
            snakePanel.BackgroundImageLayout = ImageLayout.Stretch;
            snakePanel.Controls.Add(playButton);
            snakePanel.Controls.Add(backButton);
            snakePanel.Dock = DockStyle.Fill;
            snakePanel.Location = new Point(0, 0);
            snakePanel.Name = "snakePanel";
            snakePanel.Size = new Size(875, 459);
            snakePanel.TabIndex = 11;
            snakePanel.Click += pictureSnake_Click;
            // 
            // playButton
            // 
            playButton.Anchor = AnchorStyles.Bottom;
            playButton.BackColor = Color.Transparent;
            playButton.BackgroundImage = (Image)resources.GetObject("playButton.BackgroundImage");
            playButton.BackgroundImageLayout = ImageLayout.Stretch;
            playButton.Location = new Point(260, 327);
            playButton.Name = "playButton";
            playButton.Size = new Size(373, 120);
            playButton.SizeMode = PictureBoxSizeMode.CenterImage;
            playButton.TabIndex = 2;
            playButton.TabStop = false;
            playButton.Click += playButton_Click;
            // 
            // backButton
            // 
            backButton.BackColor = Color.Transparent;
            backButton.BackgroundImage = (Image)resources.GetObject("backButton.BackgroundImage");
            backButton.BackgroundImageLayout = ImageLayout.Stretch;
            backButton.Location = new Point(3, 3);
            backButton.Name = "backButton";
            backButton.Size = new Size(123, 117);
            backButton.SizeMode = PictureBoxSizeMode.Zoom;
            backButton.TabIndex = 1;
            backButton.TabStop = false;
            backButton.Click += backButton_Click;
            // 
            // dinoPanel
            // 
            dinoPanel.BackgroundImage = (Image)resources.GetObject("dinoPanel.BackgroundImage");
            dinoPanel.BackgroundImageLayout = ImageLayout.Stretch;
            dinoPanel.Controls.Add(dinoPlayButton);
            dinoPanel.Controls.Add(dinoBackButton);
            dinoPanel.Dock = DockStyle.Fill;
            dinoPanel.Location = new Point(0, 0);
            dinoPanel.Name = "dinoPanel";
            dinoPanel.Size = new Size(875, 459);
            dinoPanel.TabIndex = 16;
            // 
            // dinoPlayButton
            // 
            dinoPlayButton.Anchor = AnchorStyles.Bottom;
            dinoPlayButton.BackColor = Color.Transparent;
            dinoPlayButton.BackgroundImage = (Image)resources.GetObject("dinoPlayButton.BackgroundImage");
            dinoPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            dinoPlayButton.Location = new Point(250, 311);
            dinoPlayButton.Name = "dinoPlayButton";
            dinoPlayButton.Size = new Size(373, 120);
            dinoPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            dinoPlayButton.TabIndex = 5;
            dinoPlayButton.TabStop = false;
            dinoPlayButton.Click += dinoPlayButton_Click;
            // 
            // dinoBackButton
            // 
            dinoBackButton.BackColor = Color.Transparent;
            dinoBackButton.BackgroundImage = (Image)resources.GetObject("dinoBackButton.BackgroundImage");
            dinoBackButton.BackgroundImageLayout = ImageLayout.Stretch;
            dinoBackButton.Location = new Point(0, 0);
            dinoBackButton.Name = "dinoBackButton";
            dinoBackButton.Size = new Size(123, 117);
            dinoBackButton.SizeMode = PictureBoxSizeMode.Zoom;
            dinoBackButton.TabIndex = 4;
            dinoBackButton.TabStop = false;
            dinoBackButton.Click += dinoBackButton_Click;
            // 
            // flappyBirdPanel
            // 
            flappyBirdPanel.BackgroundImage = (Image)resources.GetObject("flappyBirdPanel.BackgroundImage");
            flappyBirdPanel.BackgroundImageLayout = ImageLayout.Stretch;
            flappyBirdPanel.Controls.Add(flappyPlayButton);
            flappyBirdPanel.Controls.Add(flappyBackButton);
            flappyBirdPanel.Dock = DockStyle.Fill;
            flappyBirdPanel.Location = new Point(0, 0);
            flappyBirdPanel.Name = "flappyBirdPanel";
            flappyBirdPanel.Size = new Size(875, 459);
            flappyBirdPanel.TabIndex = 17;
            // 
            // flappyPlayButton
            // 
            flappyPlayButton.Anchor = AnchorStyles.Bottom;
            flappyPlayButton.BackColor = Color.Transparent;
            flappyPlayButton.BackgroundImage = (Image)resources.GetObject("flappyPlayButton.BackgroundImage");
            flappyPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            flappyPlayButton.Location = new Point(236, 355);
            flappyPlayButton.Name = "flappyPlayButton";
            flappyPlayButton.Size = new Size(373, 120);
            flappyPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            flappyPlayButton.TabIndex = 3;
            flappyPlayButton.TabStop = false;
            flappyPlayButton.Click += flappyPlayButton_Click;
            // 
            // flappyBackButton
            // 
            flappyBackButton.BackColor = Color.Transparent;
            flappyBackButton.BackgroundImage = (Image)resources.GetObject("flappyBackButton.BackgroundImage");
            flappyBackButton.BackgroundImageLayout = ImageLayout.Stretch;
            flappyBackButton.Location = new Point(3, 3);
            flappyBackButton.Name = "flappyBackButton";
            flappyBackButton.Size = new Size(123, 117);
            flappyBackButton.SizeMode = PictureBoxSizeMode.Zoom;
            flappyBackButton.TabIndex = 1;
            flappyBackButton.TabStop = false;
            flappyBackButton.Click += flappyBackButton_Click;
            // 
            // memoryPanel
            // 
            memoryPanel.BackgroundImage = (Image)resources.GetObject("memoryPanel.BackgroundImage");
            memoryPanel.BackgroundImageLayout = ImageLayout.Stretch;
            memoryPanel.Controls.Add(pictureBox1);
            memoryPanel.Controls.Add(memoryBackButton);
            memoryPanel.Dock = DockStyle.Fill;
            memoryPanel.Location = new Point(0, 0);
            memoryPanel.Name = "memoryPanel";
            memoryPanel.Size = new Size(875, 459);
            memoryPanel.TabIndex = 18;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(260, 299);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(373, 120);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            pictureBox1.Click += memoryPlayButton_Click;
            // 
            // memoryBackButton
            // 
            memoryBackButton.BackColor = Color.Transparent;
            memoryBackButton.BackgroundImage = (Image)resources.GetObject("memoryBackButton.BackgroundImage");
            memoryBackButton.BackgroundImageLayout = ImageLayout.Stretch;
            memoryBackButton.Location = new Point(3, 3);
            memoryBackButton.Name = "memoryBackButton";
            memoryBackButton.Size = new Size(123, 117);
            memoryBackButton.SizeMode = PictureBoxSizeMode.Zoom;
            memoryBackButton.TabIndex = 1;
            memoryBackButton.TabStop = false;
            memoryBackButton.Click += memoryBackButton_Click;
            // 
            // hillPanel
            // 
            hillPanel.BackgroundImage = (Image)resources.GetObject("hillPanel.BackgroundImage");
            hillPanel.BackgroundImageLayout = ImageLayout.Stretch;
            hillPanel.Controls.Add(hillPlayButton);
            hillPanel.Controls.Add(hillBackButton);
            hillPanel.Dock = DockStyle.Fill;
            hillPanel.Location = new Point(0, 0);
            hillPanel.Name = "hillPanel";
            hillPanel.Size = new Size(875, 459);
            hillPanel.TabIndex = 19;
            // 
            // hillPlayButton
            // 
            hillPlayButton.Anchor = AnchorStyles.Bottom;
            hillPlayButton.BackColor = Color.Transparent;
            hillPlayButton.BackgroundImage = (Image)resources.GetObject("hillPlayButton.BackgroundImage");
            hillPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            hillPlayButton.Location = new Point(250, 336);
            hillPlayButton.Name = "hillPlayButton";
            hillPlayButton.Size = new Size(373, 120);
            hillPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            hillPlayButton.TabIndex = 3;
            hillPlayButton.TabStop = false;
            hillPlayButton.Click += hillPlayButton_Click;
            // 
            // hillBackButton
            // 
            hillBackButton.BackColor = Color.Transparent;
            hillBackButton.BackgroundImage = (Image)resources.GetObject("hillBackButton.BackgroundImage");
            hillBackButton.BackgroundImageLayout = ImageLayout.Stretch;
            hillBackButton.Location = new Point(3, 3);
            hillBackButton.Name = "hillBackButton";
            hillBackButton.Size = new Size(123, 117);
            hillBackButton.SizeMode = PictureBoxSizeMode.Zoom;
            hillBackButton.TabIndex = 1;
            hillBackButton.TabStop = false;
            hillBackButton.Click += hillBackButton_Click;
            // 
            // TetrisPanel
            // 
            TetrisPanel.BackgroundImage = (Image)resources.GetObject("TetrisPanel.BackgroundImage");
            TetrisPanel.BackgroundImageLayout = ImageLayout.Stretch;
            TetrisPanel.Controls.Add(tetrisPlayButton);
            TetrisPanel.Controls.Add(tetrisBackButton);
            TetrisPanel.Dock = DockStyle.Fill;
            TetrisPanel.Location = new Point(0, 0);
            TetrisPanel.Name = "TetrisPanel";
            TetrisPanel.Size = new Size(875, 459);
            TetrisPanel.TabIndex = 20;
            // 
            // tetrisPlayButton
            // 
            tetrisPlayButton.Anchor = AnchorStyles.Bottom;
            tetrisPlayButton.BackColor = Color.Transparent;
            tetrisPlayButton.BackgroundImage = (Image)resources.GetObject("tetrisPlayButton.BackgroundImage");
            tetrisPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            tetrisPlayButton.Location = new Point(236, 327);
            tetrisPlayButton.Name = "tetrisPlayButton";
            tetrisPlayButton.Size = new Size(373, 120);
            tetrisPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            tetrisPlayButton.TabIndex = 3;
            tetrisPlayButton.TabStop = false;
            tetrisPlayButton.Click += tetrisPlayButton_Click;
            // 
            // tetrisBackButton
            // 
            tetrisBackButton.BackColor = Color.Transparent;
            tetrisBackButton.BackgroundImage = (Image)resources.GetObject("tetrisBackButton.BackgroundImage");
            tetrisBackButton.BackgroundImageLayout = ImageLayout.Stretch;
            tetrisBackButton.Location = new Point(3, 3);
            tetrisBackButton.Name = "tetrisBackButton";
            tetrisBackButton.Size = new Size(123, 117);
            tetrisBackButton.SizeMode = PictureBoxSizeMode.Zoom;
            tetrisBackButton.TabIndex = 1;
            tetrisBackButton.TabStop = false;
            tetrisBackButton.Click += tetrisBackButton_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(875, 459);
            Controls.Add(HomePanel);
            Controls.Add(TetrisPanel);
            Controls.Add(hillPanel);
            Controls.Add(memoryPanel);
            Controls.Add(flappyBirdPanel);
            Controls.Add(dinoPanel);
            Controls.Add(snakePanel);
            Name = "Main";
            Text = " ";
            Load += Main_Load;
            Resize += Main_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureSnake).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureFlappy).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureMemory).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureDino).EndInit();
            HomePanel.ResumeLayout(false);
            HomePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTetris).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureHill).EndInit();
            snakePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)playButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)backButton).EndInit();
            dinoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dinoPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)dinoBackButton).EndInit();
            flappyBirdPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)flappyPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)flappyBackButton).EndInit();
            memoryPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoryBackButton).EndInit();
            hillPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)hillPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)hillBackButton).EndInit();
            TetrisPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tetrisPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)tetrisBackButton).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureSnake;
        private PictureBox pictureFlappy;
        private PictureBox pictureMemory;
        private PictureBox pictureDino;
        private Panel HomePanel;
        private Panel snakePanel;
        private PictureBox backButton;
        private PictureBox playButton;
        private Panel dinoPanel;
        private PictureBox dinoBackButton;
        private PictureBox dinoPlayButton;
        private Panel flappyBirdPanel;
        private PictureBox flappyBackButton;
        private PictureBox flappyPlayButton;
        private Panel memoryPanel;
        private PictureBox memoryBackButton;
        private PictureBox pictureBox1;
        private Label label1;
        private PictureBox pictureHill;
        private Panel hillPanel;
        private PictureBox hillPlayButton;
        private PictureBox hillBackButton;
        private PictureBox pictureTetris;
        private Panel TetrisPanel;
        private PictureBox tetrisPlayButton;
        private PictureBox tetrisBackButton;
    }
}