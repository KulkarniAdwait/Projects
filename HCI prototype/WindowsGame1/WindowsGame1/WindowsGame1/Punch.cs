using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{
    public class Punch
    {
        public bool isActive;
        protected Texture2D tGuard;
        protected Texture2D tPunch;
        protected Rectangle rGuard;
        protected Rectangle rPunch;
        protected Vector2 pvGuard;
        protected Vector2 ovGuard;
        protected Vector2 pvPunch;
        protected Vector2 ovPunch;
        protected Vector2 scrollVector;
        protected float sGuard;
        protected float sPunch;

        protected Vector2 _pvGuard;
        protected Vector2 _pvPunch;

        public bool isTutorial = false;
        public String punchName;


        private bool drawGuard;
        private readonly float guardDelay = 20.0f;
        private float guardTimer;

        public void Start()
        {
            this.isActive = true;
            pvGuard = _pvGuard;
            pvPunch = _pvPunch;
            scrollVector = new Vector2(1, 1);
            guardTimer = 0.0f;
            drawGuard = false;
        }

        protected void SetStartingPositions(Vector2 pvGuard, Vector2 pvPunch)
        {
            _pvGuard = pvGuard;
            _pvPunch = pvPunch;
            scrollVector = new Vector2(1, 1);
        }

        public virtual void Update(GameTime gameTime, float scrollSpeed)
        {
            if (isActive)
            {

                scrollVector.Y -= (scrollSpeed * gameTime.ElapsedGameTime.Milliseconds);
                pvPunch.Y -= (1 - scrollVector.Y);

                guardTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (guardTimer >= (guardDelay / scrollSpeed))
                {
                    drawGuard = true;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scrollSpeed)
        {
            if (isActive)
            {
                if (isTutorial)
                {
                    pvPunch.Y = pvGuard.Y;
                }
                //Guard
                if (drawGuard == true)
                {
                    spriteBatch.Draw(tGuard, pvGuard, rGuard, Color.White, 0f, ovGuard, sGuard, SpriteEffects.None, 0);
                }
                //Punch
                spriteBatch.Draw(tPunch, pvPunch, rPunch, Color.White, 0f, ovPunch, sPunch, SpriteEffects.None, 0);
            }
        }

        public virtual bool NearingEnd()
        {
            if (this.pvPunch.Y < this.rPunch.Height / 2)
                return true;
            return false;
        }

        public virtual bool AtEnd()
        {
            if (this.pvPunch.Y + this.rPunch.Height < 5)
                return true;
            return false;
        }

    }
}
