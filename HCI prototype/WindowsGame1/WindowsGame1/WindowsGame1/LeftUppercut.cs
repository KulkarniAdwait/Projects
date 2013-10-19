using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class LeftUppercut : Punch
    {
        public LeftUppercut(ContentManager Content, float GameWidth, float GameHeight, float GuardY, float ScaleGuard, float ScalePunch)
        {
            rGuard = new Rectangle(0, 0, 450, 600);
            pvGuard = new Vector2(3 * GameWidth / 4, GuardY);
            ovGuard = new Vector2(rGuard.Width / 2, 0);

            rPunch = new Rectangle(0, 0, 300, 400);
            pvPunch = new Vector2(GameWidth / 4, GameHeight);
            ovPunch = new Vector2(rPunch.Width / 2, 0);

            sGuard = ScaleGuard;
            sPunch = ScalePunch;

            tGuard = Content.Load<Texture2D>("right_guard");
            tPunch = Content.Load<Texture2D>("left_uppercut");

            base.SetStartingPositions(pvGuard, pvPunch);

            punchName = "Throw a Left Uppercut";
        }
    }
}