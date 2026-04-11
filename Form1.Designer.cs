namespace GameHub
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            snakePanel = new Panel();
            playButton = new PictureBox();
            backButton = new PictureBox();
            panelGames = new Panel();
            panel1 = new Panel();
            label4 = new Label();
            pictureHillClimbHome = new PictureBox();
            panelTicTacToe = new Panel();
            pictureTicTacToe = new PictureBox();
            label3 = new Label();
            panelFlappy = new Panel();
            pictureFlappy = new PictureBox();
            flappyLabel = new Label();
            panelMemory = new Panel();
            label5 = new Label();
            pictureMemory = new PictureBox();
            panelSnale = new Panel();
            label2 = new Label();
            pictureSnake = new PictureBox();
            Home = new Panel();
            label1 = new Label();
            flappyBirdPanel = new Panel();
            flappyPlayButton = new PictureBox();
            flappyBackButton = new PictureBox();
            memoryPanel = new Panel();
            memoryPlayButton = new PictureBox();
            memoryBackButton = new PictureBox();
            dinoPanel = new Panel();
            dinoPlayButton = new PictureBox();
            dinoBackButton = new PictureBox();
            snakePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)playButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)backButton).BeginInit();
            panelGames.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureHillClimbHome).BeginInit();
            panelTicTacToe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTicTacToe).BeginInit();
            panelFlappy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureFlappy).BeginInit();
            panelMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMemory).BeginInit();
            panelSnale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureSnake).BeginInit();
            Home.SuspendLayout();
            flappyBirdPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)flappyPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)flappyBackButton).BeginInit();
            memoryPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)memoryPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoryBackButton).BeginInit();
            dinoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dinoPlayButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dinoBackButton).BeginInit();
            SuspendLayout();
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
            snakePanel.Size = new Size(978, 484);
            snakePanel.TabIndex = 10;
            snakePanel.Paint += snakePanel_Paint;
            // 
            // playButton
            // 
            playButton.Anchor = AnchorStyles.Bottom;
            playButton.BackColor = Color.Transparent;
            playButton.BackgroundImage = (Image)resources.GetObject("playButton.BackgroundImage");
            playButton.BackgroundImageLayout = ImageLayout.Stretch;
            playButton.Location = new Point(311, 333);
            playButton.Name = "playButton";
            playButton.Size = new Size(373, 120);
            playButton.SizeMode = PictureBoxSizeMode.CenterImage;
            playButton.TabIndex = 0;
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
            // panelGames
            // 
            panelGames.Anchor = AnchorStyles.None;
            panelGames.Controls.Add(panel1);
            panelGames.Controls.Add(panelTicTacToe);
            panelGames.Controls.Add(panelFlappy);
            panelGames.Controls.Add(panelMemory);
            panelGames.Controls.Add(panelSnale);
            panelGames.Location = new Point(0, 0);
            panelGames.Name = "panelGames";
            panelGames.Size = new Size(978, 484);
            panelGames.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(label4);
            panel1.Controls.Add(pictureHillClimbHome);
            panel1.Location = new Point(758, 111);
            panel1.Margin = new Padding(10);
            panel1.Name = "panel1";
            panel1.Size = new Size(167, 330);
            panel1.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.WhiteSmoke;
            label4.Dock = DockStyle.Bottom;
            label4.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ActiveCaptionText;
            label4.Location = new Point(0, 256);
            label4.Name = "label4";
            label4.Size = new Size(193, 37);
            label4.TabIndex = 9;
            label4.Text = "Memory Game";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // pictureHillClimbHome
            // 
            pictureHillClimbHome.Image = (Image)resources.GetObject("pictureHillClimbHome.Image");
            pictureHillClimbHome.Location = new Point(0, 0);
            pictureHillClimbHome.Name = "pictureHillClimbHome";
            pictureHillClimbHome.Size = new Size(171, 238);
            pictureHillClimbHome.SizeMode = PictureBoxSizeMode.Zoom;
            pictureHillClimbHome.TabIndex = 7;
            pictureHillClimbHome.TabStop = false;
            pictureHillClimbHome.Click += pictureHillClimbHome_Click_1;
            // 
            // panelTicTacToe
            // 
            panelTicTacToe.Controls.Add(pictureTicTacToe);
            panelTicTacToe.Controls.Add(label3);
            panelTicTacToe.Location = new Point(197, 111);
            panelTicTacToe.Margin = new Padding(10);
            panelTicTacToe.Name = "panelTicTacToe";
            panelTicTacToe.Size = new Size(178, 330);
            panelTicTacToe.TabIndex = 6;
            // 
            // pictureTicTacToe
            // 
            pictureTicTacToe.Image = (Image)resources.GetObject("pictureTicTacToe.Image");
            pictureTicTacToe.Location = new Point(0, 0);
            pictureTicTacToe.Name = "pictureTicTacToe";
            pictureTicTacToe.Size = new Size(179, 238);
            pictureTicTacToe.SizeMode = PictureBoxSizeMode.Zoom;
            pictureTicTacToe.TabIndex = 7;
            pictureTicTacToe.TabStop = false;
            pictureTicTacToe.Click += pictureTicTacToe_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.WhiteSmoke;
            label3.Dock = DockStyle.Bottom;
            label3.Font = new Font("Segoe UI", 22F);
            label3.ForeColor = SystemColors.ActiveCaptionText;
            label3.Location = new Point(0, 289);
            label3.Name = "label3";
            label3.Size = new Size(160, 41);
            label3.TabIndex = 8;
            label3.Text = "Tic Tac Toe";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelFlappy
            // 
            panelFlappy.Controls.Add(pictureFlappy);
            panelFlappy.Controls.Add(flappyLabel);
            panelFlappy.Location = new Point(370, 111);
            panelFlappy.Margin = new Padding(10);
            panelFlappy.Name = "panelFlappy";
            panelFlappy.Size = new Size(176, 330);
            panelFlappy.TabIndex = 7;
            // 
            // pictureFlappy
            // 
            pictureFlappy.Image = (Image)resources.GetObject("pictureFlappy.Image");
            pictureFlappy.Location = new Point(3, 0);
            pictureFlappy.Name = "pictureFlappy";
            pictureFlappy.Size = new Size(176, 238);
            pictureFlappy.SizeMode = PictureBoxSizeMode.Zoom;
            pictureFlappy.TabIndex = 7;
            pictureFlappy.TabStop = false;
            pictureFlappy.Click += pictureFlappy_Click;
            // 
            // flappyLabel
            // 
            flappyLabel.AutoSize = true;
            flappyLabel.BackColor = Color.WhiteSmoke;
            flappyLabel.Dock = DockStyle.Bottom;
            flappyLabel.Font = new Font("Segoe UI", 22F);
            flappyLabel.ForeColor = SystemColors.ActiveCaptionText;
            flappyLabel.Location = new Point(0, 289);
            flappyLabel.Name = "flappyLabel";
            flappyLabel.Size = new Size(166, 41);
            flappyLabel.TabIndex = 8;
            flappyLabel.Text = "Flappy Bird";
            flappyLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelMemory
            // 
            panelMemory.Controls.Add(label5);
            panelMemory.Controls.Add(pictureMemory);
            panelMemory.Location = new Point(562, 111);
            panelMemory.Margin = new Padding(10);
            panelMemory.Name = "panelMemory";
            panelMemory.Size = new Size(167, 330);
            panelMemory.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.WhiteSmoke;
            label5.Dock = DockStyle.Bottom;
            label5.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ActiveCaptionText;
            label5.Location = new Point(0, 256);
            label5.Name = "label5";
            label5.Size = new Size(193, 37);
            label5.TabIndex = 9;
            label5.Text = "Memory Game";
            label5.TextAlign = ContentAlignment.TopCenter;
            // 
            // pictureMemory
            // 
            pictureMemory.Image = (Image)resources.GetObject("pictureMemory.Image");
            pictureMemory.Location = new Point(0, 0);
            pictureMemory.Name = "pictureMemory";
            pictureMemory.Size = new Size(171, 238);
            pictureMemory.SizeMode = PictureBoxSizeMode.Zoom;
            pictureMemory.TabIndex = 7;
            pictureMemory.TabStop = false;
            pictureMemory.Click += pictureMemory_Click;
            // 
            // panelSnale
            // 
            panelSnale.Controls.Add(label2);
            panelSnale.Controls.Add(pictureSnake);
            panelSnale.Location = new Point(15, 111);
            panelSnale.Margin = new Padding(10);
            panelSnale.Name = "panelSnale";
            panelSnale.Size = new Size(180, 330);
            panelSnale.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.WhiteSmoke;
            label2.Dock = DockStyle.Bottom;
            label2.Font = new Font("Segoe UI", 22F);
            label2.ForeColor = SystemColors.ActiveCaptionText;
            label2.Location = new Point(0, 248);
            label2.Name = "label2";
            label2.Size = new Size(183, 41);
            label2.TabIndex = 9;
            label2.Text = "Snake Game";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // pictureSnake
            // 
            pictureSnake.Image = (Image)resources.GetObject("pictureSnake.Image");
            pictureSnake.Location = new Point(0, 0);
            pictureSnake.Name = "pictureSnake";
            pictureSnake.Size = new Size(183, 238);
            pictureSnake.SizeMode = PictureBoxSizeMode.Zoom;
            pictureSnake.TabIndex = 7;
            pictureSnake.TabStop = false;
            pictureSnake.Click += SnakePictureClick;
            // 
            // Home
            // 
            Home.BackColor = Color.Transparent;
            Home.BackgroundImage = (Image)resources.GetObject("Home.BackgroundImage");
            Home.BackgroundImageLayout = ImageLayout.Stretch;
            Home.Controls.Add(label1);
            Home.Controls.Add(panelGames);
            Home.Dock = DockStyle.Fill;
            Home.Location = new Point(0, 0);
            Home.Name = "Home";
            Home.Size = new Size(978, 484);
            Home.TabIndex = 12;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Broadway", 34F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(332, 19);
            label1.Name = "label1";
            label1.Size = new Size(283, 52);
            label1.TabIndex = 3;
            label1.Text = "GAME HUB";
            label1.TextAlign = ContentAlignment.TopCenter;
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
            flappyBirdPanel.Size = new Size(978, 484);
            flappyBirdPanel.TabIndex = 13;
            flappyBirdPanel.Click += backButton_Click;
            // 
            // flappyPlayButton
            // 
            flappyPlayButton.Anchor = AnchorStyles.Bottom;
            flappyPlayButton.BackColor = Color.Transparent;
            flappyPlayButton.BackgroundImage = (Image)resources.GetObject("flappyPlayButton.BackgroundImage");
            flappyPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            flappyPlayButton.Location = new Point(292, 377);
            flappyPlayButton.Name = "flappyPlayButton";
            flappyPlayButton.Size = new Size(373, 120);
            flappyPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            flappyPlayButton.TabIndex = 2;
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
            flappyBackButton.Click += backButton_Click;
            // 
            // memoryPanel
            // 
            memoryPanel.BackgroundImage = (Image)resources.GetObject("memoryPanel.BackgroundImage");
            memoryPanel.BackgroundImageLayout = ImageLayout.Stretch;
            memoryPanel.Controls.Add(memoryPlayButton);
            memoryPanel.Controls.Add(memoryBackButton);
            memoryPanel.Dock = DockStyle.Fill;
            memoryPanel.Location = new Point(0, 0);
            memoryPanel.Name = "memoryPanel";
            memoryPanel.Size = new Size(978, 484);
            memoryPanel.TabIndex = 14;
            memoryPanel.Click += backButton_Click;
            // 
            // memoryPlayButton
            // 
            memoryPlayButton.Anchor = AnchorStyles.Bottom;
            memoryPlayButton.BackColor = Color.Transparent;
            memoryPlayButton.BackgroundImage = (Image)resources.GetObject("memoryPlayButton.BackgroundImage");
            memoryPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            memoryPlayButton.Location = new Point(311, 377);
            memoryPlayButton.Name = "memoryPlayButton";
            memoryPlayButton.Size = new Size(373, 120);
            memoryPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            memoryPlayButton.TabIndex = 2;
            memoryPlayButton.TabStop = false;
            memoryPlayButton.Click += memoryPlayButton_Click;
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
            dinoPanel.Size = new Size(978, 484);
            dinoPanel.TabIndex = 15;
            // 
            // dinoPlayButton
            // 
            dinoPlayButton.Anchor = AnchorStyles.Bottom;
            dinoPlayButton.BackColor = Color.Transparent;
            dinoPlayButton.BackgroundImage = (Image)resources.GetObject("dinoPlayButton.BackgroundImage");
            dinoPlayButton.BackgroundImageLayout = ImageLayout.Stretch;
            dinoPlayButton.Location = new Point(320, 251);
            dinoPlayButton.Name = "dinoPlayButton";
            dinoPlayButton.Size = new Size(373, 120);
            dinoPlayButton.SizeMode = PictureBoxSizeMode.CenterImage;
            dinoPlayButton.TabIndex = 3;
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
            dinoBackButton.Click += backButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(978, 484);
            Controls.Add(flappyBirdPanel);
            Controls.Add(dinoPanel);
            Controls.Add(Home);
            Controls.Add(snakePanel);
            Controls.Add(memoryPanel);
            DoubleBuffered = true;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GameHub";
            Load += Form1_Load;
            snakePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)playButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)backButton).EndInit();
            panelGames.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureHillClimbHome).EndInit();
            panelTicTacToe.ResumeLayout(false);
            panelTicTacToe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureTicTacToe).EndInit();
            panelFlappy.ResumeLayout(false);
            panelFlappy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureFlappy).EndInit();
            panelMemory.ResumeLayout(false);
            panelMemory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureMemory).EndInit();
            panelSnale.ResumeLayout(false);
            panelSnale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureSnake).EndInit();
            Home.ResumeLayout(false);
            Home.PerformLayout();
            flappyBirdPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)flappyPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)flappyBackButton).EndInit();
            memoryPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)memoryPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoryBackButton).EndInit();
            dinoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dinoPlayButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)dinoBackButton).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel snakePanel;
        private PictureBox backButton;
        private PictureBox playButton;
        private Panel panelGames;
        private Panel Home;
        private Panel panelSnale;
        private PictureBox pictureSnake;
        private Panel panelMemory;
        private Label label5;
        private PictureBox pictureMemory;
        private Panel panelFlappy;
        private PictureBox pictureFlappy;
        private Label flappyLabel;
        private Panel panelTicTacToe;
        private PictureBox pictureTicTacToe;
        private Label label3;
        private Label label2;
        private Panel flappyBirdPanel;
        private PictureBox flappyBackButton;
        private PictureBox flappyPlayButton;
        private Panel memoryPanel;
        private PictureBox memoryPlayButton;
        private PictureBox memoryBackButton;
        private Panel dinoPanel;
        private PictureBox dinoPlayButton;
        private PictureBox dinoBackButton;
        private Label label1;
        private Panel panel1;
        private Label label4;
        private PictureBox pictureHillClimbHome;
    }
}
