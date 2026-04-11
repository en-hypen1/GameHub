using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GameHub
{
    public partial class HillClimbRacing : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Timer physicsTimer;
        private Form1 mainForm;

        // Variablat e lojës
        private float carX = 100;
        private float carY = 300;
        private float carRotation = 0;
        private float carSpeed = 0;
        private float carVelocityY = 0;
        private float fuel = 100;
        private float score = 0;
        private float distance = 0;
        private float coins = 0;
        private bool isGameOver = false;
        private bool isPaused = false;
        private bool hasCrashed = false;
        private string crashMessage = "";

        // Karakteristikat e makinës
        private float gravity = 0.5f;
        private float groundHeight = 350;
        private float wheelRadius = 12;
        private float carWidth = 55;
        private float carHeight = 32;

        // Kamera
        private float cameraX = 0;
        private float worldWidth = 4000;

        // Terrain
        private PointF[] terrainPoints;
        private int terrainPointCount = 80;

        // Coins dhe Fuel
        private PointF[] coinsList;
        private bool[] coinsCollected;
        private PointF[] fuelCans;
        private bool[] fuelCollected;
        private Random rand = new Random();

        // Kontrolli
        private bool keyLeft = false;
        private bool keyRight = false;
        private bool keyUp = false;

        // Animacioni
        private float wheelRotation = 0;
        private float headAngle = 0;
        private float neckOffset = 0;

        // Efektet
        private float shakeAmount = 0;
        private float exhaustX = 0;
        private float exhaustY = 0;
        private bool onGround = true;
        private float timeSinceFlip = 0;

        public HillClimbRacing(Form1 main)
        {
            mainForm = main;
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializeGame();

            this.Size = new Size(1024, 600);
            this.BackColor = Color.LightSkyBlue;
            this.KeyDown += HillClimbRacing_KeyDown;
            this.KeyUp += HillClimbRacing_KeyUp;
            this.FormClosing += HillClimbRacing_FormClosing;
            this.Paint += HillClimbRacing_Paint;
        }

        private void InitializeGame()
        {
            GenerateTerrain();
            GenerateCollectibles();
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            physicsTimer = new System.Windows.Forms.Timer { Interval = 16 };
            physicsTimer.Tick += PhysicsTimer_Tick;
            physicsTimer.Start();
        }

        private void GenerateTerrain()
        {
            terrainPoints = new PointF[terrainPointCount];
            float step = worldWidth / terrainPointCount;

            for (int i = 0; i < terrainPointCount; i++)
            {
                float x = i * step;
                float y = groundHeight;

                float heightVariation = (float)(Math.Sin(i * 0.08) * 45);
                heightVariation += (float)(Math.Sin(i * 0.25) * 25);
                heightVariation += (float)(Math.Cos(i * 0.15) * 15);

                if (i > 15 && i < 28)
                    heightVariation -= 70;
                else if (i > 32 && i < 45)
                    heightVariation += 65;
                else if (i > 50 && i < 58)
                    heightVariation -= 55;
                else if (i > 62 && i < 72)
                    heightVariation += 60;
                else if (i > 75 && i < 78)
                    heightVariation -= 40;

                y += heightVariation + rand.Next(-12, 12);
                y = Math.Max(240, Math.Min(470, y));
                terrainPoints[i] = new PointF(x, y);
            }
        }

        private void GenerateCollectibles()
        {
            int coinCount = 35;
            coinsList = new PointF[coinCount];
            coinsCollected = new bool[coinCount];

            for (int i = 0; i < coinCount; i++)
            {
                float x = 150 + rand.Next((int)(worldWidth - 300));
                float y = GetTerrainHeight(x) - 20 - rand.Next(0, 30);
                coinsList[i] = new PointF(x, y);
                coinsCollected[i] = false;
            }

            int canCount = 12;
            fuelCans = new PointF[canCount];
            fuelCollected = new bool[canCount];

            for (int i = 0; i < canCount; i++)
            {
                float x = 200 + rand.Next((int)(worldWidth - 400));
                float y = GetTerrainHeight(x) - 25;
                fuelCans[i] = new PointF(x, y);
                fuelCollected[i] = false;
            }
        }

        private float GetTerrainHeight(float x)
        {
            if (terrainPoints == null || terrainPoints.Length < 2)
                return groundHeight;

            if (x < 0) return terrainPoints[0].Y;
            if (x > worldWidth) return terrainPoints[terrainPoints.Length - 1].Y;

            float step = worldWidth / terrainPointCount;
            int index = (int)(x / step);

            if (index >= terrainPoints.Length - 1)
                return terrainPoints[terrainPoints.Length - 1].Y;

            float t = (x - (index * step)) / step;
            return terrainPoints[index].Y * (1 - t) + terrainPoints[index + 1].Y * t;
        }

        private float GetTerrainAngle(float x)
        {
            if (x < 0 || x > worldWidth)
                return 0;

            float step = worldWidth / terrainPointCount;
            int index = (int)(x / step);

            if (index >= terrainPoints.Length - 1)
                return 0;

            float height1 = terrainPoints[index].Y;
            float height2 = terrainPoints[index + 1].Y;
            return (float)Math.Atan2(height2 - height1, step);
        }

        private void PhysicsTimer_Tick(object sender, EventArgs e)
        {
            if (isGameOver || isPaused) return;

            carVelocityY += gravity;
            carY += carVelocityY;

            float acceleration = 0;
            float fuelConsumption = 0;

            if (!hasCrashed)
            {
                if (keyRight && fuel > 0)
                {
                    acceleration = 0.48f;
                    fuelConsumption = 0.18f;
                    wheelRotation += carSpeed * 9;
                    exhaustX = carX - 15;
                    exhaustY = carY + carHeight - 8;
                    headAngle = Math.Min(15, headAngle + 1);
                }
                else if (keyLeft && fuel > 0)
                {
                    acceleration = -0.4f;
                    fuelConsumption = 0.15f;
                    wheelRotation -= carSpeed * 7;
                    headAngle = Math.Max(-15, headAngle - 1);
                }
                else
                {
                    headAngle *= 0.95f;
                }

                if (keyUp && fuel > 0 && onGround && !hasCrashed)
                {
                    carVelocityY = -9;
                    fuel -= 2;
                }
            }

            fuel -= fuelConsumption;
            carSpeed = carSpeed * 0.985f + acceleration;
            carSpeed = Math.Max(-14, Math.Min(22, carSpeed));
            carX += carSpeed;
            carX = Math.Max(30, Math.Min(worldWidth - 70, carX));

            if (carSpeed < -8 && !hasCrashed && onGround)
            {
                hasCrashed = true;
                crashMessage = "BROKE HIS NECK!";
                timeSinceFlip = 0;
                shakeAmount = 15;
                score -= 500;
            }

            cameraX = carX - this.Width / 2;
            cameraX = Math.Max(0, Math.Min(worldWidth - this.Width, cameraX));

            float carCenterX = carX + carWidth / 2;
            float terrainHeight = GetTerrainHeight(carCenterX);
            float terrainAngle = GetTerrainAngle(carCenterX);
            float carBottom = carY + carHeight;

            onGround = carBottom >= terrainHeight;

            if (onGround && !hasCrashed)
            {
                carY = terrainHeight - carHeight;
                carVelocityY = 0;
                carRotation = terrainAngle * (180 / (float)Math.PI);

                float impact = Math.Abs(carSpeed) * Math.Abs((float)Math.Sin(terrainAngle));
                if (impact > 2.5f)
                {
                    shakeAmount = impact * 2.5f;
                    score -= impact * 8;
                    neckOffset = impact * 3;
                }
                shakeAmount = Math.Max(0, shakeAmount - 0.4f);
                neckOffset *= 0.9f;
                carSpeed -= (float)Math.Sin(terrainAngle) * 0.4f;
            }
            else if (!hasCrashed)
            {
                if (keyRight) carRotation += 5;
                if (keyLeft) carRotation -= 5;
                carRotation = Math.Max(-90, Math.Min(90, carRotation));

                if (Math.Abs(carRotation) > 75)
                {
                    hasCrashed = true;
                    crashMessage = "CAR FLIPPED!";
                    shakeAmount = 20;
                    score -= 300;
                }
            }

            for (int i = 0; i < coinsList.Length; i++)
            {
                if (!coinsCollected[i] &&
                    Math.Abs(carX + carWidth / 2 - coinsList[i].X) < 22 &&
                    Math.Abs(carY + carHeight / 2 - coinsList[i].Y) < 22)
                {
                    coinsCollected[i] = true;
                    coins++;
                    score += 50;
                }
            }

            for (int i = 0; i < fuelCans.Length; i++)
            {
                if (!fuelCollected[i] &&
                    Math.Abs(carX + carWidth / 2 - fuelCans[i].X) < 25 &&
                    Math.Abs(carY + carHeight / 2 - fuelCans[i].Y) < 25)
                {
                    fuelCollected[i] = true;
                    fuel = Math.Min(100, fuel + 30);
                    score += 150;
                }
            }

            if (!hasCrashed)
            {
                distance += carSpeed * 0.12f;
                score += carSpeed * 0.25f;
            }
            if (score < 0) score = 0;

            if (hasCrashed)
            {
                timeSinceFlip += 0.05f;
                if (timeSinceFlip > 2)
                {
                    isGameOver = true;
                    gameTimer.Stop();
                    physicsTimer.Stop();
                }
            }

            if (fuel <= 0 && !hasCrashed)
            {
                isGameOver = true;
                crashMessage = "OUT OF FUEL!";
                gameTimer.Stop();
                physicsTimer.Stop();
            }

            if (carY > this.Height + 150 || carY < -150)
            {
                isGameOver = true;
                crashMessage = "FELL OFF!";
                gameTimer.Stop();
                physicsTimer.Stop();
            }

            fuel = Math.Max(0, Math.Min(100, fuel));
            this.Invalidate();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void HillClimbRacing_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                float shakeX = (shakeAmount > 0) ? rand.Next(-(int)shakeAmount, (int)shakeAmount) : 0;
                float shakeY = (shakeAmount > 0) ? rand.Next(-(int)shakeAmount / 2, (int)shakeAmount / 2) : 0;

                using (LinearGradientBrush skyBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, this.Width, this.Height),
                    Color.FromArgb(135, 206, 235), Color.FromArgb(25, 25, 112), 90))
                {
                    g.FillRectangle(skyBrush, 0, 0, this.Width, this.Height);
                }

                g.FillEllipse(Brushes.Yellow, 800, 60, 80, 80);
                DrawCloud(g, 200, 80, 60, 40);
                DrawCloud(g, 500, 120, 80, 50);
                DrawCloud(g, 900, 60, 70, 45);

                var state = g.Save();
                g.TranslateTransform(-cameraX + shakeX, shakeY);

                if (terrainPoints != null && terrainPoints.Length > 1)
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLines(terrainPoints);
                        path.AddLine(terrainPoints[terrainPoints.Length - 1].X, this.Height + 100,
                                    terrainPoints[0].X, this.Height + 100);

                        using (LinearGradientBrush groundBrush = new LinearGradientBrush(
                            new Rectangle(0, (int)(groundHeight - 100), (int)worldWidth, 200),
                            Color.SaddleBrown, Color.Peru, 90))
                        {
                            g.FillPath(groundBrush, path);
                        }

                        using (Pen pen = new Pen(Color.FromArgb(101, 67, 33), 2))
                        {
                            g.DrawLines(pen, terrainPoints);
                        }
                    }

                    for (int i = 0; i < coinsList.Length; i++)
                    {
                        if (!coinsCollected[i])
                        {
                            DrawCoin(g, coinsList[i].X, coinsList[i].Y);
                        }
                    }

                    for (int i = 0; i < fuelCans.Length; i++)
                    {
                        if (!fuelCollected[i])
                        {
                            DrawFuelCan(g, fuelCans[i].X, fuelCans[i].Y);
                        }
                    }

                    // Vizato makinën me të gjitha pjesët
                    DrawCompleteCar(g, carX, carY, carRotation, wheelRotation, headAngle, neckOffset, hasCrashed);

                    if (keyRight && carSpeed > 2 && fuel > 0 && !hasCrashed)
                    {
                        int smokeSize = rand.Next(5, 12);
                        using (SolidBrush smokeBrush = new SolidBrush(Color.FromArgb(100, 128, 128, 128)))
                        {
                            g.FillEllipse(smokeBrush, exhaustX - cameraX - smokeSize / 2, exhaustY - smokeSize / 2, smokeSize, smokeSize);
                        }
                    }
                }

                g.Restore(state);
                DrawUI(g);
            }
            catch (Exception ex)
            {
                DrawUI(g);
            }
        }

        private void DrawCompleteCar(Graphics g, float x, float y, float rotation, float wheelRot, float headAng, float neck, bool crashed)
        {
            // Rrotat e para (para transformimit)
            DrawWheel(g, x + 10, y + carHeight - 5, wheelRot);
            DrawWheel(g, x + carWidth - 18, y + carHeight - 5, wheelRot);

            // Trupi i makinës me transformim
            g.TranslateTransform(x + carWidth / 2, y + carHeight / 2);
            g.RotateTransform(rotation);

            using (GraphicsPath carBody = new GraphicsPath())
            {
                carBody.AddRectangle(new Rectangle(-(int)(carWidth / 2), -(int)(carHeight / 2), (int)carWidth, (int)carHeight));
                carBody.AddEllipse(-(int)(carWidth / 2) + 5, -(int)(carHeight / 2) - 5, 15, 12);
                carBody.AddEllipse((int)(carWidth / 2) - 20, -(int)(carHeight / 2) - 5, 15, 12);

                using (LinearGradientBrush carBrush = new LinearGradientBrush(
                    new Rectangle(-(int)(carWidth / 2), -(int)(carHeight / 2), (int)carWidth, (int)carHeight),
                    Color.FromArgb(220, 20, 60), Color.FromArgb(139, 0, 0), 45))
                {
                    g.FillPath(carBrush, carBody);
                }
                using (Pen pen = new Pen(Color.DarkRed, 2))
                {
                    g.DrawPath(pen, carBody);
                }
            }

            // Dritaret
            g.FillRectangle(Brushes.LightBlue, -(int)(carWidth / 2) + 8, -(int)(carHeight / 2) + 3, 15, 12);
            g.FillRectangle(Brushes.LightBlue, (int)(carWidth / 2) - 23, -(int)(carHeight / 2) + 3, 15, 12);

            g.ResetTransform();

            // PERSONAZHI (vizatohet pas transformimit)
            float headX = x + 15;
            float headY = y + 8;

            if (crashed)
            {
                g.DrawLine(new Pen(Color.Black, 2), headX + 3, headY + 8, headX + 10, headY + 25);
                g.DrawLine(new Pen(Color.Black, 2), headX + 12, headY + 8, headX + 5, headY + 25);
                g.FillEllipse(Brushes.LightYellow, headX, headY + 5, 15, 15);
                g.DrawLine(new Pen(Color.Black, 2), headX + 5, headY + 12, headX + 10, headY + 12);
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString("X", font, Brushes.Red, headX + 4, headY + 9);
                    g.DrawString("X", font, Brushes.Red, headX + 10, headY + 9);
                }
            }
            else
            {
                float neckX = headX + 7 + (neck * 0.5f);
                float neckY = headY + 8;

                g.DrawLine(new Pen(Color.Black, 2), headX + 7, headY + 10, neckX, neckY);
                g.FillEllipse(Brushes.LightYellow, headX + (headAng * 0.3f), headY, 14, 14);

                g.FillEllipse(Brushes.Black, headX + 4 + (headAng * 0.2f), headY + 4, 3, 3);
                g.FillEllipse(Brushes.Black, headX + 10 + (headAng * 0.2f), headY + 4, 3, 3);

                if (carSpeed > 5)
                {
                    g.DrawArc(new Pen(Color.Black, 2), headX + 5, headY + 7, 8, 5, 0, 180);
                }
                else if (carSpeed < -3)
                {
                    g.DrawArc(new Pen(Color.Black, 2), headX + 5, headY + 9, 8, 5, 180, 180);
                }
                else
                {
                    g.DrawLine(new Pen(Color.Black, 2), headX + 5, headY + 10, headX + 12, headY + 10);
                }
            }
        }

        private void DrawWheel(Graphics g, float x, float y, float rotation)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(rotation);
            g.FillEllipse(Brushes.Black, -wheelRadius, -wheelRadius, wheelRadius * 2, wheelRadius * 2);
            g.DrawEllipse(Pens.Gray, -wheelRadius, -wheelRadius, wheelRadius * 2, wheelRadius * 2);

            using (Pen pen = new Pen(Color.Gray, 1.5f))
            {
                g.DrawLine(pen, 0, -wheelRadius + 3, 0, wheelRadius - 3);
                g.DrawLine(pen, -wheelRadius + 3, 0, wheelRadius - 3, 0);
            }
            g.ResetTransform();
        }

        private void DrawCoin(Graphics g, float x, float y)
        {
            g.FillEllipse(Brushes.Gold, x - cameraX, y, 12, 12);
            g.DrawEllipse(Pens.Orange, x - cameraX, y, 12, 12);
            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                g.DrawString("★", font, Brushes.Yellow, x - cameraX + 3, y + 1);
            }
        }

        private void DrawFuelCan(Graphics g, float x, float y)
        {
            g.FillRectangle(Brushes.DarkGreen, x - cameraX, y, 15, 20);
            g.FillRectangle(Brushes.Green, x - cameraX + 5, y - 5, 5, 5);
            using (Font font = new Font("Segoe UI", 10))
            {
                g.DrawString("⛽", font, Brushes.White, x - cameraX + 2, y + 2);
            }
        }

        private void DrawCloud(Graphics g, float x, float y, float width, float height)
        {
            using (SolidBrush cloudBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.FillEllipse(cloudBrush, x, y, width, height);
                g.FillEllipse(cloudBrush, x + width * 0.5f, y - height * 0.3f, width * 0.7f, height * 0.8f);
                g.FillEllipse(cloudBrush, x - width * 0.2f, y - height * 0.2f, width * 0.6f, height * 0.7f);
            }
        }

        private void DrawUI(Graphics g)
        {
            using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(darkBrush, 0, 0, 280, 130);
                g.FillRectangle(darkBrush, this.Width - 210, 0, 210, 85);
            }

            using (Font font = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString($"SCORE: {(int)score}", font, Brushes.Gold, 15, 15);
                g.DrawString($"COINS: {(int)coins}", font, Brushes.Yellow, 15, 45);
                g.DrawString($"DISTANCE: {(int)distance}m", font, Brushes.White, 15, 75);
                g.DrawString($"SPEED: {Math.Abs(carSpeed * 12):F0} km/h", font, Brushes.Cyan, 15, 105);
            }

            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            {
                g.DrawString("FUEL", font, Brushes.White, this.Width - 190, 15);
                g.FillRectangle(Brushes.Gray, this.Width - 190, 40, 160, 20);
                int fuelWidth = (int)(160 * (fuel / 100));
                using (SolidBrush fuelBrush = new SolidBrush(fuel > 30 ? Color.LimeGreen : Color.OrangeRed))
                {
                    g.FillRectangle(fuelBrush, this.Width - 190, 40, fuelWidth, 20);
                }
                g.DrawString($"{fuel:F0}%", font, Brushes.White, this.Width - 85, 42);
            }

            using (Font smallFont = new Font("Arial", 10, FontStyle.Regular))
            {
                g.DrawString("← →  :  Drive", smallFont, Brushes.LightGray, 15, this.Height - 80);
                g.DrawString("↑  :  Jump", smallFont, Brushes.LightGray, 15, this.Height - 60);
                g.DrawString("R  :  Restart", smallFont, Brushes.LightGray, 15, this.Height - 40);
            }

            if (hasCrashed && !isGameOver)
            {
                using (Font crashFont = new Font("Arial", 24, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(crashMessage, crashFont);
                    g.FillRectangle(Brushes.Black, (this.Width - textSize.Width) / 2 - 20, this.Height / 2 - 60, textSize.Width + 40, 70);
                    g.DrawString(crashMessage, crashFont, Brushes.Red, (this.Width - textSize.Width) / 2, this.Height / 2 - 40);
                }
            }

            if (isGameOver)
            {
                using (Font bigFont = new Font("Arial", 32, FontStyle.Bold))
                using (Font smallFont = new Font("Arial", 18, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString("GAME OVER", bigFont);
                    g.FillRectangle(Brushes.Black, (this.Width - textSize.Width) / 2 - 20, this.Height / 2 - 100, textSize.Width + 40, 180);
                    g.DrawString("GAME OVER", bigFont, Brushes.Red, (this.Width - textSize.Width) / 2, this.Height / 2 - 80);
                    g.DrawString($"{crashMessage}", smallFont, Brushes.Yellow, (this.Width - 150) / 2, this.Height / 2 - 20);
                    g.DrawString($"Score: {(int)score}  |  Coins: {(int)coins}", smallFont, Brushes.White, (this.Width - 220) / 2, this.Height / 2 + 30);
                    g.DrawString("Press R to restart", smallFont, Brushes.LightGray, (this.Width - 150) / 2, this.Height / 2 + 70);
                }
            }
        }

        private void HillClimbRacing_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    keyRight = true;
                    break;
                case Keys.Left:
                    keyLeft = true;
                    break;
                case Keys.Up:
                    keyUp = true;
                    break;
                case Keys.R:
                    RestartGame();
                    break;
                case Keys.P:
                    isPaused = !isPaused;
                    break;
            }
        }

        private void HillClimbRacing_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    keyRight = false;
                    break;
                case Keys.Left:
                    keyLeft = false;
                    break;
                case Keys.Up:
                    keyUp = false;
                    break;
            }
        }

        private void RestartGame()
        {
            carX = 100;
            carY = 300;
            carRotation = 0;
            carSpeed = 0;
            carVelocityY = 0;
            fuel = 100;
            score = 0;
            distance = 0;
            coins = 0;
            isGameOver = false;
            hasCrashed = false;
            cameraX = 0;
            shakeAmount = 0;
            wheelRotation = 0;
            headAngle = 0;
            neckOffset = 0;
            timeSinceFlip = 0;

            GenerateTerrain();
            GenerateCollectibles();

            if (!gameTimer.Enabled)
                gameTimer.Start();
            if (!physicsTimer.Enabled)
                physicsTimer.Start();
        }

        private void HillClimbRacing_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            physicsTimer?.Stop();
            mainForm?.Show();
        }

        private void HillClimbRacing_Load(object sender, EventArgs e) { }
    }
}