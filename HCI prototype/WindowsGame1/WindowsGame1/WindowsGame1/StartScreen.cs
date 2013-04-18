using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class StartScreen
    {
        int gameHeight = 0;
        int gameWidth = 0;
        Rectangle startRect, tutRect;
        Texture2D startTex, tutTex;
        public StartScreen(int GameHeight, int GameWidth, ContentManager content)
        {
            gameHeight = GameHeight;
            gameWidth = GameWidth;

            int top = (gameHeight / 3);
            int left = (gameWidth / 3);

            startRect = new Rectangle(left - 100, top, 300, 150);
            tutRect = new Rectangle(left + 250, top, 300, 150);
            startTex = content.Load<Texture2D>("startButton");
            tutTex = content.Load<Texture2D>("tutorialButton");
        }

        KeyboardState oldKbState;
        KeyboardState newKbState;
        public bool Update(ref GameState gameState)
        {
            newKbState = Keyboard.GetState();
            if (oldKbState != newKbState)
            {
                if (newKbState.IsKeyDown(Keys.T))
                {
                    gameState = GameState.Tutorial;
                }
                else if (newKbState.IsKeyDown(Keys.S))
                {
                    gameState = GameState.Running;
                }
            }
            oldKbState = newKbState;
            return false;
        }

        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(startTex, startRect, Color.White);
            spriteBatch.Draw(tutTex, tutRect, Color.White);
        }
    }
}
