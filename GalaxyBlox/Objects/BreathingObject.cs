using GalaxyBlox.Models;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    class BreathingObject : GameObject
    {
        public Color StartColor;
        public Color EndColor;
        public Color StartTextColor;
        public Color EndTextColor;

        public float MaxScale = 1f;
        public float MinScale = 1f;
        public float MaxAlpha = 1f;
        public float MinAlpha = 1f;
        public int Timer = 0;
        public int TimerLimit = 0;

        public bool IsBreathingIn = true;
        public bool Loop = true;

        public bool IsPaused;

        public BreathingObject(Room parentRoom) : base(parentRoom)
        {
            Origin = new Vector2(.5f, .5f);
            StartColor = Color.White;
            EndColor = Color.White;
            StartTextColor = Color.Black;
            EndTextColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsPaused || TimerLimit == 0)
                return;

            if (IsBreathingIn)
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;
                if (Timer >= TimerLimit)
                {
                    Timer = TimerLimit;
                    IsBreathingIn = false;

                    if (!Loop)
                        IsPaused = true;
                }
            }
            else
            {
                Timer -= gameTime.ElapsedGameTime.Milliseconds;
                if (Timer <= 0)
                {
                    Timer = 0;
                    IsBreathingIn = true;

                    if (!Loop)
                        IsPaused = true;
                }
            }            

            Alpha = MinAlpha + (float)Timer / TimerLimit * (MaxAlpha - MinAlpha);
            Scale = MinScale + (float)Timer / TimerLimit * (MaxScale - MinScale);

            if (!StartColor.Equals(EndColor))
            {
                BaseColor = Color.Red;
                var starColValue = ((float)TimerLimit - Timer) / TimerLimit;
                var endColValue = (float)Timer / TimerLimit;

                var r = (int)(StartColor.R * starColValue + EndColor.R * endColValue);
                var g = (int)(StartColor.G * starColValue + EndColor.G * endColValue);
                var b = (int)(StartColor.B * starColValue + EndColor.B * endColValue);

                BaseColor = new Color(r,g,b);
            }

            if (!StartTextColor.Equals(EndTextColor))
            {
                var starColValue = ((float)TimerLimit - Timer) / TimerLimit;
                var endColValue = (float)Timer / TimerLimit;

                var r = (int)(StartTextColor.R * starColValue + EndTextColor.R * endColValue);
                var g = (int)(StartTextColor.G * starColValue + EndTextColor.G * endColValue);
                var b = (int)(StartTextColor.B * starColValue + EndTextColor.B * endColValue);

                TextColor = new Color(r,g,b);
            }
        }

        public void BreathIn()
        {
            IsPaused = false;
            Timer = 0;
            IsBreathingIn = true;
        }

        public void BreathOut()
        {
            IsPaused = false;
            Timer = TimerLimit;
            IsBreathingIn = false;
        }
    }
}