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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RightPunch : Punch
    {
        public RightPunch(ContentManager Content, float GameWidth, float GameHeight, float GuardY, float ScaleGuard, float ScalePunch)
        {
            rGuard = new Rectangle(0, 0, 450, 600);
            pvGuard = new Vector2(GameWidth / 4, GuardY);
            ovGuard = new Vector2(rGuard.Width / 2, 0);

            rPunch = new Rectangle(0, 0, 450, 450);
            pvPunch = new Vector2(3 * GameWidth / 4, GameHeight);
            ovPunch = new Vector2(rPunch.Width / 2, 0);

            sGuard = ScaleGuard;
            sPunch = ScalePunch;

            tGuard = Content.Load<Texture2D>("left_guard");
            tPunch = Content.Load<Texture2D>("right_punch");

            base.SetStartingPositions(pvGuard, pvPunch);

            punchName = "Right Cross";

        }
    }
}
