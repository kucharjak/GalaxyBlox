using GalaxyBlox.Models;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    class Logo : GameObject
    {
        public float MaxScale = 1f;
        public float MinScale = 1f;
        public int Timer = 0;
        public int TimeLimit = 0;

        private bool ScallingUp = true;

        public bool IsPaused;

        public Logo(Room parentRoom) : base(parentRoom)
        {
            BackgroundImage = Static.Contents.Textures.Logo;
            Origin = new Vector2(.5f, .5f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsPaused || TimeLimit == 0)
                return;

            if (ScallingUp)
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;
                if (Timer >= TimeLimit)
                {
                    Timer = TimeLimit;
                    ScallingUp = false;
                }
            }
            else
            {
                Timer -= gameTime.ElapsedGameTime.Milliseconds;
                if (Timer <= 0)
                {
                    Timer = 0;
                    ScallingUp = true;
                }
            }            

            Scale = MinScale + (float)Timer / TimeLimit * (MaxScale - MinScale);
        }
    }
}