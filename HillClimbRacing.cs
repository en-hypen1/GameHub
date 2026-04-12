using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GameHub
{
    public partial class HillClimbRacing : Form
    {
        private System.Windows.Forms.Timer gameTimer;
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
        private bool hasWon = false;
        private string crashMessage = "";

        // Karakteristikat e makinës
        private float gravity = 0.5f;
        private float groundHeight = 350;
        private float wheelRadius = 14;
        private float carWidth = 60;
        private float carHeight = 28;

        // Kamera
        private float cameraX = 0;
        private float worldWidth = 12000; // 3x më gjatë!

        // Finish line
        private float finishX = 11500; // afër fundit

        // Terrain
        private PointF[] terrainPoints;
        private int terrainPointCount = 220; // më shumë pika për botë më të madhe

        // Coins dhe Fuel
        private PointF[] coinsList;
        private bool[] coinsCollected;
        private PointF[] fuelCans;
        private bool[] fuelCollected;
        private Random rand = new Random();

        // Kontrolli
        private bool keyLeft = false;
        private bool keyRight = false;

        // Animacioni
        private float wheelRotation = 0;
        private float headAngle = 0;
        private float neckOffset = 0;

        // Efektet
        private float shakeAmount = 0;
        private bool onGround = true;
        private float timeSinceFlip = 0;

        // Win animacion
        private float winTimer = 0;
        private float flagWave = 0;

        // Font-et si fields
        private Font fontUI = new Font("Arial", 14, FontStyle.Bold);
        private Font fontFuel = new Font("Arial", 12, FontStyle.Bold);
        private Font fontSmall = new Font("Arial", 10);
        private Font fontCrash = new Font("Arial", 24, FontStyle.Bold);
        private Font fontBig = new Font("Arial", 32, FontStyle.Bold);
        private Font fontMed = new Font("Arial", 18, FontStyle.Bold);
        private Font fontWin = new Font("Arial", 36, FontStyle.Bold);

        public HillClimbRacing(Form1 main)
        {
            mainForm = main;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            this.Size = new Size(1024, 600);
            this.BackColor = Color.LightSkyBlue;
            this.KeyDown += HillClimbRacing_KeyDown;
            this.KeyUp += HillClimbRacing_KeyUp;
            this.FormClosing += HillClimbRacing_FormClosing;

            InitializeGame();
        }

        private void InitializeGame()
        {
            GenerateTerrain();
            GenerateCollectibles();

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GenerateTerrain()
        {
            terrainPoints = new PointF[terrainPointCount];
            float step = worldWidth / terrainPointCount;

            for (int i = 0; i < terrainPointCount; i++)
            {
                float x = i * step;
                float y = groundHeight;

                // Terreni bëhet gradualisht më i vështirë
                float difficulty = 1.0f + (i / (float)terrainPointCount) * 1.5f;

                float h = (float)(Math.Sin(i * 0.08) * 45 * difficulty);
                h += (float)(Math.Sin(i * 0.25) * 25 * difficulty);
                h += (float)(Math.Cos(i * 0.15) * 15);
                h += rand.Next(-10, 10);

                // Zona speciale të terrenit - të shpërndara gjatë gjithë botës
                int zone = i % 55;
                if (zone > 5 && zone < 15) h -= 80 * difficulty;  // kodër lart
                else if (zone > 20 && zone < 32) h += 70 * difficulty;  // luginë
                else if (zone > 38 && zone < 46) h -= 60 * difficulty;  // kodër
                else if (zone > 48 && zone < 53) h += 50 * difficulty;  // fund

                // Zona e finishit - bëje të rrafshët
                if (i > terrainPointCount - 15)
                    y = groundHeight + rand.Next(-5, 5);
                else
                    y += h;

                y = Math.Max(200, Math.Min(470, y));
                terrainPoints[i] = new PointF(x, y);
            }
        }

        private void GenerateCollectibles()
        {
            // Shumë më shumë coins dhe fuel për botë 3x më të madhe
            int coinCount = 120;
            coinsList = new PointF[coinCount];
            coinsCollected = new bool[coinCount];
            for (int i = 0; i < coinCount; i++)
            {
                float x = 150 + rand.Next((int)(worldWidth - 800));
                float y = GetTerrainHeight(x) - 25 - rand.Next(0, 35);
                coinsList[i] = new PointF(x, y);
                coinsCollected[i] = false;
            }

            int canCount = 30;
            fuelCans = new PointF[canCount];
            fuelCollected = new bool[canCount];
            for (int i = 0; i < canCount; i++)
            {
                float x = 300 + i * (int)((worldWidth - 600) / canCount);
                float y = GetTerrainHeight(x) - 28;
                fuelCans[i] = new PointF(x, y);
                fuelCollected[i] = false;
            }
        }

        private float GetTerrainHeight(float x)
        {
            if (terrainPoints == null || terrainPoints.Length < 2) return groundHeight;
            if (x <= 0) return terrainPoints[0].Y;
            if (x >= worldWidth) return terrainPoints[terrainPoints.Length - 1].Y;

            float step = worldWidth / terrainPointCount;
            int idx = (int)(x / step);
            if (idx >= terrainPoints.Length - 1)
                return terrainPoints[terrainPoints.Length - 1].Y;

            float t = (x - idx * step) / step;
            return terrainPoints[idx].Y * (1 - t) + terrainPoints[idx + 1].Y * t;
        }

        private float GetTerrainAngle(float x)
        {
            if (x < 0 || x > worldWidth) return 0;
            float step = worldWidth / terrainPointCount;
            int idx = (int)(x / step);
            if (idx >= terrainPoints.Length - 1) return 0;
            return (float)Math.Atan2(
                terrainPoints[idx + 1].Y - terrainPoints[idx].Y, step);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (isGameOver || isPaused) { this.Invalidate(); return; }

            if (hasWon)
            {
                winTimer += 0.05f;
                flagWave = (float)Math.Sin(winTimer * 3) * 8f;
                this.Invalidate();
                return;
            }

            // ── FIZIKA ───────────────────────────────────────────
            carVelocityY += gravity;
            carY += carVelocityY;

            float acceleration = 0;
            float fuelConsumption = 0;

            if (!hasCrashed)
            {
                if (keyRight && fuel > 0)
                {
                    acceleration = 0.5f;
                    fuelConsumption = 0.18f;
                    wheelRotation += carSpeed * 9;
                    headAngle = Math.Min(15, headAngle + 1);
                }
                else if (keyLeft && fuel > 0)
                {
                    acceleration = -0.4f;
                    fuelConsumption = 0.15f;
                    wheelRotation -= Math.Abs(carSpeed) * 7;
                    headAngle = Math.Max(-15, headAngle - 1);
                }
                else
                {
                    headAngle *= 0.92f;
                }
            }

            fuel -= fuelConsumption;
            carSpeed = carSpeed * 0.985f + acceleration;
            carSpeed = Math.Max(-14, Math.Min(22, carSpeed));
            carX += carSpeed;
            carX = Math.Max(30, Math.Min(worldWidth - 70, carX));

            // Kamera
            cameraX = carX - this.Width / 2f;
            cameraX = Math.Max(0, Math.Min(worldWidth - this.Width, cameraX));

            // Toka
            float carCenterX = carX + carWidth / 2;
            float terrainH = GetTerrainHeight(carCenterX);
            float terrainAngle = GetTerrainAngle(carCenterX);
            float carBottom = carY + carHeight + wheelRadius;

            onGround = carBottom >= terrainH;

            if (onGround && !hasCrashed)
            {
                carY = terrainH - carHeight - wheelRadius;
                carVelocityY = 0;
                carRotation = terrainAngle * (180f / (float)Math.PI);

                float impact = Math.Abs(carSpeed) * Math.Abs((float)Math.Sin(terrainAngle));
                if (impact > 2.5f)
                {
                    shakeAmount = impact * 2.5f;
                    score -= impact * 8;
                    neckOffset = impact * 3;
                }
                shakeAmount *= 0.85f;
                neckOffset *= 0.9f;
                carSpeed -= (float)Math.Sin(terrainAngle) * 0.4f;
            }
            else if (!hasCrashed)
            {
                if (keyRight) carRotation += 3;
                if (keyLeft) carRotation -= 3;
                carRotation = Math.Max(-85, Math.Min(85, carRotation));

                if (Math.Abs(carRotation) > 78)
                {
                    hasCrashed = true;
                    crashMessage = "CAR FLIPPED!";
                    shakeAmount = 20;
                    score -= 300;
                }
            }

            if (carSpeed < -8 && !hasCrashed && onGround)
            {
                hasCrashed = true;
                crashMessage = "BROKE HIS NECK!";
                timeSinceFlip = 0;
                shakeAmount = 15;
                score -= 500;
            }

            // CHECK FINISH LINE
            if (carX + carWidth / 2 >= finishX && !hasCrashed)
            {
                hasWon = true;
                score += 5000;
                winTimer = 0;
                gameTimer.Interval = 50; // ngadalëso tickun për animacion
            }

            // Coins
            for (int i = 0; i < coinsList.Length; i++)
            {
                if (!coinsCollected[i] &&
                    Math.Abs(carX + carWidth / 2 - coinsList[i].X) < 24 &&
                    Math.Abs(carY + carHeight / 2 - coinsList[i].Y) < 24)
                {
                    coinsCollected[i] = true;
                    coins++;
                    score += 50;
                }
            }

            // Fuel cans
            for (int i = 0; i < fuelCans.Length; i++)
            {
                if (!fuelCollected[i] &&
                    Math.Abs(carX + carWidth / 2 - fuelCans[i].X) < 28 &&
                    Math.Abs(carY + carHeight / 2 - fuelCans[i].Y) < 28)
                {
                    fuelCollected[i] = true;
                    fuel = Math.Min(100, fuel + 30);
                    score += 150;
                }
            }

            if (!hasCrashed)
            {
                distance += Math.Max(0, carSpeed) * 0.12f;
                score += Math.Max(0, carSpeed) * 0.25f;
            }
            score = Math.Max(0, score);

            if (hasCrashed)
            {
                timeSinceFlip += 0.05f;
                if (timeSinceFlip > 2f)
                {
                    isGameOver = true;
                    gameTimer.Stop();
                }
            }

            if (fuel <= 0 && !hasCrashed)
            {
                isGameOver = true;
                crashMessage = "OUT OF FUEL!";
                gameTimer.Stop();
            }

            if (carY > this.Height + 200 || carY < -200)
            {
                isGameOver = true;
                crashMessage = "FELL OFF!";
                gameTimer.Stop();
            }

            fuel = Math.Max(0, Math.Min(100, fuel));
            this.Invalidate();
        }

        // ── VIZATIMI ─────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float shakeX = shakeAmount > 0 ? rand.Next(-(int)shakeAmount, (int)shakeAmount) : 0;
            float shakeY = shakeAmount > 0 ? rand.Next(-(int)shakeAmount / 2, (int)shakeAmount / 2) : 0;

            // SKY - gradient bëhet më i errët me distancën
            float skyProgress = Math.Min(1f, cameraX / (worldWidth - this.Width));
            Color skyTop = InterpolateColor(
                Color.FromArgb(135, 206, 235),
                Color.FromArgb(20, 20, 80),
                skyProgress);
            using (var sky = new LinearGradientBrush(
                new Rectangle(0, 0, this.Width, this.Height),
                skyTop, Color.FromArgb(25, 25, 112), 90f))
                g.FillRectangle(sky, 0, 0, this.Width, this.Height);

            // Dielli lëviz pak me kamerën (parallax)
            float sunX = this.Width - 150 - cameraX * 0.02f;
            g.FillEllipse(Brushes.Yellow, sunX, 40, 80, 80);

            // Retë me parallax
            DrawCloud(g, 150 + cameraX * 0.1f % this.Width, 70, 90, 50);
            DrawCloud(g, 420 + cameraX * 0.08f % this.Width, 100, 70, 40);
            DrawCloud(g, 720 + cameraX * 0.12f % this.Width, 55, 85, 48);
            DrawCloud(g, 950 + cameraX * 0.09f % this.Width, 85, 75, 42);

            // Apliko kamerën + shake
            var saved = g.Save();
            g.TranslateTransform(-cameraX + shakeX, shakeY);

            // TERRAIN
            if (terrainPoints != null && terrainPoints.Length > 1)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddLines(terrainPoints);
                    path.AddLine(terrainPoints[terrainPoints.Length - 1].X, this.Height + 200,
                                 terrainPoints[0].X, this.Height + 200);

                    using (var groundBrush = new LinearGradientBrush(
                        new Rectangle(0, (int)groundHeight - 50, (int)worldWidth, 300),
                        Color.SaddleBrown, Color.Peru, 90f))
                        g.FillPath(groundBrush, path);

                    using (var pen = new Pen(Color.FromArgb(101, 67, 33), 3))
                        g.DrawLines(pen, terrainPoints);
                }
            }

            // FINISH LINE
            DrawFinishLine(g);

            // COINS
            for (int i = 0; i < coinsList.Length; i++)
                if (!coinsCollected[i])
                    DrawCoin(g, coinsList[i].X, coinsList[i].Y);

            // FUEL CANS
            for (int i = 0; i < fuelCans.Length; i++)
                if (!fuelCollected[i])
                    DrawFuelCan(g, fuelCans[i].X, fuelCans[i].Y);

            // PROGRESS MARKER - trekëndësh mbi makinë
            float prog = Math.Min(1f, (carX / finishX));
            DrawProgressMarker(g, prog);

            // MAKINA
            DrawCar(g, carX, carY, carRotation, wheelRotation, headAngle, neckOffset, hasCrashed);

            // EXHAUST
            if (keyRight && carSpeed > 2 && fuel > 0 && !hasCrashed)
            {
                int sz = rand.Next(5, 13);
                using (var smoke = new SolidBrush(Color.FromArgb(90, 128, 128, 128)))
                    g.FillEllipse(smoke, carX - 10, carY + carHeight, sz, sz);
            }

            g.Restore(saved);

            // PROGRESS BAR - lart qendër
            DrawProgressBar(g);

            // UI
            DrawUI(g);
        }

        private void DrawFinishLine(Graphics g)
        {
            float fh = GetTerrainHeight(finishX);

            // Shtylla e flamurit
            float poleX = finishX + 5;
            float poleY = fh - 120;
            using (var pole = new Pen(Color.White, 4))
                g.DrawLine(pole, poleX, poleY, poleX, fh);

            // Flamuri me animacion vale
            PointF[] flag = new PointF[]
            {
                new PointF(poleX,      poleY),
                new PointF(poleX + 30, poleY + 10 + flagWave),
                new PointF(poleX + 30, poleY + 30 + flagWave),
                new PointF(poleX,      poleY + 20),
            };
            // Karocat e flamurit (checkered)
            using (var flagBrush = new SolidBrush(Color.Black))
                g.FillPolygon(flagBrush, flag);

            // Karocat e bardha
            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 2; col++)
                    if ((row + col) % 2 == 0)
                    {
                        float cx = poleX + col * 15;
                        float cy = poleY + row * 10 + (col == 1 ? flagWave / 2 : 0);
                        PointF[] cell = {
                            new PointF(cx,      cy),
                            new PointF(cx + 15, cy + (col==1 ? flagWave/2 : 0)),
                            new PointF(cx + 15, cy + 10 + (col==1 ? flagWave/2 : 0)),
                            new PointF(cx,      cy + 10),
                        };
                        g.FillPolygon(Brushes.White, cell);
                    }

            // Vija e finishit (checkered pattern)
            int stripeW = 20;
            int stripeH = 10;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if ((i + j) % 2 == 0)
                        g.FillRectangle(Brushes.White,
                            finishX - 80 + i * stripeW,
                            fh - stripeH + j * stripeH,
                            stripeW, stripeH);
                    else
                        g.FillRectangle(Brushes.Black,
                            finishX - 80 + i * stripeW,
                            fh - stripeH + j * stripeH,
                            stripeW, stripeH);
                }
            }

            // Tekst "FINISH"
            using (var finFont = new Font("Arial", 14, FontStyle.Bold))
            {
                SizeF fs = g.MeasureString("FINISH", finFont);
                // Shadow
                g.DrawString("FINISH", finFont, Brushes.Black,
                    finishX - fs.Width / 2 + 2, fh - 145);
                g.DrawString("FINISH", finFont, Brushes.Yellow,
                    finishX - fs.Width / 2, fh - 147);
            }
        }

        private void DrawProgressMarker(Graphics g, float progress)
        {
            // Trekëndësh i vogël mbi makinë - tregon pozicionin
            float mx = carX + carWidth / 2;
            float my = carY - 20;
            using (var pen = new Pen(Color.Yellow, 2))
            {
                PointF[] arrow = {
                    new PointF(mx,      my),
                    new PointF(mx - 6,  my - 12),
                    new PointF(mx + 6,  my - 12),
                };
                g.FillPolygon(Brushes.Yellow, arrow);
            }
        }

        private void DrawProgressBar(Graphics g)
        {
            int barW = this.Width - 320;
            int barH = 14;
            int barX = 300;
            int barY = 8;

            float prog = Math.Min(1f, carX / finishX);
            int fill = (int)(barW * prog);

            // Background
            using (var bg = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                g.FillRectangle(bg, barX, barY, barW, barH);

            // Fill - jeshile → portokalli → kuqe
            Color c;
            if (prog < 0.5f)
                c = Color.FromArgb(50, (int)(255 * prog * 2), 50);
            else
                c = Color.FromArgb((int)(255 * (prog - 0.5f) * 2), 200, 50);

            if (fill > 0)
                using (var fb = new SolidBrush(c))
                    g.FillRectangle(fb, barX, barY, fill, barH);

            g.DrawRectangle(Pens.White, barX, barY, barW, barH);

            // Flamuri i finishit
            using (var ff = new Font("Arial", 9))
                g.DrawString("🏁", ff, Brushes.White, barX + barW + 3, barY - 2);

            // Makina e vogël mbi bar
            using (var ff = new Font("Arial", 9))
                g.DrawString("🚗", ff, Brushes.White, barX + fill - 10, barY - 14);

            // Distanca / total
            using (var ff = new Font("Arial", 9, FontStyle.Bold))
            {
                string txt = $"{(int)(prog * 100)}%";
                SizeF ts = g.MeasureString(txt, ff);
                g.DrawString(txt, ff, Brushes.Black, barX + (barW - ts.Width) / 2 + 1, barY + 1);
                g.DrawString(txt, ff, Brushes.White, barX + (barW - ts.Width) / 2, barY);
            }
        }

        // ── MAKINA ───────────────────────────────────────────────
        private void DrawCar(Graphics g, float x, float y, float rotation,
                             float wheelRot, float headAng, float neck, bool crashed)
        {
            float cx = x + carWidth / 2;
            float cy = y + carHeight / 2;

            var saved = g.Save();
            g.TranslateTransform(cx, cy);
            g.RotateTransform(rotation);

            float bx = -carWidth / 2;
            float by = -carHeight / 2;

            // Trupi
            using (var carBrush = new LinearGradientBrush(
                new RectangleF(bx, by, carWidth, carHeight),
                Color.FromArgb(220, 20, 60), Color.FromArgb(139, 0, 0), 45f))
                g.FillRectangle(carBrush, bx, by, carWidth, carHeight);
            g.DrawRectangle(new Pen(Color.DarkRed, 2), bx, by, carWidth, carHeight);

            // Kabina
            float cabX = bx + 10;
            float cabW = carWidth - 20;
            float cabH = 16;
            using (var cabBrush = new LinearGradientBrush(
                new RectangleF(cabX, by - cabH, cabW, cabH),
                Color.FromArgb(180, 20, 60), Color.FromArgb(120, 0, 0), 45f))
                g.FillRectangle(cabBrush, cabX, by - cabH, cabW, cabH);
            g.DrawRectangle(new Pen(Color.DarkRed, 1), cabX, by - cabH, cabW, cabH);

            // Dritaret
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 173, 216, 230)),
                cabX + 3, by - cabH + 3, (cabW / 2) - 5, cabH - 6);
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 173, 216, 230)),
                cabX + cabW / 2 + 2, by - cabH + 3, (cabW / 2) - 5, cabH - 6);

            // Rrotat
            DrawWheelLocal(g, -carWidth / 2 + 12, carHeight / 2, wheelRot);
            DrawWheelLocal(g, carWidth / 2 - 12, carHeight / 2, wheelRot);

            // Personazhi
            float px = -carWidth / 2 + 12;
            float py = -carHeight / 2 - 20;

            if (crashed)
            {
                g.FillEllipse(Brushes.LightYellow, px, py + 8, 15, 15);
                g.DrawLine(new Pen(Color.Black, 2), px + 3, py + 8, px + 10, py + 26);
                g.DrawLine(new Pen(Color.Black, 2), px + 12, py + 8, px + 5, py + 26);
                using (var f = new Font("Arial", 7))
                {
                    g.DrawString("X", f, Brushes.Red, px + 3, py + 11);
                    g.DrawString("X", f, Brushes.Red, px + 9, py + 11);
                }
            }
            else
            {
                g.DrawLine(new Pen(Color.Black, 2), px + 7, py + 10, px + 7 + neck * 0.3f, py + 22);
                g.FillEllipse(Brushes.LightYellow, px + headAng * 0.3f, py, 14, 14);
                g.FillEllipse(Brushes.Black, px + 3 + headAng * 0.2f, py + 4, 3, 3);
                g.FillEllipse(Brushes.Black, px + 9 + headAng * 0.2f, py + 4, 3, 3);
                if (carSpeed > 5)
                    g.DrawArc(new Pen(Color.Black, 2), px + 4 + headAng * 0.2f, py + 8, 8, 5, 0, 180);
                else if (carSpeed < -2)
                    g.DrawArc(new Pen(Color.Black, 2), px + 4 + headAng * 0.2f, py + 9, 8, 5, 180, 180);
                else
                    g.DrawLine(new Pen(Color.Black, 2), px + 4 + headAng * 0.2f, py + 10, px + 12 + headAng * 0.2f, py + 10);
                g.FillRectangle(Brushes.DarkBlue, px + headAng * 0.2f, py - 5, 14, 6);
            }

            g.Restore(saved);
        }

        private void DrawWheelLocal(Graphics g, float relX, float relY, float rotation)
        {
            var saved = g.Save();
            g.TranslateTransform(relX, relY);
            g.RotateTransform(rotation);

            g.FillEllipse(Brushes.Black, -wheelRadius, -wheelRadius, wheelRadius * 2, wheelRadius * 2);
            g.DrawEllipse(new Pen(Color.FromArgb(60, 60, 60), 2), -wheelRadius, -wheelRadius, wheelRadius * 2, wheelRadius * 2);

            float rimR = wheelRadius * 0.6f;
            g.FillEllipse(new SolidBrush(Color.FromArgb(180, 180, 180)), -rimR, -rimR, rimR * 2, rimR * 2);
            g.DrawEllipse(new Pen(Color.Gray, 1), -rimR, -rimR, rimR * 2, rimR * 2);

            using (var pen = new Pen(Color.DimGray, 1.5f))
                for (int i = 0; i < 5; i++)
                {
                    double angle = i * (2 * Math.PI / 5);
                    g.DrawLine(pen, 0, 0,
                        (float)Math.Cos(angle) * rimR * 0.8f,
                        (float)Math.Sin(angle) * rimR * 0.8f);
                }

            g.FillEllipse(Brushes.DarkGray, -3, -3, 6, 6);
            g.Restore(saved);
        }

        private void DrawCoin(Graphics g, float x, float y)
        {
            using (var goldBrush = new LinearGradientBrush(
                new RectangleF(x - 8, y - 8, 16, 16),
                Color.Gold, Color.DarkGoldenrod, 45f))
                g.FillEllipse(goldBrush, x - 8, y - 8, 16, 16);
            g.DrawEllipse(new Pen(Color.Orange, 1), x - 8, y - 8, 16, 16);
            using (var f = new Font("Arial", 8, FontStyle.Bold))
                g.DrawString("$", f, Brushes.White, x - 5, y - 7);
        }

        private void DrawFuelCan(Graphics g, float x, float y)
        {
            g.FillRectangle(Brushes.DarkGreen, x - 8, y, 16, 22);
            g.FillRectangle(Brushes.Green, x - 3, y - 6, 6, 6);
            g.DrawRectangle(new Pen(Color.DarkOliveGreen, 1), x - 8, y, 16, 22);
            using (var f = new Font("Arial", 8, FontStyle.Bold))
                g.DrawString("F", f, Brushes.White, x - 4, y + 4);
        }

        private void DrawCloud(Graphics g, float x, float y, float w, float h)
        {
            // Mbaj retë brenda ekranit
            x = x % (this.Width + 200) - 100;
            using (var b = new SolidBrush(Color.FromArgb(210, 255, 255, 255)))
            {
                g.FillEllipse(b, x, y, w, h);
                g.FillEllipse(b, x + w * 0.45f, y - h * 0.3f, w * 0.65f, h * 0.75f);
                g.FillEllipse(b, x - w * 0.15f, y - h * 0.2f, w * 0.55f, h * 0.65f);
            }
        }

        private Color InterpolateColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private void DrawUI(Graphics g)
        {
            // Panel i majtë
            using (var dark = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(dark, 0, 0, 290, 155);
                g.FillRectangle(dark, this.Width - 220, 0, 220, 90);
            }

            g.DrawString($"SCORE:    {(int)score}", fontUI, Brushes.Gold, 15, 12);
            g.DrawString($"COINS:    {(int)coins}", fontUI, Brushes.Yellow, 15, 42);
            g.DrawString($"DIST:      {(int)distance} m", fontUI, Brushes.White, 15, 72);
            g.DrawString($"SPEED:  {Math.Abs(carSpeed * 12):F0} km/h", fontUI, Brushes.Cyan, 15, 102);
            g.DrawString($"FUEL:     {fuel:F0}%",
                fontUI,
                fuel > 40 ? Brushes.LimeGreen : fuel > 20 ? Brushes.Orange : Brushes.OrangeRed,
                15, 128);

            // Fuel bar
            g.DrawString("FUEL", fontFuel, Brushes.White, this.Width - 200, 12);
            g.FillRectangle(Brushes.DimGray, this.Width - 200, 38, 170, 22);
            int fw = (int)(170 * fuel / 100);
            using (var fb = new SolidBrush(fuel > 40 ? Color.LimeGreen :
                                           fuel > 20 ? Color.Orange : Color.OrangeRed))
                g.FillRectangle(fb, this.Width - 200, 38, fw, 22);
            g.DrawRectangle(Pens.White, this.Width - 200, 38, 170, 22);
            g.DrawString($"{fuel:F0}%", fontFuel, Brushes.White, this.Width - 112, 40);

            // Kontrollet
            g.DrawString("← →  Drive   ↑  Jump   R  Restart   P  Pause",
                fontSmall, Brushes.LightGray, 12, this.Height - 28);

            // PAUSED
            if (isPaused)
            {
                using (var bg = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                    g.FillRectangle(bg, 0, 0, this.Width, this.Height);
                SizeF ps = g.MeasureString("PAUSED", fontBig);
                g.DrawString("PAUSED", fontBig, Brushes.Orange,
                    (this.Width - ps.Width) / 2, this.Height / 2 - 30);
                SizeF ps2 = g.MeasureString("Press P to resume", fontMed);
                g.DrawString("Press P to resume", fontMed, Brushes.White,
                    (this.Width - ps2.Width) / 2, this.Height / 2 + 20);
            }

            // CRASH
            if (hasCrashed && !isGameOver)
            {
                SizeF ts = g.MeasureString(crashMessage, fontCrash);
                using (var bg = new SolidBrush(Color.FromArgb(200, 0, 0, 0)))
                    g.FillRectangle(bg,
                        (this.Width - ts.Width) / 2 - 20, this.Height / 2 - 55,
                        ts.Width + 40, 65);
                g.DrawString(crashMessage, fontCrash, Brushes.Red,
                    (this.Width - ts.Width) / 2, this.Height / 2 - 40);
            }

            // GAME OVER
            if (isGameOver)
            {
                using (var bg = new SolidBrush(Color.FromArgb(210, 0, 0, 0)))
                    g.FillRectangle(bg, this.Width / 2 - 230, this.Height / 2 - 115, 460, 220);
                using (var border = new Pen(Color.Red, 2))
                    g.DrawRectangle(border, this.Width / 2 - 230, this.Height / 2 - 115, 460, 220);

                SizeF gs = g.MeasureString("GAME OVER", fontBig);
                g.DrawString("GAME OVER", fontBig, Brushes.Red,
                    (this.Width - gs.Width) / 2, this.Height / 2 - 100);

                SizeF cs = g.MeasureString(crashMessage, fontMed);
                g.DrawString(crashMessage, fontMed, Brushes.Yellow,
                    (this.Width - cs.Width) / 2, this.Height / 2 - 45);

                string stats = $"Score: {(int)score}   Coins: {(int)coins}   Dist: {(int)distance}m";
                SizeF ss = g.MeasureString(stats, fontMed);
                g.DrawString(stats, fontMed, Brushes.White,
                    (this.Width - ss.Width) / 2, this.Height / 2 + 5);

                SizeF rs = g.MeasureString("Press R to Restart", fontMed);
                g.DrawString("Press R to Restart", fontMed, Brushes.LightGray,
                    (this.Width - rs.Width) / 2, this.Height / 2 + 55);
            }

            // YOU WIN!
            if (hasWon)
            {
                using (var bg = new SolidBrush(Color.FromArgb(210, 0, 60, 0)))
                    g.FillRectangle(bg, this.Width / 2 - 260, this.Height / 2 - 130, 520, 250);
                using (var border = new Pen(Color.Gold, 3))
                    g.DrawRectangle(border, this.Width / 2 - 260, this.Height / 2 - 130, 520, 250);

                string win = "🏆 YOU WIN! 🏆";
                SizeF ws = g.MeasureString(win, fontWin);
                // Shadow
                g.DrawString(win, fontWin, Brushes.Black,
                    (this.Width - ws.Width) / 2 + 2, this.Height / 2 - 115);
                g.DrawString(win, fontWin, Brushes.Gold,
                    (this.Width - ws.Width) / 2, this.Height / 2 - 117);

                string sub1 = "You reached the FINISH LINE!";
                SizeF s1 = g.MeasureString(sub1, fontMed);
                g.DrawString(sub1, fontMed, Brushes.White,
                    (this.Width - s1.Width) / 2, this.Height / 2 - 55);

                string stats = $"Score: {(int)score}   Coins: {(int)coins}   Dist: {(int)distance}m";
                SizeF ss = g.MeasureString(stats, fontMed);
                g.DrawString(stats, fontMed, Brushes.Yellow,
                    (this.Width - ss.Width) / 2, this.Height / 2 - 10);

                string rr = "Press R to Play Again";
                SizeF rs = g.MeasureString(rr, fontMed);
                g.DrawString(rr, fontMed, Brushes.Cyan,
                    (this.Width - rs.Width) / 2, this.Height / 2 + 55);
            }
        }

        private void HillClimbRacing_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: keyRight = true; break;
                case Keys.Left: keyLeft = true; break;
                case Keys.Up:
                    if (onGround && !hasCrashed && fuel > 0)
                    {
                        carVelocityY = -10;
                        fuel -= 1.5f;
                    }
                    break;
                case Keys.R: RestartGame(); break;
                case Keys.P:
                    isPaused = !isPaused;
                    this.Invalidate();
                    break;
            }
        }

        private void HillClimbRacing_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: keyRight = false; break;
                case Keys.Left: keyLeft = false; break;
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
            isPaused = false;
            hasWon = false;
            cameraX = 0;
            shakeAmount = 0;
            wheelRotation = 0;
            headAngle = 0;
            neckOffset = 0;
            timeSinceFlip = 0;
            winTimer = 0;
            flagWave = 0;

            gameTimer.Interval = 16;

            GenerateTerrain();
            GenerateCollectibles();

            if (!gameTimer.Enabled) gameTimer.Start();
        }

        private void HillClimbRacing_FormClosing(object sender, FormClosingEventArgs e)
        {
            gameTimer?.Stop();

            fontUI?.Dispose(); fontFuel?.Dispose();
            fontSmall?.Dispose(); fontCrash?.Dispose();
            fontBig?.Dispose(); fontMed?.Dispose();
            fontWin?.Dispose();

            mainForm?.Show();
        }

        private void HillClimbRacing_Load(object sender, EventArgs e) { }
    }
}