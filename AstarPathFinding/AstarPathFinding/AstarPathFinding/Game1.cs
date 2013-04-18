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

namespace AstarPathFinding
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Vector2 position = Vector2.Zero;
        //setting time greater because we want a path right from the start
        double pathTimer = 6;
        double pathTimeout = 3;
        Vector2 direction = Vector2.Zero;
        Texture2D movingPart;
        Vector2 targetGridPosition;
        bool chaseActive = false;

        float speed = 100f;
        Vector2 start, end;
        int gridRows = 16;
        int gridColumns = 26;
        int GridSize = 50;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect basicEffect;
        VertexPositionColor[] rowVertices;
        VertexPositionColor[] columnVertices;
        PathFinder pathFinder;
        Texture2D pathPoint;
        bool[,] isWalkable;
        SpriteFont font;

        Vector2 newPosition;
        Vector2[] path;
        Vector2[] oldPath;
        int pathIndex = 0;
        int oldPathIndex = 0;
        private Vector2 textPosition;

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
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = gridColumns * GridSize;
            graphics.PreferredBackBufferHeight = gridRows * GridSize;
            graphics.ApplyChanges();
            // TODO: Add your initialization logic here
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.DiffuseColor = new Vector3(0, 0, 0.1f);

            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0
                , graphics.GraphicsDevice.Viewport.Width
                , graphics.GraphicsDevice.Viewport.Height, 0, 0, 1);

            rowVertices = new VertexPositionColor[2 * gridRows + 2];
            int j = 0;
            for (int i = 0; i <= gridRows; i++, j += 2)
            {
                rowVertices[j].Position = new Vector3(0, i * GridSize, 0);
                rowVertices[j].Color = Color.Blue;
                rowVertices[j + 1].Position = new Vector3(GridSize * gridColumns, i * GridSize, 0);
                rowVertices[j + 1].Color = Color.Blue;
            }

            columnVertices = new VertexPositionColor[2 * gridColumns + 2];
            j = 0;
            for (int i = 0; i <= gridColumns; i++, j += 2)
            {
                columnVertices[j].Position = new Vector3(i * GridSize, 0, 0);
                columnVertices[j].Color = Color.Blue;
                columnVertices[j + 1].Position = new Vector3(i * GridSize, GridSize * gridRows, 0);
                columnVertices[j + 1].Color = Color.Blue;
            }

            isWalkable = new bool[gridRows, gridColumns];
            for (int i = 0; i < gridRows; i++)
            {
                for (j = 0; j < gridColumns; j++)
                {
                    isWalkable[i, j] = true;
                }
            }

            //outer walls
            for (int i = 0; i < gridRows; i++)
            {
                isWalkable[i, 0] = false;
                isWalkable[i, gridColumns - 1] = false;
            }
            for (j = 0; j < gridColumns; j++)
            {
                isWalkable[0, j] = false;
                isWalkable[gridRows - 1, j] = false;
            }

            start = new Vector2(1, 1);
            end = new Vector2(gridColumns - 2, gridRows - 2);

            position = start * GridSize;
            targetGridPosition = end;

            //text position
            textPosition = new Vector2(gridColumns, (gridRows - 1) * GridSize);

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
            pathPoint = Content.Load<Texture2D>("pathPoint");
            movingPart = Content.Load<Texture2D>("movingPart");
            font = Content.Load<SpriteFont>("font");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        KeyboardState oldState = new KeyboardState();
        KeyboardState newState;
        MouseState oldMouse = new MouseState();
        MouseState newMouse;
        Vector2 newWall = Vector2.Zero;
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //mouse: add walls & start chase
            MouseHandler();

            //keyboard: move target
            MoveTarget();

            //Find path
            pathTimer += gameTime.ElapsedGameTime.TotalSeconds;
            //find new path after timeout
            if (pathTimer > pathTimeout)
            {
                //if chaser is already on a paht then 
                //find new path from current point in path
                if (path != null && pathIndex < path.Length)
                {
                    start = path[pathIndex];
                }
                else//find path form current position
                {
                    start.X = (float)Math.Round(position.X / GridSize);
                    start.Y = (float)Math.Round(position.Y / GridSize);
                }
                pathTimer = 0;
                //save old path in case new target position is unreachable
                if (path != null)
                {
                    oldPathIndex = pathIndex;
                    oldPath = path;
                }

                //only find new path if target hasnt been reached
                if (!start.Equals(targetGridPosition) && chaseActive)
                {
                    pathIndex = 0;
                    pathFinder = new PathFinder(gridRows, gridColumns, isWalkable);
                    path = pathFinder.findPath(start, targetGridPosition, isWalkable);

                    //if new path path cant be foudn then take old path
                    if (path == null)
                    {
                        path = oldPath;
                        pathIndex = oldPathIndex;
                    }
                }
            }
            if (chaseActive)
            {
                Move();
                Animate(gameTime);
            }
            base.Update(gameTime);
        }

        private void MoveTarget()
        {
            newState = Keyboard.GetState(PlayerIndex.One);
            if (oldState != newState)
            {
                if (newState.IsKeyDown(Keys.Down))
                {
                    if (targetGridPosition.Y < gridRows - 2)
                        targetGridPosition.Y += 1.0f;
                }
                if (newState.IsKeyDown(Keys.Up))
                {
                    if (targetGridPosition.Y > 1)
                        targetGridPosition.Y -= 1.0f;
                }
                if (newState.IsKeyDown(Keys.Left))
                {
                    if (targetGridPosition.X > 1)
                        targetGridPosition.X -= 1.0f;
                }
                if (newState.IsKeyDown(Keys.Right))
                {
                    if (targetGridPosition.X < gridColumns - 2)
                        targetGridPosition.X += 1.0f;
                }
            }
            oldState = newState;
        }

        private void MouseHandler()
        {
            newMouse = Mouse.GetState();
            if (oldMouse != newMouse)
            {
                if (newMouse.LeftButton == ButtonState.Pressed)
                {
                    newWall.X = (int)(newMouse.X / GridSize);
                    newWall.Y = (int)(newMouse.Y / GridSize);
                    //cannot add or remove bounding walls
                    if (newWall.X > 0 && newWall.X < gridColumns - 1)
                    {
                        if (newWall.Y > 0 && newWall.Y < gridRows - 1)
                        {
                            isWalkable[(int)newWall.Y, (int)newWall.X] = !isWalkable[(int)newWall.Y, (int)newWall.X];
                        }
                    }
                }
                else if (newMouse.RightButton == ButtonState.Pressed)
                {
                    chaseActive = !chaseActive;
                }
            }
            oldMouse = newMouse;
        }

        private void Move()
        {
            if (path != null)
            {
                if (pathIndex < path.Length)
                {
                    direction = path[pathIndex] * GridSize - position;
                    if (Math.Abs(direction.X) < 0.01f && Math.Abs(direction.Y) < 0.01f)
                    {
                        pathIndex++;
                    }
                    else
                    {
                        direction.Normalize();
                    }
                }
                else
                {
                    direction = Vector2.Zero;
                }
            }
        }

        private void Animate(GameTime gameTime)
        {
            newPosition = position + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isWalkable[(int)Math.Round(newPosition.Y / GridSize), (int)Math.Round(newPosition.X / GridSize)])
            {
                //position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position = newPosition;
            }
        }

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color color = Color.Blue;
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, rowVertices, 0, gridRows + 1);
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, columnVertices, 0, gridColumns + 1);

            spriteBatch.Begin();

            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    if (isWalkable[i, j] == false)
                    {
                        spriteBatch.Draw(pathPoint, new Rectangle(j * (GridSize), i * (GridSize), GridSize, GridSize), Color.Blue);
                    }
                }

            }

            spriteBatch.Draw(movingPart, position, null, Color.White);
            spriteBatch.Draw(movingPart, targetGridPosition * GridSize, null, Color.Purple);
            spriteBatch.End();


            spriteBatch.Begin();

            if (path != null)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (i == 0)
                        color = Color.Red;
                    else if (i == path.Length - 1)
                        color = Color.Green;
                    else
                        color = Color.Yellow;

                    spriteBatch.Draw(pathPoint, new Rectangle((int)path[i].X * (GridSize), (int)path[i].Y * (GridSize), 10, 10), color);
                }
            }
            spriteBatch.DrawString(font, "Left click: place/remove barrier; Right click: begin chase; Arrows: move target", textPosition, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}
