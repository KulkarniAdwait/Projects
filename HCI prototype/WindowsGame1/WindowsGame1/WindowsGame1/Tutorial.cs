using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class Tutorial
    {
        private List<Punch> tutorial;
        private int currentPunch = 0;
        public Tutorial(ContentManager Content, int GameWidth, int GameHeight, float GuardY, float ScaleGuard, float ScalePunch)
        {
            //tutorial
            tutorial = new List<Punch>();
            tutorial.Add(new LeftPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new RightHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new LeftUppercut(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new RightUppercut(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            tutorial.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
        }

        KeyboardState oldKbState;
        KeyboardState newKbState;
        public bool Update(GameTime gameTime)
        {
            newKbState = Keyboard.GetState();
            if (oldKbState != newKbState)
            {
                if (newKbState.IsKeyDown(Keys.Enter))
                {
                    currentPunch++;
                    if (currentPunch >= tutorial.Count)
                    {
                        currentPunch = 0;
                        return true;
                    }
                }
            }
            oldKbState = newKbState;
            return false;
        }

        Vector2 tutTextPosition = new Vector2(500, 100);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            tutorial[currentPunch].isActive = true;
            tutorial[currentPunch].isTutorial = true;
            tutorial[currentPunch].Draw(gameTime, spriteBatch, 0.0f);
            spriteBatch.DrawString(font, tutorial[currentPunch].punchName, tutTextPosition, Color.Black);
        }
    }
}
