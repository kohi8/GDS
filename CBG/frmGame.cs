using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
    Game Rules

    Gameplay

    Game Goal 
 */
namespace CBG
{
    public partial class frmGame : Form
    {
        #region Helpers - Game Library
        /// <summary>
        /// Draws a ball of BallSize size in BallPosition, centered around the ball center
        /// </summary>
        /// <param name="g">The graphics to draw to</param>
        /// <param name="BallPosition">The Position</param>
        void DrawBall(Graphics g, Brush brush, PointF BallPosition)
        {
            g.FillEllipse(brush, new RectangleF(BallPosition - BallCenter, BallSize));
        }

        /// <summary>
        /// Restricts a value between min and max
        /// </summary>
        /// <param name="NextValue">The New Value that might be out of bounds</param>
        /// <param name="Min">The minimum allowed value</param>
        /// <param name="Max">The maximum allowed value</param>
        /// <returns></returns>
        float ConstrainMovement(float NextValue, float Min, float Max)
        {
            // playerX = Math.Max(0, Math.Min(playerX, pbGameScreen.Size.Width));
            // playerX = playerX < 0 ? 0 : (playerX > pbGameScreen.Size.Width : pbGameScreen.Size.Width : playerX);

            if (NextValue <= Min)
            {
                return Min;
            }
            else if (NextValue >= Max)
            {
                return Max;
            }
            else
            {
                return NextValue;
            }
        }

        PointF ConstrainMovement(PointF NextValue, float MinX, float MaxX, float MinY, float MaxY)
        {
            return new PointF(ConstrainMovement(NextValue.X, MinX, MaxX), ConstrainMovement(NextValue.Y, MinY, MaxY));
        }

        PointF ConstrainMovement(PointF NextValue, RectangleF rect)
        {
            return ConstrainMovement(NextValue, rect.X, rect.X + rect.Width, rect.Y, rect.Y + rect.Width);
        }

        PointF ConstrainMovement(PointF NextValue, Size size)
        {
            return ConstrainMovement(NextValue, 0, size.Width, 0, size.Height);
        }

        #endregion

        #region UI & Paint / update driver
        Form MainMenu;
        public frmGame(Form MainMenu)
        {
            this.MainMenu = MainMenu;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitGame();
        }

        private void tmrGameTick_Tick(object sender, EventArgs e)
        {
            pbGameScreen.Refresh();
        }

        private void pbGameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Calculate game time since begginging, and since last calculation of time
            TimeSpan TotalTimeSpan = DateTime.Now - StartTime;
            TimeSpan DeltaTimeSpan = DateTime.Now - LastTime;

            // Update time reference for last calculation
            LastTime = DateTime.Now;

            UpdateGame((float)TotalTimeSpan.TotalSeconds, (float)DeltaTimeSpan.TotalSeconds);

            Graphics g = e.Graphics;
            DrawGame(g);
        }
        #endregion

        #region Game State
        /// <summary>
        ///  When the game started
        /// </summary>
        DateTime StartTime;
        DateTime LastTime;

        float PlayerSpeedLimit;
        float EnemySpeedLimit;

        // Current Player Position
        PointF PlayerPosition;

        // How much the player moves each second
        SizeF PlayerSpeed;

        PointF EnemyPosition;
        SizeF EnemySpeed;

        float WinningDistance;
        SizeF BallSize;

        SizeF BallCenter;

        float HighScore = 100;

        
        #endregion

        #region Game Init & Update Loop

        void InitGame()
        {
            // Game Parameters
            PlayerSpeedLimit = 500;
            EnemySpeedLimit = 500;
            WinningDistance = 20;

            EnemyPosition = new PointF(400, 0);
            PlayerPosition = new PointF(400, 225);

            
            // Reset game state
            PlayerSpeed = new SizeF(0, 0);
            EnemySpeed = new SizeF(EnemySpeedLimit, EnemySpeedLimit);
            BallSize = new SizeF(WinningDistance, WinningDistance);
            BallCenter = new SizeF(WinningDistance/2, WinningDistance/2);

            // Initialize Time
            StartTime = DateTime.Now;
            LastTime = DateTime.Now;
        }

        /// <summary>
        /// Updates the game logic
        /// </summary>
        /// <param name="totalTime">Total time the game is running up to now</param>
        /// <param name="deltaTime">Time since the last update</param>
        void UpdateGame(float totalTime, float deltaTime)
        {
            // Update player position based on speed
            SizeF PlayerStep = new SizeF(PlayerSpeed.Width * deltaTime, PlayerSpeed.Height * deltaTime);
            PlayerPosition += PlayerStep;

            // Player Collision Check
            PlayerPosition = ConstrainMovement(PlayerPosition, pbGameScreen.Size);

            // Update enemy positon based on speed
            SizeF EnemyStep = new SizeF(EnemySpeed.Width * deltaTime, EnemySpeed.Height * deltaTime);
            EnemyPosition += EnemyStep;

            // Bounce

            if (EnemyPosition.X < 0 || EnemyPosition.X > pbGameScreen.Size.Width)
            {
                EnemySpeed.Width = -EnemySpeed.Width;
            }

            if (EnemyPosition.Y < 0 || EnemyPosition.Y > pbGameScreen.Size.Height)
            {
                EnemySpeed.Height= -EnemySpeed.Height;
            }

            // Make sure it stays inside the screen limits
            EnemyPosition = ConstrainMovement(EnemyPosition, pbGameScreen.Size);

            // Did the player manage to catch the enemy
            if (Math.Pow(EnemyPosition.X - PlayerPosition.X, 2) + Math.Pow(EnemyPosition.Y - PlayerPosition.Y, 2) < (WinningDistance * WinningDistance)) {
                tmrGameTick.Enabled = false;

                totalTime = Math.Min(totalTime, HighScore);

                if (MessageBox.Show("YOU WON in " + totalTime + "\nHigh Score is: " + totalTime + "\nDo you want to play AGAIN?!", "Was it that hard?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    InitGame();
                    tmrGameTick.Enabled = true;
                }
                else
                {
                    Hide();
                    MainMenu.Show();
                }
            }

            // Do something else here

            /*
            // eg, a new level idea
            BallSize.Height = 20 + 10 * (float)Math.Sin(totalTime * 12);
            BallSize.Width= 20 + 10 * (float)Math.Sin(totalTime * 12);
            WinningDistance = 20 + 10 * (float)Math.Sin(totalTime * 12);

            BallCenter.Height = BallSize.Height / 2;
            BallCenter.Width = BallSize.Width / 2;

            pbGameScreen.BackColor = Color.FromArgb((int)(100 + 50 * Math.Sin(totalTime * 12)), (int)(100 + 50 * Math.Sin(totalTime * 92)), (int)(100 + 50 * Math.Sin(totalTime * 2)));
            */
             
        }
        #endregion

        #region Draw Game Screen
        void DrawGame(Graphics g)
        {
            DrawBall(g, Brushes.Red, PlayerPosition);

            DrawBall(g, Brushes.Blue, EnemyPosition);
        }


        #endregion

        #region Player Input
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Up:   PlayerSpeed.Height = -PlayerSpeedLimit; break;
                case Keys.Down: PlayerSpeed.Height = PlayerSpeedLimit; break;

                case Keys.Left:  PlayerSpeed.Width = -PlayerSpeedLimit; break;
                case Keys.Right: PlayerSpeed.Width = PlayerSpeedLimit; break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up: 
                    PlayerSpeed.Height = 0; 
                    break;

                case Keys.Right:
                case Keys.Left: 
                    PlayerSpeed.Width = 0;
                    break;
            }
        }
        #endregion
    }
}
