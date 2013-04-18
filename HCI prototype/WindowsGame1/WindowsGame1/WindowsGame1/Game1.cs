using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GameState gameState = GameState.Menu;
        public const int GAME_WIDTH = 1280;
        public const int GAME_HEIGHT = 800;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Texture2D tPunchLine;
        Rectangle rPunchLine;
        const float sGuard = 0.3f;
        const float sPunch = 0.5f;

        public float ScrollSpeed = 0.05f;
        public const float GUARD_Y = GAME_HEIGHT / 4.0f;

        int mLeft = 0;
        int mRight = 0;

        Combination combination;
        Tutorial tutorial;
        StartScreen startScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            tutorial = new Tutorial(Content, GAME_WIDTH, GAME_HEIGHT, GUARD_Y, sGuard, sPunch);
            combination = new Combination(Content, GAME_WIDTH, GAME_HEIGHT, GUARD_Y, sGuard, sPunch);
            startScreen = new StartScreen(GAME_HEIGHT, GAME_WIDTH, Content);
            if (combination != null)
            {
                combination.getFirstPunch();
            }

            //rand = new Random(DateTime.Now.Millisecond);

            //currentPunch = getNextPunch();

            rPunchLine = new Rectangle(0, 450, 1280, 5);

            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            base.Initialize();
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tPunchLine = Content.Load<Texture2D>("punch_line");
            font = Content.Load<SpriteFont>("SpriteFont1");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        KeyboardState oldKbState;
        KeyboardState newKbState;
        float SCROLL_INCREMENT = 0.015f;
        protected override void Update(GameTime gameTime)
        {
            newKbState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (gameState == GameState.Menu)
            {
                startScreen.Update(ref gameState);
            }
            else if (gameState == GameState.Tutorial)
            {
                //tutorial done running
                if (tutorial.Update(gameTime) == true)
                {
                    gameState = GameState.Menu;
                }
            }
            else if (gameState == GameState.Running)
            {

                if (oldKbState != newKbState)
                {
                    //left miss
                    if (newKbState.IsKeyDown(Keys.A))
                    {
                        mLeft++;
                    }
                    else if (newKbState.IsKeyDown(Keys.K))
                    {
                        mRight++;
                    }
                    //speed
                    else if (newKbState.IsKeyDown(Keys.Up))
                    {
                        ScrollSpeed += SCROLL_INCREMENT;
                    }
                    else if (newKbState.IsKeyDown(Keys.Down))
                    {
                        ScrollSpeed -= SCROLL_INCREMENT;
                    }
                }

                combination.Update(gameTime, ScrollSpeed);
                oldKbState = newKbState;
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        Vector2 SpeedPosition = new Vector2(10, 5);
        Vector2 MissesPosition = new Vector2(200, 5);
        Vector2 LeftMissesPosition = new Vector2(300, 5);
        Vector2 RightMissesPosition = new Vector2(400, 5);
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            if (gameState == GameState.Menu)
            {
                startScreen.Draw(spriteBatch);
            }
            else if (gameState == GameState.Tutorial)
            {
                tutorial.Draw(gameTime, spriteBatch, font);
            }
            else
            {
                if (combination.currentPunch != null)
                {
                    combination.currentPunch.Draw(gameTime, spriteBatch, ScrollSpeed);
                }
                if (combination.queuedPunch != null)
                {
                    combination.queuedPunch.Draw(gameTime, spriteBatch, ScrollSpeed);
                }

                spriteBatch.Draw(tPunchLine, rPunchLine, Color.White);

                spriteBatch.DrawString(font, "Speed: " + ScrollSpeed.ToString(), SpeedPosition, Color.Black);
                spriteBatch.DrawString(font, "Misses: ", MissesPosition, Color.Black);
                spriteBatch.DrawString(font, "Left:" + mLeft.ToString(), LeftMissesPosition, Color.Red);
                spriteBatch.DrawString(font, "Right:" + mRight.ToString(), RightMissesPosition, Color.Red);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
