using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Combination
    {
        Random rand;
        List<List<Punch>> combinations;
        int comboIndex, punchIndex;
        public Punch currentPunch, queuedPunch;
        private ContentManager Content;
        private int GameWidth;
        private int GameHeight;
        private float GuardY;
        private float ScaleGuard;
        private float ScalePunch;

        public Combination(ContentManager Content, int GameWidth, int GameHeight, float GuardY, float ScaleGuard, float ScalePunch)
        {
            combinations = new List<List<Punch>>();
            rand = new Random(System.DateTime.Now.Millisecond);
            this.Content = Content;
            this.GameWidth = GameWidth;
            this.GameHeight = GameHeight;
            this.GuardY = GuardY;
            this.ScaleGuard = ScaleGuard;
            this.ScalePunch = ScalePunch;
            PopulateCombinations();
            punchIndex = 0;
            comboIndex = rand.Next(0, combinations.Count);

        }

        private void PopulateCombinations()
        {
            //For a right handed fighter:
            //1 is left jab to the face, often repeated 0 to 2 times (1 to 3 jabs total) with or without a blank space in between each (random number of jabs, random blank-space/no-blank-space after each)
            //2 is right cross (horizontal fist and arm) or uppercut (vertical fist and arm) to body or face
            //(random type cross/uppercut and random target body/face)
            //3 is a left hook to body or face
            //(random target body/face)

            //1 2
            List<Punch> combo1 = new List<Punch>();
            combo1.Add(new LeftPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;

            //2 3
            combo1 = new List<Punch>();
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;

            //3 2
            combo1 = new List<Punch>();
            combo1.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;

            //2 3 2
            combo1 = new List<Punch>();
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;

            //1 2 3
            combo1 = new List<Punch>();
            combo1.Add(new LeftPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;

            //1 2 3 2
            combo1 = new List<Punch>();
            combo1.Add(new LeftPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new LeftHook(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new RightPunch(Content, GameWidth, GameHeight, GuardY, ScaleGuard, ScalePunch));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combo1.Add(new Block(Content, GameWidth, GameHeight, GuardY, ScaleGuard));
            combinations.Add(combo1);
            combo1 = null;
        }

        public void getFirstPunch()
        {
            currentPunch = combinations[comboIndex][punchIndex];
            currentPunch.Start();
        }

        public void Update(GameTime gameTime, float scrollSpeed)
        {
            currentPunch.Update(gameTime, scrollSpeed);
            if (queuedPunch != null)
                queuedPunch.Update(gameTime, scrollSpeed);

            //if only half the current punch is visible
            //queue next punch
            if (currentPunch.NearingEnd())
            {
                //queued punch hasnt ben activated yet
                if (queuedPunch == null)
                {
                    //punchIndex = (punchIndex + 1) % combinations[comboIndex].Count;
                    if (++punchIndex == combinations[comboIndex].Count)
                    {
                        punchIndex = 0;
                        comboIndex = rand.Next(0, combinations.Count);
                    }
                    queuedPunch = combinations[comboIndex][punchIndex];
                    //queuedPunch = getNextPunch();
                    queuedPunch.Start();
                }
            }
            //de activate current punch if it has reached end of screen
            if (currentPunch.AtEnd())
            {
                if (currentPunch.isActive)
                {
                    currentPunch.isActive = false;
                    currentPunch = queuedPunch;
                    queuedPunch = null;
                }
            }
        }
        
        //use later
        public Punch getNextRandomPunch()
        {
            Punch nextPunch = null;
            if (rand.Next(0, 2) == 0)
            {
                //nextPunch = new LeftPunch(Content, GAME_WIDTH, GAME_HEIGHT, GUARD_Y, sGuard, sPunch);
            }
            else
            {
                //nextPunch = new RightPunch(Content, GAME_WIDTH, GAME_HEIGHT, GUARD_Y, sGuard, sPunch);
            }
            if (nextPunch != null)
            {
                nextPunch.isActive = true;
            }
            return nextPunch;
        }

    }
}
