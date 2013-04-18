using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Block : Punch
    {
        public Block(ContentManager Content, float GameWidth, float GameHeight, float GuardY, float ScaleGuard)
        {
            rGuard = new Rectangle(0, 0, 450, 600);
            pvGuard = new Vector2(GameWidth / 4, GuardY);
            ovGuard = new Vector2(rGuard.Width / 2, 0);

            //cheating and internally using the punch as the right guard
            rPunch = new Rectangle(0, 0, 450, 600);
            pvPunch = new Vector2(3 * GameWidth / 4, GuardY);
            ovPunch = new Vector2(rGuard.Width / 2, 0);

            sGuard = ScaleGuard;
            sPunch = ScaleGuard;

            tGuard = Content.Load<Texture2D>("left_guard");
            tPunch = Content.Load<Texture2D>("right_guard");

            base.SetStartingPositions(pvGuard, pvPunch);
        }

        public override void Update(GameTime gameTime, float scrollSpeed)
        {
            if (isActive)
            {
                scrollVector.Y -= (scrollSpeed / 5 * gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        public override bool NearingEnd()
        {
            if (scrollVector.Y < 0.00002f)
                return true;
            return false;
        }

        public override bool AtEnd()
        {
            if (scrollVector.Y < 0.00001f)
                return true;
            return false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scrollSpeed)
        {
            if (isActive)
            {
                //Guard
                spriteBatch.Draw(tGuard, pvGuard, rGuard, Color.White, 0f, ovGuard, sGuard, SpriteEffects.None, 0);

                spriteBatch.Draw(tPunch, pvPunch, rPunch, Color.White, 0f, ovPunch, sPunch, SpriteEffects.None, 0);
            }
        }
    }
}
